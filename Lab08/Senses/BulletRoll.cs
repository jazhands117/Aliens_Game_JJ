namespace Lab08.Senses
{
    public class BulletSense : ISense
    {
        private readonly Location targetLocation;
        public BulletSense(Location targetLocation)
        {
            this.targetLocation = targetLocation;
        }

        public bool Detect(Game game)
        {
            if (!game.Map.IsWithinBounds(targetLocation))
            {
                return false;
            }
            RoomType type = game.Map.GetRoomTypeAt(targetLocation);
            bool alienPresent = game.Aliens.Any(alien => alien.Location.Equals(targetLocation) && alien.IsAlive);
            return type != RoomType.Normal || alienPresent;
        }
        public void Notify(Game game)
        {
            RoomType type = game.Map.GetRoomTypeAt(targetLocation);
            if (game.Aliens.Any(alien => alien.Location.Equals(targetLocation) && alien.IsAlive))
            {
                DisplayStyle.WriteLine("You hear movement and angry hissing. There is something in the room.", ConsoleColor.Red);
            }
            else if (type == RoomType.MedBay)
            {
                DisplayStyle.WriteLine("You hear the faint hum of medical equipment. The bullet clinks against a surgical table.", ConsoleColor.Cyan);
            }
            else if (type == RoomType.MechBay)
            {
                DisplayStyle.WriteLine("You hear the mechanical whirring of a vehicle bay. The bullet clinks into factory equipment.", ConsoleColor.Cyan);
            }
            else if (type == RoomType.Pit)
            {
                DisplayStyle.WriteLine("You hear the bullet roll and clatter as it drops into damaged floors.", ConsoleColor.Cyan);
            }
            else
            {
                DisplayStyle.WriteLine("You hear the bullet roll and clatter on the floor. The room appears to be empty.", ConsoleColor.Cyan);
            }
        }
    }
}