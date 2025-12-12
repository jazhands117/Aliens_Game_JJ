using NUnit.Framework;
using Lab08.GameDesign;

namespace Lab08.Tests
{
    public class AlienTests
    {
        [Test]
        public void Facehugger_Encounter_Produces_Both_Attack_And_Teleport_Over_Many_Runs()
        {
            bool sawAttack = false;
            bool sawTeleport = false;
            int runs = 200;

            for (int i = 0; i < runs && (!sawAttack || !sawTeleport); i++)
            {
                var game = new Game();
                // put player roughly in center
                game.Player.Location = new Location(6, 6);
                int healthBefore = game.Player.Health;

                // place a Facehugger on the player's tile
                var fh = new Lab08.Aliens.Facehugger(game.Player.Location);
                // replace first alien for simplicity
                game.Aliens[0] = fh;

                // activate the facehugger (this will either attack or teleport)
                fh.Activate(game);

                // attack outcome: facehugger dies and player takes ~10 damage
                if (!fh.IsAlive && game.Player.Health <= healthBefore - 10)
                {
                    sawAttack = true;
                }
                // teleport outcome: player location changed from start
                if (!game.Player.Location.Equals(new Location(6, 6)))
                {
                    sawTeleport = true;
                    // final tile must be discovered because teleport discovers landing tile
                    Assert.IsTrue(game.Map.IsDiscovered(game.Player.Location), "Teleport landing tile should be discovered.");
                }
            }

            Assert.IsTrue(sawAttack, "Across many runs we should observe at least one Facehugger attack outcome.");
            Assert.IsTrue(sawTeleport, "Across many runs we should observe at least one Facehugger teleport outcome.");
        }

        [Test]
        public void Xenomorph_Never_Moves_Into_Player_Tile()
        {
            var game = new Game();
            // choose distinct positions
            game.Player.Location = new Location(5, 5);
            var xLoc = new Location(2, 5);
            var xm = new Lab08.Aliens.Xenomorph(xLoc);
            game.Aliens[0] = xm;

            // perform several move-toward steps
            for (int i = 0; i < 20; i++)
            {
                xm.MoveTowardsPlayer(game);
                Assert.IsFalse(xm.Location.Equals(game.Player.Location), "Xenomorph should not move into the player's tile.");
            }
        }
    }
}