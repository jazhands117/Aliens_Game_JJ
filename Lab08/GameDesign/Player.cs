namespace Lab08.GameDesign
{
    public class Player
    {
        private static readonly Random _random = new Random();
        public Location Location { get; set; }
        public bool IsAlive { get; private set; } = true;
        public string CauseOfDeath { get; private set; } = "";
        public int BulletsRemaining { get; private set; }
        public int Health { get; private set; } = 50;
        private bool isInfected = false;
        private int infectionCountdown = 0;
        public int TotalDamageTaken { get; private set; }
        public int TotalDamageDealt { get; private set; }
        public bool BossDiscovered { get; set; }

        public Inventory Inventory { get;} = new Inventory();
        public IItem? EquippedWeapon { get; private set; }

        public Player(Location start, int initialBullets = 5)
        {
            Location = start;
            BulletsRemaining = initialBullets;
            TotalDamageDealt = 0;
            TotalDamageTaken = 0;
            BossDiscovered = false;
        }

        public void EquipWeapon(IItem? weapon)
        {
            if (weapon == null)
            {
                EquippedWeapon = null;
                DisplayStyle.WriteLine("You ready your shaking fists.", ConsoleColor.Yellow);
                return;
            }
            if (weapon.Type == ItemType.Weapon && Inventory.HasItem(weapon))
            {
                EquippedWeapon = weapon;
                DisplayStyle.WriteLine($"You equip your {weapon.Name}. It deals {weapon.Damage} damage.", ConsoleColor.Yellow);
            }
        }
        
        public bool HasItem(string name)
        {
            return Inventory.Contains(name);
        }

        public void Move(Direction direction)
        {
            Location = Location.Move(direction);
        }

        public void Kill(string cause)
        {
            IsAlive = false;
            CauseOfDeath = cause;
        }

        public void TakeDamage(int amount)
        {
            Health -= amount;
            TotalDamageTaken = TotalDamageTaken + amount;
            DisplayStyle.WriteLine($"You take {amount} damage. Health remaining: {Health}.", ConsoleColor.Red);
            if (Health <= 0)
            {
                Kill("Health depleted.");
            }
        }

        // Heals the player by the given amount but caps at 50 (max health).
        // Returns the actual amount healed (0 if none). Does not modify TotalDamageTaken.
        public int Heal(int amount)
        {
            if (amount <= 0)
                return 0;
            if (Health >= 50)
            {
                DisplayStyle.WriteLine("You are already at full health.", ConsoleColor.Yellow);
                return 0;
            }
            int before = Health;
            Health = Math.Min(50, Health + amount);
            int healed = Health - before;
            DisplayStyle.WriteLine($"You heal {healed} health. Health: {Health}.", ConsoleColor.Green);
            return healed;
        }
        public int DamageAmount(IItem? weapon)
        {
            IItem? usedWeapon = weapon ?? EquippedWeapon;
            int damage;
            if (usedWeapon != null && usedWeapon.Type == ItemType.Weapon)
                damage = usedWeapon.Damage;
            else
                damage = 0;
            return damage;
        }

        public void DealDamage(Alien alien, IItem? weapon)
        {
            if (!IsAlive)
                return;

            if (alien == null || !alien.IsAlive)
                return;

            //had AI help me on this line//
            IItem? usedWeapon = weapon ?? EquippedWeapon;
            int damage;
            string attackDescription;
            if (usedWeapon != null && usedWeapon.Type == ItemType.Weapon)
            {
                DisplayUI.ClearMessageHistory();
                damage = usedWeapon.Damage;
                attackDescription = $"You attack the alien with your {usedWeapon.Name}, dealing {damage} damage.";

                // Special-case: Plasma Cutter consumes a Charge Node automatically when fired.
                if (usedWeapon.Name.Equals("Plasma Cutter", StringComparison.OrdinalIgnoreCase))
                {
                    var chargeNodes = Inventory.GetItemByName("Charge Nodes");
                    if (chargeNodes != null && chargeNodes.Quantity > 0)
                    {
                        // consume one charge node
                        Inventory.RemoveItem(chargeNodes, 1);
                        DisplayStyle.WriteLine("The Plasma Cutter hums as it consumes a Charge Node.", ConsoleColor.Yellow);
                    }
                    else
                    {
                        // no ammo: fallback to fists
                        damage = 1;
                        attackDescription = $"The Plasma Cutter sputters — no Charge Nodes. You strike with your fists, dealing {damage} damage.";
                    }
                }
            }
            else
            {
                damage = 1; //fist damage//
                attackDescription = $"You punch the alien with your fists, dealing {damage} damage.";
            }
            DisplayStyle.WriteLine(attackDescription, ConsoleColor.Green);
            alien.TakeDamage(damage);
            TotalDamageDealt = TotalDamageDealt + damage;

            if (!alien.IsAlive)
            {
                DisplayStyle.WriteLine("The alien has been defeated!", ConsoleColor.Green);
            }
        }

        public bool UseBullet()
        {
            // bullets are stored in the Inventory as a Bullets item; consume one from inventory
            var bulletsItem = Inventory.GetItemByName("Bullets");
            if (bulletsItem == null || bulletsItem.Quantity <= 0)
                return false;

            // remove one bullet from the stack
            Inventory.RemoveItem(bulletsItem, 1);
            UpdateBulletsFromInventory();
            return true;
        }

        public void UpdateBulletsFromInventory()
        {
            var bulletsItem = Inventory.GetItemByName("Bullets");
            BulletsRemaining = bulletsItem?.Quantity ?? 0;
        }

        public void RollBullet(Direction direction, Game game)
        {
            Location target = Location.Move(direction);
            DisplayUI.ClearMessageHistory();

            if (!game.Map.IsWithinBounds(target))
            {
                DisplayStyle.WriteLine("You roll a bullet into the wall. It clatters uselessly. You stoop to pick it back up.", ConsoleColor.Yellow);
                return;
            }
            if (!UseBullet())
            {
                DisplayStyle.WriteLine("You have no bullets left to roll!", ConsoleColor.Yellow);
                return;
            }

            game.Map.DiscoverRoom(target);
            DisplayMap.MarkTileDiscovered(target, game.Map.GetRoomTypeAt(target));

            var sense = new BulletSense(target);
            if (sense.Detect(game))
            {
                sense.Notify(game);
            }
            else
            {
                DisplayStyle.WriteLine("The bullet rolls harmlessly across the floor and comes to a stop.", ConsoleColor.Yellow);
            }

            foreach (var alien in game.Aliens)
            {
                if (!alien.IsAlive)
                    continue;
                if (alien.IsAlive && alien.Location.Equals(target))
                {
                    game.Map.DiscoverRoom(alien.Location);
                    DisplayMap.MarkTileDiscovered(alien.Location, game.Map.GetRoomTypeAt(alien.Location));
                    DisplayMap.MarkMonsterHit(alien.Location);
                }
                if (alien.IsAlive)
                {
                    alien.MoveTowardsPlayer(game);
                }
            }
            // mark that aliens have moved due to the bullet roll so the main game loop won't move them again
            game.AliensMovedThisTurn = true;
        }

        public void MarkBossDiscovered()
        {
            BossDiscovered = true;
        }

        public void Infect(int turns)
        {
            if (isInfected)
            {
                return; //already infected//
            }
            isInfected = true;
            infectionCountdown = turns; //number of turns before death//
            DisplayStyle.WriteLine("You feel sharp pains in your chest. Something moves. Could it be your heart?", ConsoleColor.Yellow);
        }

        public void AdvanceInfection(Map map)
        {
            if (!isInfected)
                return;

            if (map.GetRoomTypeAt(Location) == RoomType.MedBay)
            {
                CureInfection();
                return;
            }

            infectionCountdown--;
            if (infectionCountdown <= 0)
            {
                DisplayUI.ClearMessageHistory();
                Console.WriteLine();
                DisplayStyle.WriteLine("The alien embryo bursts from your chest. Blood sprays and clouds your vision.", ConsoleColor.Red);
                DisplayStyle.WriteLine("You fall, and hear the alien scuttle away into the darkness.", ConsoleColor.Red);
                Console.WriteLine();
                Kill("Succumbed to alien infection.");
            }
            else if (infectionCountdown <= 3)
            {
                Console.WriteLine();
                DisplayStyle.WriteLine($"You are in critical condition. The alien will emerge in {infectionCountdown} turns.", ConsoleColor.Yellow);
                DisplayStyle.WriteLine("Find the MedBay immediately.", ConsoleColor.Yellow);
                Console.WriteLine();
            }
            else if (infectionCountdown <= 6)
            {
                Console.WriteLine();
                DisplayStyle.WriteLine($"You feel the alien moving inside you. It's growing. You have {infectionCountdown} turns left.", ConsoleColor.Yellow);
                DisplayStyle.WriteLine("You must reach the MedBay soon.", ConsoleColor.Yellow);
                Console.WriteLine();
            }
            else if (infectionCountdown <= 8)
            {
                Console.WriteLine();
                DisplayStyle.WriteLine($"A persistent ache in your chest reminds you of the alien within. You have {infectionCountdown} turns before it emerges.", ConsoleColor.Yellow);
                DisplayStyle.WriteLine("Head to the MedBay, quickly.", ConsoleColor.Yellow);
                Console.WriteLine();
            }
            else if (infectionCountdown == 9)
            {
                Console.WriteLine();
                DisplayStyle.WriteLine($"Your gut knows the Facehugger has implanted something in you. You have {infectionCountdown} turns left before it emerges.", ConsoleColor.Yellow);
                DisplayStyle.WriteLine("Seek medical in the MedBay.", ConsoleColor.Yellow);
                Console.WriteLine();
            }
        }
        
        public void CureInfection()
        {
            if (isInfected)
            {
                isInfected = false;
                infectionCountdown = 0;
                DisplayStyle.WriteLine("You feel relief as the alien within you stops moving. But is it really cured?", ConsoleColor.Green);
            }
        }
    }
}