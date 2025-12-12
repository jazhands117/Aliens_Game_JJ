using NUnit.Framework;
using Lab08.GameDesign;
using Lab08.Items;
using Lab08.Commands;

namespace Lab08.Tests
{
    [TestFixture]
    public class PowerSupplyTests
    {
        [Test]
        public void PowerSupply_Only_Works_In_MechBay()
        {
            var game = new Game();
            var player = game.Player;
            var powerSupply = new PowerSupply { Quantity = 1 };
            player.Inventory.AddItem(powerSupply);

            // try to use power supply outside MechBay
            Location normalRoom = new Location(5, 5);
            while (game.Map.GetRoomTypeAt(normalRoom) != RoomType.Normal)
            {
                normalRoom = game.Map.GetRandomLocation();
            }
            player.Location = normalRoom;
            
            powerSupply.Use(game);
            
            // should still be in inventory!!
            Assert.That(player.Inventory.HasItem(powerSupply), Is.True, "Power Supply should remain in inventory when used outside MechBay");
            Assert.That(game.IsBossFightActive, Is.False, "Boss fight should not activate outside MechBay");
            Assert.That(game.Progress & GameProgress.MechActivated, Is.EqualTo(GameProgress.None), "MechActivated progress should not be set outside MechBay");
        }

        [Test]
        public void PowerSupply_Triggers_Progress_When_Used_In_MechBay()
        {
            var game = new Game();
            var player = game.Player;
            var powerSupply = new PowerSupply { Quantity = 1 };
            player.Inventory.AddItem(powerSupply);

            var mechBayLocation = game.Map.GetRoomLocation(RoomType.MechBay);
            player.Location = mechBayLocation;

            var testCommand = new SpecialCommand("Test", isTest: true) { Choice = true };
            powerSupply.UseWithCommand(game, testCommand, testCommand);

            Assert.That(game.Progress.HasFlag(GameProgress.MechActivated), Is.True, "Using Power Supply should set MechActivated progress");
            Assert.That(game.IsBossFightActive, Is.True, "Boss fight should be active");
            Assert.That(player.Inventory.HasItem(powerSupply), Is.False, "Power Supply should be consumed");
        }
    }
}