namespace Lab08.Items
{
    public class PowerSupply : IItem
    {
        public string Name => "Power Supply";
        public string Description => "A heavy nuclear unit used to power the MechSuit.";
        public ItemType Type => ItemType.Utility;
        public int Quantity { get; set; }
        public int Damage => 0;

        public void Use(Game game)
        {
            RoomType type = game.Map.GetRoomTypeAt(game.Player.Location);
            if (type == RoomType.MechBay)
            {
                DisplayStyle.WriteLine("You approach the dormant Mech Suit. It looms over you, silent and imposing.", ConsoleColor.Cyan);
                SpecialCommand activateMechSuit = new SpecialCommand("Insert Power Supply? (y/n)");
                SpecialCommand bossFight = new SpecialCommand("Activate boss fight? (y/n)");
                UseWithCommand(game, activateMechSuit, bossFight);
            }
        }
        
        public void UseWithCommand(Game game, SpecialCommand activateMechSuit, SpecialCommand bossFight)
        {
            RoomType type = game.Map.GetRoomTypeAt(game.Player.Location);
            if (type != RoomType.MechBay) 
                return;
                
            activateMechSuit.Execute(game);
            if (activateMechSuit.Choice)
            {
                DisplayStyle.WriteLine("The Mech Suit hums to life as you insert the Power Supply.", ConsoleColor.Cyan);
                game.Player.Inventory.RemoveStack(this);

                DisplayStyle.WriteLine("You suddenly hear a deep roar echoing from the sealed area in the bay. Something has heard you.", ConsoleColor.Red);
                bossFight.Execute(game);

                if (bossFight.Choice)
                {
                    game.StartBossFight();
                    game.TrackProgress(GameProgress.MechActivated);
                }
                else
                {
                    game.Player.BossDiscovered = true;
                    DisplayStyle.WriteLine("You decide it's time to run. Get to the airlock.", ConsoleColor.Cyan);
                }
            }
            else
            {
                DisplayStyle.WriteLine("You decide not to use the Power Supply right now.", ConsoleColor.Cyan);
            }
        }
    }
}