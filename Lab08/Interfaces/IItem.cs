namespace Lab08.Interfaces
{
    public interface IItem
    {
        string Name { get; }
        string Description { get; }
        ItemType Type { get; }
        int Quantity { get; set; }
        int Damage { get; }

        void Use(Game game);
    }
}