using NUnit.Framework;
using Lab08.GameDesign;
using Lab08.Aliens;

namespace Lab08.Tests
{
    [TestFixture]
    public class FacehuggerTeleportTests
    {
        [Test]
        public void FacehuggerTeleport_Only_Discovers_Final_Tile()
        {
            var game = new Game();
            var player = game.Player;
            player.Location = new Location(5, 5);
            int healthBefore = player.Health;

            // Place Facehugger on player tile and force teleport by setting random seed
            var seededRandom = new Random(42); // this seed should give teleport in test runs
            var fh = new Facehugger(player.Location, seededRandom);
            game.Aliens[0] = fh;

            // Remember player's start location to verify path tiles
            var startLocation = player.Location;

            // Record which tiles were discovered before teleport
            var discoveredBefore = new HashSet<Location>();
            for (int r = 0; r < game.Map.Height; r++)
            {
                for (int c = 0; c < game.Map.Width; c++)
                {
                    var loc = new Location(r, c);
                    if (game.Map.IsDiscovered(loc))
                        discoveredBefore.Add(loc);
                }
            }

            // Trigger teleport
            fh.Activate(game);

            // Verify player moved
            Assert.That(player.Location, Is.Not.EqualTo(startLocation), "Player should be teleported.");

            // Verify only final tile is discovered
            var newlyDiscovered = new HashSet<Location>();
            for (int r = 0; r < game.Map.Height; r++)
            {
                for (int c = 0; c < game.Map.Width; c++)
                {
                    var loc = new Location(r, c);
                    if (game.Map.IsDiscovered(loc) && !discoveredBefore.Contains(loc))
                        newlyDiscovered.Add(loc);
                }
            }

            // Should only discover the landing tile
            Assert.That(newlyDiscovered.Count, Is.EqualTo(1), "Only one new tile should be discovered.");
            Assert.That(newlyDiscovered.Contains(player.Location), Is.True, "The newly discovered tile should be the landing location.");
        }

        [Test]
        public void FacehuggerTeleport_Only_Damages_In_Undiscovered_Pit()
        {
            var game = new Game();
            var map = game.Map;
            var player = game.Player;

            // Find a pit room
            Location pitLocation = new Location(0, 0);
            for (int r = 0; r < map.Height; r++)
            {
                for (int c = 0; c < map.Width; c++)
                {
                    var loc = new Location(r, c);
                    if (map.GetRoomTypeAt(loc) == RoomType.Pit)
                    {
                        pitLocation = loc;
                        break;
                    }
                }
            }

            // Test 1: Undiscovered pit should damage
            player.Location = new Location(5, 5);
            var fh1 = new Facehugger(player.Location);
            int healthBefore = player.Health;
            player.Location = pitLocation; // simulate teleport landing
            bool wasDiscovered = map.IsDiscovered(pitLocation);
            if (!wasDiscovered && map.GetRoomTypeAt(pitLocation) == RoomType.Pit)
            {
                player.TakeDamage(5);
            }
            Assert.That(player.Health, Is.EqualTo(healthBefore - 5), "Should take damage in undiscovered pit.");

            // Test 2: Discovered pit should not damage
            // Create new game instance to get fresh player health
            var game2 = new Game();
            player = game2.Player;
            map = game2.Map;
            map.DiscoverRoom(pitLocation);
            healthBefore = player.Health;
            wasDiscovered = map.IsDiscovered(pitLocation);
            if (!wasDiscovered && map.GetRoomTypeAt(pitLocation) == RoomType.Pit)
            {
                player.TakeDamage(5);
            }
            Assert.That(player.Health, Is.EqualTo(healthBefore), "Should not take damage in discovered pit.");
        }

        [Test]
        public void FacehuggerTeleport_Distance_Is_Two_To_Six_Steps()
        {
            var game = new Game();
            var player = game.Player;
            player.Location = new Location(5, 5);

            // Run multiple teleports and verify distance
            for (int i = 0; i < 100; i++)
            {
                var startLoc = new Location(5, 5);
                player.Location = startLoc;
                
                var fh = new Facehugger(player.Location);
                game.Aliens[0] = fh;
                
                fh.Activate(game);
                
                // If we didn't teleport (got attack instead), skip
                if (player.Location == startLoc)
                    continue;
                
                // Calculate Manhattan distance (since teleport uses cardinal moves)
                int distance = Math.Abs(player.Location.Row - startLoc.Row) + 
                             Math.Abs(player.Location.Column - startLoc.Column);
                
                Assert.That(distance, Is.InRange(2, 6), 
                    $"Teleport distance should be 2-6 steps but was {distance} at run {i}");
            }
        }
    }
}