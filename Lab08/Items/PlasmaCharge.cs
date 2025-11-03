namespace Lab08.Items
{
    public class PlasmaCharge : IItem
    {
        public string Name => "Charge Nodes";
        public string Description => "A power node used to energize the Plasma Cutter";
        public ItemType Type => ItemType.Ammo;
        public int Quantity { get; set; }
        public int Damage => 0;

        public void Use(Game game)
        {
            //placeholder, goes into the plasma cutter//
        }
    }
}