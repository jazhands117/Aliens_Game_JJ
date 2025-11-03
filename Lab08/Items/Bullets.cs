namespace Lab08.Items
{
    public class Bullets : IItem
    {
        public string Name => "Bullets";
        public string Description => "Old ballistic rounds. Can be rolled to check nearby rooms.";
        public ItemType Type => ItemType.Utility;
        public int Quantity { get; set; }
        public int Damage => 0;

        public void Use(Game game)
        {
            var bulletCommand = new UseBulletCommand();
            bulletCommand.Execute(game);
        }
    }
}