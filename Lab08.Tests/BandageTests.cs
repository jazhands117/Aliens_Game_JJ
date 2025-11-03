using NUnit.Framework;
using Lab08.GameDesign;

namespace Lab08.Tests
{
    public class BandageTests
    {
        [Test]
        public void Bandage_Heals_Up_To_Max()
        {
            var game = new Game();
            // reduce health to 20
            game.Player.TakeDamage(30); // 50 -> 20
            var bandage = new Lab08.Items.Bandages { Quantity = 1 };
            game.Player.Inventory.AddItem(bandage);

            Assert.That(game.Player.Health, Is.EqualTo(20));

            // use bandage
            bandage.Use(game);

            // should heal 20 to 40 and consume bandage
            Assert.That(game.Player.Health, Is.EqualTo(40));
            var stored = game.Player.Inventory.GetItemByName("Bandages");
            Assert.That(stored, Is.Null, "Bandage stack should be removed after use.");
        }

        [Test]
        public void Bandage_Caps_At_50_When_Over_30()
        {
            var game = new Game();
            // reduce health to 40
            game.Player.TakeDamage(10); // 50 -> 40
            var bandage = new Lab08.Items.Bandages { Quantity = 1 };
            game.Player.Inventory.AddItem(bandage);

            Assert.That(game.Player.Health, Is.EqualTo(40));

            // use bandage
            bandage.Use(game);

            // should cap at 50
            Assert.That(game.Player.Health, Is.EqualTo(50));
            var stored = game.Player.Inventory.GetItemByName("Bandages");
            Assert.That(stored, Is.Null, "Bandage stack should be removed after use.");
        }

        [Test]
        public void Bandage_Not_Consumed_At_FullHealth()
        {
            var game = new Game();
            // ensure full health
            Assert.That(game.Player.Health, Is.EqualTo(50));
            var bandage = new Lab08.Items.Bandages { Quantity = 1 };
            game.Player.Inventory.AddItem(bandage);

            // use bandage when at full health
            bandage.Use(game);

            // should still be at full health and bandage should not be removed
            Assert.That(game.Player.Health, Is.EqualTo(50));
            var stored = game.Player.Inventory.GetItemByName("Bandages");
            Assert.That(stored, Is.Not.Null, "Bandage should not be consumed when at full health.");
        }
    }
}
