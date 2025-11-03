namespace Lab08.Displays
{
    public static class DisplayStyle
    {
        public static void WriteLine(string message, ConsoleColor color)
        {
            DisplayUI.AddMessage(message, color);
        }
    }
}