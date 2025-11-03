namespace Lab08.GameDesign
{
    public record Location(int Row, int Column)
    {
        public int X => Row;
        public int Y => Column;

        public Location Move(Direction direction)
        {
            return direction switch
            {
                Direction.North => new Location(Row - 1, Column),
                Direction.East => new Location(Row, Column + 1),
                Direction.South => new Location(Row + 1, Column),
                Direction.West => new Location(Row, Column - 1),
                Direction.NorthEast => new Location(Row - 1, Column + 1),
                Direction.SouthEast => new Location(Row + 1, Column + 1),
                Direction.SouthWest => new Location(Row + 1, Column - 1),
                Direction.NorthWest => new Location(Row - 1, Column - 1),
                _ => this
            };
        }

        public bool IsAdjacent(Location other)
        {
            int rowDiff = Math.Abs(Row - other.Row);
            int colDiff = Math.Abs(Column - other.Column);
            return rowDiff <= 1 && colDiff <= 1 && !(rowDiff == 0 && colDiff == 0);
        }

        public bool IsCardinallyAdjacent(Location other)
        {
            int rowDiff = Math.Abs(Row - other.Row);
            int colDiff = Math.Abs(Column - other.Column);
            return (rowDiff == 1 && colDiff == 0) || (rowDiff == 0 && colDiff == 1);
        }

        public Location GetAdjacentLocation(Direction direction)
        {
            return Move(direction);
        }
    }
}