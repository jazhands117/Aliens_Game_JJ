namespace Lab08.Items
{
    public class WoodenBat : IItem
    {
        public string Name => "Wooden Bat";
        public string Description => "A sturdy wooden bat. Deals light damage.";
        public ItemType Type => ItemType.Weapon;
        public int Quantity { get; set; }
        public int Damage => 3;

        public void Use(Game game)
        {
            if (game.CurrentAlien != null && game.CurrentAlien.IsAlive)
            {
                DisplayStyle.WriteLine("You swing the wooden bat at the alien!", ConsoleColor.Yellow);
                game.Player.DealDamage(game.CurrentAlien, this);
            }
            else
            {
                DisplayStyle.WriteLine("There's nothing to attack here.", ConsoleColor.Gray);
            }
        }
    }
}