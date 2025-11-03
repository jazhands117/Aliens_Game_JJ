namespace Lab08.Displays
{
    public static class DisplayStyle
    {
        public static void WriteLine(string message, ConsoleColor color)
        {
            // buffer messages so the map can be printed first, then messages will be flushed
            DisplayUI.AddMessage(message, color);
        }
    }
}