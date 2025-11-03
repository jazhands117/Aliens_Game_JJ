using System.Collections.Generic;
namespace Lab08.GameDesign
{
    public class Map
    {
        private readonly RoomType[,] rooms;
        private readonly bool[,] discovered;
        private readonly Random random = new();

        public int Width { get; } = 12;
        public int Height { get; } = 12;

        public Map()
        {
            rooms = new RoomType[Height, Width];
            discovered = new bool[Height, Width];
            Initialize();
        }

        public void Initialize()
        {
            for (int row = 0; row < Height; row++)
            {
                for (int col = 0; col < Width; col++)
                {
                    rooms[row, col] = RoomType.Normal;
                    discovered[row, col] = false;
                }
            }
        }

        public void SetStart(Location startLoc)
        {
            SetRoomType(startLoc, RoomType.Airlock);
            // reveal the starting airlock so the player sees it immediately
            DiscoverRoom(startLoc);
            var medbay = GetUniqueRandomLocation();
            SetRoomType(medbay, RoomType.MedBay);
            var mechbay = GetUniqueRandomLocation();
            SetRoomType(mechbay, RoomType.MechBay);

            //placing pits randomly//
            //increase to 10 pits as requested
            for (int i = 0; i < 10; i++)
            {
                var pitLocation = GetUniqueRandomLocation();
                SetRoomType(pitLocation, RoomType.Pit);
            }
        }

        public void SetRoomType(Location loc, RoomType type)
        {
            if (IsWithinBounds(loc))
            {
                rooms[loc.Row, loc.Column] = type;
            }
            else
            {
                throw new ArgumentOutOfRangeException("Location is out of map bounds.");
            }
        }

        public RoomType GetRoomTypeAt(Location loc)
        {
            if (IsWithinBounds(loc))
            {
                return rooms[loc.Row, loc.Column];
            }
            else
            {
                return RoomType.OffMap;
            }
        }

        public bool IsWithinBounds(Location loc)
        {
            return loc.Row >= 0 && loc.Row < Height && loc.Column >= 0 && loc.Column < Width;
        }

        public void DiscoverRoom(Location loc)
        {
            if (IsWithinBounds(loc))
            {
                discovered[loc.Row, loc.Column] = true;
            }
        }

        public Location GetRoomLocation(RoomType type)
        {
            for (int row = 0; row < Height; row++)
            {
                for (int col = 0; col < Width; col++)
                {
                    if (rooms[row, col] == type)
                    {
                        return new Location(row, col);
                    }
                }
            }
            throw new Exception($"No room of type {type} found on the map.");
        }

        public bool IsDiscovered(Location loc)
        {
            return IsWithinBounds(loc) && discovered[loc.Row, loc.Column];
        }

        public double CalculateDiscovered()
        {
            int totalRooms = Width * Height;
            int discoveredCount = 0;

            for (int row = 0; row < Height; row++)
            {
                for (int col = 0; col < Width; col++)
                {
                    if (discovered[row, col])
                    {
                        discoveredCount = discoveredCount + 1;
                    }
                }
            }
            double percent = (double)discoveredCount / (double)totalRooms * 100.0;
            return percent;
        }

        public Location[] GetCardinalAdjacentRooms(Location loc)
        {
            var cardinalRooms = new List<Location>();
            var directions = new Direction[] { Direction.North, Direction.East, Direction.South, Direction.West };

            foreach (var direction in directions)
            {
                Location neighbor = loc.GetAdjacentLocation(direction);
                if (IsWithinBounds(neighbor))
                    cardinalRooms.Add(neighbor);
            }
            return cardinalRooms.ToArray();
        }

        public Location[] GetAllAdjacentRooms(Location loc)
        {
            var adjacentRooms = new List<Location>();

            //check all 8 surrounding positions (including diagonals)//
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    //skip the center position (the location itself)//
                    if (dx == 0 && dy == 0) continue;

                    Location neighbor = new Location(loc.Row + dx, loc.Column + dy);
                    if (IsWithinBounds(neighbor))
                        adjacentRooms.Add(neighbor);
                }
            }
            return adjacentRooms.ToArray();
        }

        public bool HasNeighborWithType(Location loc, RoomType type)
        {
            foreach (var neighbor in GetCardinalAdjacentRooms(loc))
            {
                if (GetRoomTypeAt(neighbor) == type)
                    return true;
            }
            return false;
        }

        public Location GetRandomNeighbor(Location loc)
        {
            var neighbors = GetCardinalAdjacentRooms(loc).Where(n => GetRoomTypeAt(n) != RoomType.Airlock).ToList();
            if (neighbors.Count == 0)
                throw new Exception("No valid neighboring rooms available.");
            return neighbors[random.Next(neighbors.Count)];
        }

        public Location GetRandomLocation()
        {
            int row = random.Next(Height);
            int col = random.Next(Width);
            return new Location(row, col);
        }

        private Location GetUniqueRandomLocation()
        {
            Location loc;
            do
            {
                loc = GetRandomLocation();
            } while (GetRoomTypeAt(loc) != RoomType.Normal);
            return loc;
        }


    }
}