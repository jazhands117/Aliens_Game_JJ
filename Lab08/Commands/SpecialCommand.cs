namespace Lab08.Commands
{
    public class SpecialCommand : ICommand
    {
        private string _prompt;
        public bool Choice { get; set; } // Allow setting for tests
        private bool _isTest;

        public SpecialCommand(string prompt, bool isTest = false)
        {
            _prompt = prompt;
            _isTest = isTest;
        }

        public void Execute(Game game)
        {
            if (_isTest)
                return;
                
            // Write the prompt directly without requiring ENTER
            DisplayUI.WriteMessage(_prompt, ConsoleColor.Cyan);
            while (true)
            {
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                if (keyInfo.Key == ConsoleKey.Y)
                {
                    Choice = true;
                    return;
                }
                else if (keyInfo.Key == ConsoleKey.N)
                {
                    Choice = false;
                    return;
                }
                else
                {
                    DisplayUI.WriteMessage("Invalid input. Please press 'y' or 'n'.", ConsoleColor.Magenta);
                }
            }
        }
    }
}