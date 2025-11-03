namespace Lab08.Commands
{
    public class HelpCommand : ICommand
    {
        public void Execute(Game game)
        {
            DisplayStyle.WriteLine("Available commands:", ConsoleColor.DarkYellow);
            DisplayStyle.WriteLine("  'w'  - Move one room forward", ConsoleColor.DarkYellow);
            DisplayStyle.WriteLine("  's'  - Move one room backward", ConsoleColor.DarkYellow);
            DisplayStyle.WriteLine("  'd'  - Move one room to the right", ConsoleColor.DarkYellow);
            DisplayStyle.WriteLine("  'a'  - Move one room to the left", ConsoleColor.DarkYellow);
            DisplayStyle.WriteLine("  'r'  - Roll a bullet to check a nearby room", ConsoleColor.DarkYellow);
            DisplayStyle.WriteLine("  'i'  - Open Inventory", ConsoleColor.DarkYellow);
            DisplayStyle.WriteLine("  'h'  - Show commands message", ConsoleColor.DarkYellow);
        }
    }
}