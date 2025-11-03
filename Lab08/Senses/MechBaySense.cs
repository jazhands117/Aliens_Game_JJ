namespace Lab08.Senses
{
    public class MechbaySense : ISense
    {
        public bool Detect(Game game)
        {
            foreach (var neighbor in game.Map.GetCardinalAdjacentRooms(game.Player.Location))
            {
                if (game.Map.GetRoomTypeAt(neighbor) == RoomType.MechBay)
                    return true;
            }
            return false;
        }
        public void Notify(Game game)
        {
            if (!game.VisitedMechBay)
                DisplayStyle.WriteLine("You hear a loud mechanical whirring. Perhaps this is something worth looking into.", ConsoleColor.Cyan);
        }
    }
}