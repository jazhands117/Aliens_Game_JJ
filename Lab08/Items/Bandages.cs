namespace Lab08.Items
{
    public class Bandages : IItem
    {
        public string Name => "Bandages";
        public string Description => "Fresh bandages. Can be applied to wounds.";
        public ItemType Type => ItemType.Healing;
        public int Quantity { get; set; }
        public int Damage => -20;

        public void Use(Game game)
        {
            int healed = game.Player.Heal(20);
            if (healed > 0)
            {
                game.Player.Inventory.RemoveItem(this, 1);
                DisplayStyle.WriteLine("You apply the bandage and feel slightly better.", ConsoleColor.Green);
            }
            else
            {
                DisplayStyle.WriteLine("You are already at full health. Bandage not used.", ConsoleColor.Yellow);
            }
        }
    }
}