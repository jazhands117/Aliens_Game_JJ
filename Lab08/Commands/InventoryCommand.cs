namespace Lab08.Commands
{
    public class InventoryCommand : ICommand
    {
        public void Execute(Game game)
        {
            DisplayUI.ClearMessageHistory();
            var items = game.Player.Inventory.Items;
            if (!items.Any())
            {
                DisplayStyle.WriteLine("Your inventory is empty.", ConsoleColor.Magenta);
                // draw map immediately so the buffered message is visible
                DisplayUI.DrawMap(game);
                return;
            }
            DisplayStyle.WriteLine("INVENTORY:", ConsoleColor.Magenta);
            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];
                DisplayStyle.WriteLine($"{i + 1}. {item.Name} (x{item.Quantity}) - {item.Description}", ConsoleColor.Magenta);
            }
            // flush the inventory listing so the player can see it before we read input
            DisplayStyle.WriteLine("Enter the number of an item to use it, or press ESC to exit:", ConsoleColor.White);
            DisplayUI.DrawMap(game);

            while (true)
            {
                var keyInfo = Console.ReadKey(true);
                if (keyInfo.Key == ConsoleKey.Escape)
                {
                    DisplayUI.ClearMessageHistory(); // Clear previous messages
                    return;
                }

                if (char.IsDigit(keyInfo.KeyChar))
                {
                    int choice = keyInfo.KeyChar - '0';
                    if (choice >= 1 && choice <= items.Count)
                    {
                        var selectedItem = items[choice - 1];
                        HandleItemUse(game, selectedItem);
                    }
                }
                else
                {
                    DisplayStyle.WriteLine("Invalid selection.", ConsoleColor.White);
                    DisplayUI.DrawMap(game);
                }
            }
        }

        private void HandleItemUse(Game game, IItem item)
        {
            switch (item.Name.ToLower())
            {
                case "bullets":
                    {
                        var confirm = new SpecialCommand("Would you like to roll a bullet? (y/n)");
                        confirm.Execute(game);
                        if (confirm.Choice)
                        {
                            if (item.Quantity > 0)
                            {
                                // calling Use will execute the roll which consumes a bullet from inventory
                                item.Use(game);
                            }
                            else
                            {
                                DisplayStyle.WriteLine("You have no bullets left to roll.", ConsoleColor.White);
                            }
                        }
                        break;
                    }
                case "charge node":
                case "charge nodes":
                    {
                        if (!game.Player.HasItem("Plasma Cutter"))
                        {
                            DisplayStyle.WriteLine("You need a Plasma Cutter to use Charge Nodes.", ConsoleColor.White);
                            return;
                        }
                        else
                        {
                            var confirm = new SpecialCommand("Reload Plasma Cutter? (y/n)");
                            confirm.Execute(game);
                            if (confirm.Choice)
                            {
                                DisplayStyle.WriteLine("You reload the Plasma Cutter.", ConsoleColor.White);
                            }
                        }
                        break;
                    }
                case "plasma cutter":
                    {
                        if (!game.Player.HasItem("Charge Nodes"))
                        {
                            DisplayStyle.WriteLine("You have no Charge Nodes to power the Plasma Cutter.", ConsoleColor.White);
                            return;
                        }
                        else
                        {
                            var confirm = new SpecialCommand("Equip Plasma Cutter? (y/n)");
                            confirm.Execute(game);
                            if (confirm.Choice)
                            {
                                game.Player.EquipWeapon(item);
                            }
                        }
                        break;
                    }
                case "power supply":
                    {
                        DisplayStyle.WriteLine("The Power Supply can only be used at the Mech Bay.", ConsoleColor.White);
                        break;
                    }
                case "machete":
                    {
                        var confirm = new SpecialCommand("Equip Machete? (y/n)");
                        confirm.Execute(game);
                        if (confirm.Choice)
                        {
                            game.Player.EquipWeapon(item);
                        }
                        break;
                    }
                case "wooden bat":
                    {
                        var confirm = new SpecialCommand("Equip Wooden Bat? (y/n)");
                        confirm.Execute(game);
                        if (confirm.Choice)
                        {
                            game.Player.EquipWeapon(item);
                        }
                        break;
                    }
                case "bandage":
                case "bandages":
                    {
                        var confirm = new SpecialCommand("Use Bandage to heal 20 HP? (y/n)");
                        confirm.Execute(game);
                        if (confirm.Choice)
                        {
                            // item.Use handles applying healing and consuming the bandage from inventory
                            item.Use(game);
                        }
                        break;
                    }
            }
        }
    }
}