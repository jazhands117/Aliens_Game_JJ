namespace Lab08.Commands
{
    public class UseBulletCommand : ICommand
    {
        public void Execute(Game game)
        {
            DisplayStyle.WriteLine("Which direction would you like to roll a bullet? (w/a/s/d)", ConsoleColor.Cyan);
            ConsoleKeyInfo keyInfo = Console.ReadKey(true);
            Direction direction = keyInfo.Key switch
            {
                ConsoleKey.W => Direction.North,
                ConsoleKey.A => Direction.West,
                ConsoleKey.S => Direction.South,
                ConsoleKey.D => Direction.East,
                _ => throw new InvalidOperationException("Invalid direction key.")
            };
            game.Player.RollBullet(direction, game);
        }
    }
}