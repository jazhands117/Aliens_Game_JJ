using NUnit.Framework;
using Lab08.GameDesign;

namespace Lab08.Tests
{
    public class MapTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Map_Is_12_by_12()
        {
            var map = new Map();
            Assert.That(map.Width, Is.EqualTo(12));
            Assert.That(map.Height, Is.EqualTo(12));
        }

        [Test]
        public void SetStart_Places_Exactly_Ten_Pits()
        {
            var map = new Map();
            // use a deterministic start location
            var start = new Location(0, 0);
            map.SetStart(start);

            int pitCount = 0;
            for (int r = 0; r < map.Height; r++)
            {
                for (int c = 0; c < map.Width; c++)
                {
                    if (map.GetRoomTypeAt(new Location(r, c)) == RoomType.Pit)
                        pitCount++;
                }
            }

            Assert.That(pitCount, Is.EqualTo(10), "Map should contain exactly 10 pits after SetStart.");
        }

        [Test]
        public void DiscoverRoom_Toggles_IsDiscovered()
        {
            var map = new Map();
            var loc = new Location(3, 4);
            Assert.That(map.IsDiscovered(loc), Is.False);
            map.DiscoverRoom(loc);
            Assert.That(map.IsDiscovered(loc), Is.True);
        }

        [Test]
        public void CalculateDiscovered_Returns_Correct_Percentage()
        {
            var map = new Map();
            // discover three tiles
            map.DiscoverRoom(new Location(0, 0));
            map.DiscoverRoom(new Location(0, 1));
            map.DiscoverRoom(new Location(0, 2));

            double percent = map.CalculateDiscovered();
            double expected = (3.0 / (map.Width * map.Height)) * 100.0;
            Assert.That(percent, Is.EqualTo(expected).Within(1e-6));
        }

        [Test]
        public void Game_Starts_At_Airlock()
        {
            var game = new Game();
            // Player location should match the map's Airlock location
            Assert.That(game.Player.Location, Is.EqualTo(game.Map.GetRoomLocation(RoomType.Airlock)));
        }
    }
}