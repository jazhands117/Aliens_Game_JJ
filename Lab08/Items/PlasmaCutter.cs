namespace Lab08.Items
{
    public class PlasmaCutter : IItem
    {
        public string Name => "Plasma Cutter";
        public string Description => "A high-energy cutting tool. Extremely effective. Uses charge nodes.";
        public ItemType Type => ItemType.Weapon;
        public int Quantity { get; set; }
        public int Damage => 10;

        public void Use(Game game)
        {
            // Get Charge Nodes from inventory
            var chargeNodes = game.Player.Inventory.GetItemByName("Charge Nodes");
            // Validate that we have Charge Nodes before trying to use them
            if (chargeNodes == null || chargeNodes.Quantity <= 0)
            {
                DisplayStyle.WriteLine("You have no Charge Nodes to power the Plasma Cutter!", ConsoleColor.Yellow);
                return;
            }

            // Only allow attack when there's a live alien present
            if (game.CurrentAlien != null && game.CurrentAlien.IsAlive)
            {
                DisplayStyle.WriteLine("You fire the Plasma Cutter at the alien!", ConsoleColor.Yellow);
                game.Player.DealDamage(game.CurrentAlien, this);
                
                // Remove one charge node after the attack; we know chargeNodes is not null here
                game.Player.Inventory.RemoveItem(chargeNodes, 1);
            }
            else
            {
                DisplayStyle.WriteLine("There's nothing to attack here.", ConsoleColor.Gray);
            }
        }
    }
}