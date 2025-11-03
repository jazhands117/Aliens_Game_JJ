namespace Lab08.Commands
{
    public class MoveCommand : ICommand
    {
        private readonly Direction _direction;

        public MoveCommand(Direction direction)
        {
            _direction = direction;
        }

        public void Execute(Game game)
        {
            Location newLocation = game.Player.Location.Move(_direction);
            // if the location is off-map, GetRoomTypeAt will return OffMap
            // remember whether this room was already discovered before we move into it
            bool wasDiscovered = game.Map.IsDiscovered(newLocation);

            if (game.Map.GetRoomTypeAt(newLocation) != RoomType.OffMap)
            {
                game.Player.Move(_direction);
                DisplayStyle.WriteLine($"You moved {_direction.ToString().ToLower()}.", ConsoleColor.DarkYellow);
            }
            else
            {
                DisplayStyle.WriteLine("There is a wall there.", ConsoleColor.Red);
            }
            game.Map.DiscoverRoom(game.Player.Location);
            DisplayMap.MarkTileDiscovered(game.Player.Location, game.Map.GetRoomTypeAt(game.Player.Location));

            foreach (var sense in game.Senses)
            {
                if (sense.Detect(game))
                {
                    sense.Notify(game);
                }
            }

            if (game.CurrentRoom == RoomType.Pit && !wasDiscovered)
            {
                DisplayStyle.WriteLine("In your stumbling, your leg falls into a pit. You twist your ankle.", ConsoleColor.Red);
                game.Player.TakeDamage(5);
            }
        }
    }
}