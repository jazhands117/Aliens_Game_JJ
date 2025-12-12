namespace Lab08.Displays
{
    public static class DisplayUI
    {
        private static int _mapHeight;
        private static readonly Queue<(string message, ConsoleColor color)> _messageHistory = new();
        private static readonly object _lock = new();
        private static bool _isStatusMessage = false; 

        public static void DrawMap(Game game)
        {
            try
            {
                lock (_lock)
                {
                    Console.SetCursorPosition(0, 0);
                    DisplayMap.ShowMap(game);
                    _mapHeight = Console.CursorTop;

                    int width = Math.Max(0, Console.WindowWidth);
                    Console.WriteLine(new string('-', width));

                    ShowMessageHistory();
                }
            }
            catch (System.ArgumentOutOfRangeException)
            {
                lock (_lock)
                {
                    _mapHeight = 0;
                    try
                    {
                        Console.SetCursorPosition(0, 0);
                        Console.WriteLine("Map unavailable in this environment.");
                    }
                    catch
                    {
                        // ignore further console errors
                    }

                    try
                    {
                        ShowMessageHistory();
                    }
                    catch
                    {
                        // ignore
                    }
                }
            }
            catch
            {
                // ignore other console errors in test/non-interactive envs
            }
        }

        public static void WriteMessage(string message, ConsoleColor color = ConsoleColor.White)
        {
            if (message.StartsWith("You moved") || message.StartsWith("You rolled"))
            {
                _messageHistory.Clear();
                _isStatusMessage = true;
            }
            else if (_isStatusMessage && message.Contains("what would you like to do", StringComparison.OrdinalIgnoreCase))
            {
                _isStatusMessage = false;
            }

            lock (_lock)
            {
                _messageHistory.Enqueue((message, color));
                ShowMessageHistory();
            }
        }

        public static void AddMessage(string message, ConsoleColor color = ConsoleColor.White)
        {
            WriteMessage(message, color);
        }

        private static void ShowMessageHistory()
        {
            lock (_lock)
            {
                ClearMessageArea();
                int startLine = _mapHeight + 2;
                int currentLine = startLine;

                foreach (var (message, color) in _messageHistory)
                {
                    Console.SetCursorPosition(0, currentLine);
                    Console.ForegroundColor = color;
                    Console.WriteLine(message);
                    Console.ResetColor();
                    currentLine++;
                }

                Console.SetCursorPosition(0, currentLine);
            }
        }

        private static void ClearMessageArea()
        {
            lock (_lock)
            {
                int startLine = _mapHeight + 2;
                for (int i = startLine; i < Console.WindowHeight; i++)
                {
                    Console.SetCursorPosition(0, i);
                    Console.Write(new string(' ', Console.WindowWidth));
                }
            }
        }

        public static void ClearMessageHistory()
        {
            lock (_lock)
            {
                _messageHistory.Clear();
                ClearMessageArea();
            }
        }

        public static void SetMessageStartLine(int startLine)
        {
            lock (_lock)
            {
                _mapHeight = Math.Max(0, startLine - 2);
                ClearMessageArea();
            }
        }
    }
}