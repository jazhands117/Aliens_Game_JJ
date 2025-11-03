namespace Lab08.Items
{
    public class Machete : IItem
    {
        public string Name => "Machete";
        public string Description => "A blunt, but heavy machete. Deals moderate damage.";
        public ItemType Type => ItemType.Weapon;
        public int Quantity { get; set; }
        public int Damage => 5;

        public void Use(Game game)
        {
            if (game.CurrentAlien != null && game.CurrentAlien.IsAlive)
            {
                game.Player.DealDamage(game.CurrentAlien, this);
            }
            else
            {
                DisplayStyle.WriteLine("There is nothing to use the Machete on here.", ConsoleColor.Yellow);
            }
        }
    }
}