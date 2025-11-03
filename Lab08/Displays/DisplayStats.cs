namespace Lab08.Displays
{
    public static class StatsScreen
    {
        public static void DisplayStats(Game game)
        {
            DisplayUI.ClearMessageHistory();
            DisplayStyle.WriteLine("====== MISSION STATISTICS ====== ", ConsoleColor.Yellow);
            Console.WriteLine();
            DisplayStyle.WriteLine($"   Total Damage Taken: {game.Player.TotalDamageTaken}", ConsoleColor.Cyan);
            DisplayStyle.WriteLine($"   Total Damage Dealt: {game.Player.TotalDamageDealt}", ConsoleColor.Cyan);

            double discoveredPercent = game.Map.CalculateDiscovered();
            DisplayStyle.WriteLine("   Ship Discovered: " + discoveredPercent.ToString("F1") + "%", ConsoleColor.Cyan);

            string bossStatus;
            if (game.Player.BossDiscovered)
            {
                bossStatus = "Yes";
            }
            else
            {
                bossStatus = "No";
            }
            DisplayStyle.WriteLine("   Boss Discovered: " + bossStatus, ConsoleColor.Cyan);

            string storyDiscovered;
            if (game.HasWon)
            {
                storyDiscovered = "Yes";
            }
            else
            {
                storyDiscovered = "No";
            }
            DisplayStyle.WriteLine("   Story Completed: " + storyDiscovered, ConsoleColor.Cyan);

            Console.WriteLine();
            DisplayStyle.WriteLine("================================", ConsoleColor.White);
            Console.WriteLine();
            System.Threading.Thread.Sleep(2000);
            DisplayStyle.WriteLine("Press ENTER to exit. Thanks for playing!", ConsoleColor.Blue);
            Console.ReadLine();

            //kills process//
            Environment.Exit(0);
        }
    }
}