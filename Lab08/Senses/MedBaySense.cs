namespace Lab08.Senses
{
    public class MedbaySense : ISense
    {
        public bool Detect(Game game)
        {
            foreach (var neighbor in game.Map.GetCardinalAdjacentRooms(game.Player.Location))
            {
                if (game.Map.GetRoomTypeAt(neighbor) == RoomType.MedBay)
                    return true;
            }
            return false;
        }
        public void Notify(Game game)
        {
            if (!game.VisitedMedbay)
                DisplayStyle.WriteLine("You hear a low hum. Perhaps this is something worth looking into.", ConsoleColor.Cyan);
        }
    }
}