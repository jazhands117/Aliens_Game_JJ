namespace Lab08.Displays
{
    public static class DisplayUI
    {
        private static int _mapHeight;
        private static readonly Queue<(string message, ConsoleColor color)> _messageHistory = new();
        private static bool _isStatusMessage = false; // Track if we're in a status update sequence

        public static void DrawMap(Game game)
        {
            // Draw map at the top
            Console.SetCursorPosition(0, 0);
            DisplayMap.ShowMap(game);
            _mapHeight = Console.CursorTop;

            // Draw a separator line between map and messages
            Console.WriteLine(new string('-', Console.WindowWidth));
            
            // Show message history
            ShowMessageHistory();
        }

        public static void WriteMessage(string message, ConsoleColor color = ConsoleColor.White)
        {
            // If this is the start of a new status update (movement/health/location messages)
            if (message.StartsWith("You moved") || message.StartsWith("You rolled"))
            {
                // Clear previous status messages
                _messageHistory.Clear();
                _isStatusMessage = true;
            }
            // If we're in a status update and see a prompt, end the status update sequence
            else if (_isStatusMessage && message.Contains("what would you like to do", StringComparison.OrdinalIgnoreCase))
            {
                _isStatusMessage = false;
            }

            // Add new message to history
            _messageHistory.Enqueue((message, color));

            // Show the updated message history
            ShowMessageHistory();
        }

        public static void AddMessage(string message, ConsoleColor color = ConsoleColor.White)
        {
            WriteMessage(message, color);
        }

        private static void ShowMessageHistory()
        {
            // Clear message area
            ClearMessageArea();

            // Start writing messages below the map and separator
            int startLine = _mapHeight + 2; // +2 for the separator line
            int currentLine = startLine;

            foreach (var (message, color) in _messageHistory)
            {
                Console.SetCursorPosition(0, currentLine);
                Console.ForegroundColor = color;
                Console.WriteLine(message);
                Console.ResetColor();
                currentLine++;
            }

            // Set cursor to line after messages
            Console.SetCursorPosition(0, currentLine);
        }

        private static void ClearMessageArea()
        {
            int startLine = _mapHeight + 2; // +2 for the separator line
            for (int i = startLine; i < Console.WindowHeight; i++)
            {
                Console.SetCursorPosition(0, i);
                Console.Write(new string(' ', Console.WindowWidth));
            }
        }

        public static void ClearMessageHistory()
        {
            _messageHistory.Clear();
            ClearMessageArea();
        }
    }
}