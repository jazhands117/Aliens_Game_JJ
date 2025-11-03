namespace Lab08.Senses
{
    public class FacehuggerSense : ISense
    {
        public bool Detect(Game game)
        {
            foreach (Alien alien in game.Aliens)
            {
                if (alien is Facehugger && alien.IsAlive)
                {
                    if (game.Player.Location.IsAdjacent(alien.Location))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public void Notify(Game game)
        {
            DisplayStyle.WriteLine("You hear scuttling in the walls. Be cautious.", ConsoleColor.Cyan);
        }
    }
}