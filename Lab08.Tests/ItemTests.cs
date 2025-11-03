using NUnit.Framework;
using Lab08.GameDesign;

namespace Lab08.Tests
{
    public class ItemTests
    {
        [Test]
        public void Items_Are_Placed_Correctly_At_Game_Start()
        {
            var game = new Game();

            // Count bullet batches and total bullets
            int bulletBatches = 0;
            int totalBullets = 0;
            foreach (var kvp in game.ItemsOnMap)
            {
                foreach (var item in kvp.Value)
                {
                    if (item.Name.Equals("Bullets", StringComparison.OrdinalIgnoreCase))
                    {
                        bulletBatches++;
                        totalBullets += item.Quantity;
                    }
                }
            }

            Assert.That(bulletBatches, Is.EqualTo(6), "There should be 6 bullet batches placed on the map.");
            Assert.That(totalBullets, Is.EqualTo(30), "Total bullets placed on the map should equal 30.");

            // There should be at least one bullet batch adjacent (cardinal) to the Airlock
            var airlock = game.Map.GetRoomLocation(RoomType.Airlock);
            bool foundAdjacent = false;
            foreach (var kvp in game.ItemsOnMap)
            {
                var loc = kvp.Key;
                if (loc.IsCardinallyAdjacent(airlock))
                {
                    if (kvp.Value.Any(i => i.Name.Equals("Bullets", StringComparison.OrdinalIgnoreCase)))
                    {
                        foundAdjacent = true;
                        break;
                    }
                }
            }
            Assert.IsTrue(foundAdjacent, "At least one bullet batch should be adjacent to the Airlock.");

            // Bandages: 5 single stacks + 1 stack of 3 in MedBay => total quantity 8
            int bandageStacks = 0;
            int bandageTotal = 0;
            foreach (var kvp in game.ItemsOnMap)
            {
                foreach (var item in kvp.Value)
                {
                    if (item.Name.Equals("Bandages", StringComparison.OrdinalIgnoreCase) || item.Name.Equals("Bandage", StringComparison.OrdinalIgnoreCase))
                    {
                        bandageStacks++;
                        bandageTotal += item.Quantity;
                    }
                }
            }
            Assert.That(bandageStacks, Is.EqualTo(6), "There should be 6 bandage stacks (5 random singles + 1 MedBay stack).");
            Assert.That(bandageTotal, Is.EqualTo(8), "Total bandages should equal 8 (5 random + 3 in MedBay).");

            // Weapons: ensure at least one WoodenBat, Machete, and PlasmaCutter present on the map
            bool hasWoodenBat = false, hasMachete = false, hasPlasmaCutter = false;
            foreach (var kvp in game.ItemsOnMap)
            {
                foreach (var item in kvp.Value)
                {
                    if (item.Name.Equals("Wooden Bat", StringComparison.OrdinalIgnoreCase) || item.Name.Equals("Wooden Bat", StringComparison.OrdinalIgnoreCase))
                        hasWoodenBat = true;
                    if (item.Name.Equals("Machete", StringComparison.OrdinalIgnoreCase))
                        hasMachete = true;
                    if (item.Name.Equals("Plasma Cutter", StringComparison.OrdinalIgnoreCase) || item.Name.Equals("PlasmaCutter", StringComparison.OrdinalIgnoreCase))
                        hasPlasmaCutter = true;
                }
            }
            Assert.IsTrue(hasWoodenBat, "Map should contain a Wooden Bat.");
            Assert.IsTrue(hasMachete, "Map should contain a Machete.");
            Assert.IsTrue(hasPlasmaCutter, "Map should contain a Plasma Cutter.");
        }

        [Test]
        public void RollBullet_Decreases_Bullets_And_Discovers_Target()
        {
            var game = new Game();
            // place player at a central location to ensure adjacent tile exists
            game.Player.Location = new Location(6, 6);
            game.Player.UpdateBulletsFromInventory();

            int before = game.Player.BulletsRemaining;
            var target = game.Player.Location.Move(Direction.North);
            Assert.IsTrue(game.Map.IsWithinBounds(target), "Target must be within map bounds for the test.");

            game.Player.RollBullet(Direction.North, game);

            Assert.That(game.Player.BulletsRemaining, Is.EqualTo(before - 1), "Rolling a bullet should consume one bullet from inventory.");
            Assert.That(game.Map.IsDiscovered(target), Is.True, "The room the bullet rolled into should be discovered.");
        }

        [Test]
        public void Inventory_Add_And_Remove_Updates_BulletsRemaining()
        {
            var game = new Game();
            // add bullets to the existing stack and verify totals update accordingly
            int before = game.Player.BulletsRemaining;
            var bullets = new Lab08.Items.Bullets { Quantity = 7 };
            game.Player.Inventory.AddItem(bullets);
            game.Player.UpdateBulletsFromInventory();
            Assert.That(game.Player.BulletsRemaining, Is.EqualTo(before + 7));

            var stored = game.Player.Inventory.GetItemByName("Bullets");
            Assert.That(stored, Is.Not.Null);
            game.Player.Inventory.RemoveItem(stored!, 2);
            game.Player.UpdateBulletsFromInventory();
            Assert.That(game.Player.BulletsRemaining, Is.EqualTo(before + 7 - 2));
        }

        [Test]
        public void PlasmaCutter_Consumes_ChargeNode_On_Attack()
        {
            var game = new Game();

            // give player a plasma cutter and some charge nodes
            var cutter = new Lab08.Items.PlasmaCutter { Quantity = 1 };
            var charges = new Lab08.Items.PlasmaCharge { Quantity = 2 };
            game.Player.Inventory.AddItem(cutter);
            game.Player.Inventory.AddItem(charges);
            game.Player.EquipWeapon(cutter);

            var before = game.Player.Inventory.GetItemByName("Charge Nodes")?.Quantity ?? 0;

            var alien = new Lab08.Aliens.Xenomorph(game.Map.GetRandomLocation());
            // perform the attack
            game.Player.DealDamage(alien, game.Player.EquippedWeapon);

            var after = game.Player.Inventory.GetItemByName("Charge Nodes")?.Quantity ?? 0;
            Assert.That(after, Is.EqualTo(before - 1), "Using the Plasma Cutter should consume one Charge Node when available.");
        }

        [Test]
        public void PlasmaCutter_Falls_Back_To_Fists_When_No_ChargeNodes()
        {
            var game = new Game();

            var cutter = new Lab08.Items.PlasmaCutter { Quantity = 1 };
            game.Player.Inventory.AddItem(cutter);
            game.Player.EquipWeapon(cutter);

            // ensure no charge nodes are present
            var existing = game.Player.Inventory.GetItemByName("Charge Nodes");
            if (existing != null)
                game.Player.Inventory.RemoveStack(existing);

            int beforeDamage = game.Player.TotalDamageDealt;

            var alien = new Lab08.Aliens.Xenomorph(game.Map.GetRandomLocation());
            game.Player.DealDamage(alien, game.Player.EquippedWeapon);

            int afterDamage = game.Player.TotalDamageDealt;
            Assert.That(afterDamage, Is.EqualTo(beforeDamage + 1), "When no Charge Nodes are available the Plasma Cutter should deal fist damage (1).");
        }
    }
}