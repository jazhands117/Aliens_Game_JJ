using System.Collections.Generic;
namespace Lab08.GameDesign
{
    public static class DirectionHelper
    {
        private static readonly Random random = new();
        private static readonly Direction[] allDirections = (Direction[])Enum.GetValues(typeof(Direction));
        private static readonly Direction[] cardinalDirections = new Direction[]
        {
            Direction.North,
            Direction.East,
            Direction.South,
            Direction.West
        };
        public static Direction GetRandomDirection(bool allowDiagonals = true)
        {
            Direction[] directions;
            if (allowDiagonals)
            {
                directions = allDirections;
            }
            else
            {
                directions = cardinalDirections;
            }
            int index = random.Next(directions.Length);
            return directions[index];
        }
        public static IEnumerable<Direction> GetAllDirections(bool allowDiagonals = true)
        {
            if (allowDiagonals)
            {
                return allDirections;
            }
            else
            {
                return cardinalDirections;
            }
        }
    }
}