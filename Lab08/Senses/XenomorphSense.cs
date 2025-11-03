namespace Lab08.Senses
{
    public class XenomorphSense : ISense
    {
        public bool Detect(Game game)
        {
            foreach (Alien alien in game.Aliens)
            {
                if (alien is Xenomorph && alien.IsAlive)
                {
                    if (game.Player.Location.IsCardinallyAdjacent(alien.Location))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public void Notify(Game game)
        {
            DisplayStyle.WriteLine("You hear a low hiss. There is something very close.", ConsoleColor.Cyan);
        }
    }
}