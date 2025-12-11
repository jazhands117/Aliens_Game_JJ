namespace Lab08.GameDesign
{
    public class Game
    {
        public Map Map {get;} 
        public Player Player {get;}
        public Alien[] Aliens {get;}
        private readonly ISense[] _senses;
        public IEnumerable<ISense> Senses => _senses;
        public bool VisitedMedbay { get; private set; } = false;
        public bool VisitedMechBay { get; private set; } = false;
        public bool IsBossFightActive { get; private set; } = false;
        public GameProgress Progress { get; private set; } = GameProgress.None;
        public Alien? CurrentAlien { get; set; }
        // flag to prevent double-moving aliens when an action already moved them (e.g., bullet roll)
        public bool AliensMovedThisTurn { get; set; } = false;
        public Dictionary<Location, List<IItem>> ItemsOnMap { get; } = new();

        public Game()
        {
            Map = new Map();
                Location randomStart = Map.GetRandomLocation();
            Map.SetStart(randomStart);
            // give the player 5 starting bullets by default
            Player = new Player(randomStart, initialBullets: 5);

            Location airlock = Map.GetRoomLocation(RoomType.Airlock);
            Location medbay = Map.GetRoomLocation(RoomType.MedBay);
            Aliens = new Alien[]
            {
                new Xenomorph(Map.GetRandomLocation()),
                new Xenomorph(Map.GetRandomLocation()),
                new Facehugger(Map.GetRandomLocation()),
                new Facehugger(Map.GetRandomNeighbor(airlock)),
                new Facehugger(Map.GetRandomNeighbor(medbay))
            };

            DisplayMap.InitializeMap(Map.Height, Map.Width);

            _senses = new ISense[]
            {
                new XenomorphSense(),
                new FacehuggerSense(),
                new MedbaySense(),
                new MechbaySense()
            };

            InitializeItems();

            // ensure player's inventory contains 5 bullets and sync the count
            Player.Inventory.AddItem(new Lab08.Items.Bullets { Quantity = 5 });
            Player.UpdateBulletsFromInventory();
        }

        public void Run()
        {
            while (!HasWon && Player.IsAlive)
            {
                // collect status/messages, then draw the map (map prints first, then messages)
                DisplayStatus();
                DisplayUI.DrawMap(this);

                ICommand command = GetUserInput();
                command.Execute(this);

                // Check all conditions and gather messages before redrawing
                CheckForAliens();
                CheckForRoomItems();
                CheckForMechBay();
                CheckForMedBay();
                Player.AdvanceInfection(Map);
                
                DisplayUI.DrawMap(this);
                Console.WriteLine(); 
                
            }
            if (HasWon)
            {
                Console.Clear();
                DisplayStyle.WriteLine("The mystery has been solved, and the crew's souls can finally be at peace.", ConsoleColor.Green);
                Console.WriteLine();
                DisplayStyle.WriteLine("YOU WIN! Congratulations!", ConsoleColor.Green);
                Console.WriteLine();
                DisplayStyle.WriteLine("Press ENTER to continue:", ConsoleColor.Green);
                Console.ReadLine();
                StatsScreen.DisplayStats(this);
            }
            else
            {
                Console.Clear();
                Console.WriteLine();
                DisplayStyle.WriteLine("YOU DIED. Game over.", ConsoleColor.Red);
                DisplayStyle.WriteLine($"Cause of death: {Player.CauseOfDeath}", ConsoleColor.Red);
                Console.WriteLine();
                DisplayStyle.WriteLine("Press ENTER to continue:", ConsoleColor.Yellow);
                Console.ReadLine();
                StatsScreen.DisplayStats(this);
            }
        }

        private void DisplayStatus()
        {
            DisplayStyle.WriteLine($"Health: {Player.Health}", ConsoleColor.White);
            foreach (ISense sense in _senses)
            {
                if (sense.Detect(this))
                {
                    sense.Notify(this);
                }
            }
        }
        
        private void InitializeItems()
        {
            var random = new Random();
            // had AI teach me how to do func :D
            void PlaceItem(IItem item, Func<Location> locationSelect)
            {
                Location loc = locationSelect();
                if (!ItemsOnMap.ContainsKey(loc))
                {
                    ItemsOnMap[loc] = new List<IItem>();
                }
                ItemsOnMap[loc].Add(item);
            }
            // bullets
            Location airlockLoc = Map.GetRoomLocation(RoomType.Airlock);
            // place first batch adjacent to airlock
            PlaceItem(new Bullets { Quantity = 5 }, () => Map.GetRandomNeighbor(airlockLoc));
            for (int i = 1; i < 3; i++)
            {
                PlaceItem(new Bullets { Quantity = 5 }, () => Map.GetRandomLocation());
            }
            // bandages
            for (int i = 0; i < 3; i++)
            {
                PlaceItem(new Bandages { Quantity = 1 }, () => Map.GetRandomLocation());
            }
            Location medBay = Map.GetRoomLocation(RoomType.MedBay);
            PlaceItem(new Bandages { Quantity = 3 }, () => medBay);

            // charge nodes
            for (int i = 0; i < 4; i++)
            {
                PlaceItem(new PlasmaCharge { Quantity = 2 }, () => Map.GetRandomLocation());
            }

            // weapons
            PlaceItem(new WoodenBat { Quantity = 1 }, () => Map.GetRandomLocation());
            PlaceItem(new Machete { Quantity = 1 }, () => Map.GetRandomLocation());
            PlaceItem(new PlasmaCutter { Quantity = 1 }, () => Map.GetRandomLocation());
            PlaceItem(new Lab08.Items.PowerSupply { Quantity = 1 }, () => Map.GetRandomLocation());
        }

        private ICommand GetUserInput(bool inCombat = false, Alien? targetAlien = null, bool canUseSpecial = false)
        {
            while (true)
            {
                if (inCombat && targetAlien != null)
                {
                    DisplayUI.WriteMessage($"Press SPACE to attack!", ConsoleColor.White);
                }
                else if (canUseSpecial)
                {
                    DisplayUI.WriteMessage($"Press Y to use the item, N to cancel.", ConsoleColor.White);
                }
                else
                {
                    DisplayUI.WriteMessage("What would you like to do?", ConsoleColor.White);
                }
                Console.ForegroundColor = ConsoleColor.Yellow;

                ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                ConsoleKey key = keyInfo.Key;

                if (inCombat && key == ConsoleKey.Spacebar && targetAlien != null)
                {
                    return new AttackCommand(targetAlien);
                }
                if (key == ConsoleKey.W)
                    return new MoveCommand(Direction.North);
                if (key == ConsoleKey.S)
                    return new MoveCommand(Direction.South);
                if (key == ConsoleKey.D)
                    return new MoveCommand(Direction.East);
                if (key == ConsoleKey.A)
                    return new MoveCommand(Direction.West);
                if (key == ConsoleKey.R)
                    return new UseBulletCommand();
                if (key == ConsoleKey.I)
                    return new InventoryCommand();
                if (key == ConsoleKey.H)
                    return new HelpCommand();

                DisplayUI.WriteMessage($"Not a valid command. Type 'h' for list of commands.", ConsoleColor.Magenta);
                DisplayUI.DrawMap(this);
            }
        }

        private void CheckForAliens()
        {
            foreach (Alien alien in Aliens)
            {
                if (alien.Location == Player.Location && alien.IsAlive)
                {
                    alien.Act(this, Player, Map);
                }
            }
            // move aliens unless they were already moved by an action (e.g., bullet roll)
            if (!AliensMovedThisTurn)
            {
                foreach (Alien alien in Aliens)
                {
                    if (!alien.IsAlive)
                        continue;
                    if (alien is Facehugger facehugger)
                    {
                        facehugger.MoveRandomly(this);
                    }
                    else if (alien is Xenomorph xenomorph)
                    {
                        xenomorph.MoveRandomly(this);
                    }
                }
            }
            else
            {
                // reset the flag for the next turn
                AliensMovedThisTurn = false;
            }
        }

        // had LOTS of help here
        private void CheckForRoomItems()
        {
            if (ItemsOnMap.TryGetValue(Player.Location, out List<IItem>? itemsInRoom))
            {
                foreach (var item in itemsInRoom.ToList())
                {
                    string itemName;
                    if (item.Quantity > 1)
                    {
                        itemName = item.Quantity + " " + item.Name;
                    }
                    else
                    {
                        itemName = item.Name;
                    }

                    var prompt = new SpecialCommand("You find " + itemName + ". Add to your inventory? (y/n)");
                    prompt.Execute(this);
                    if (prompt.Choice)
                    {
                        Player.Inventory.AddItem(item);
                        // if bullets were picked up, sync the player's bullets count
                        if (item.Name.Equals("Bullets", StringComparison.OrdinalIgnoreCase))
                        {
                            Player.UpdateBulletsFromInventory();
                        }
                        DisplayStyle.WriteLine("You picked up " + itemName + ".", ConsoleColor.Cyan);
                    }
                    else
                    {
                        DisplayStyle.WriteLine("You decide to leave the " + itemName + ".", ConsoleColor.Cyan);
                    }
                }
                ItemsOnMap.Remove(Player.Location);
            }
        }

        private void CheckForMedBay()
        {
            RoomType type = Map.GetRoomTypeAt(Player.Location);
            if (type == RoomType.MedBay)
            {
                DisplayUI.ClearMessageHistory();
                if (!VisitedMedbay)
                {
                    VisitedMedbay = true;
                    DisplayStyle.WriteLine("You have reached the MedBay.", ConsoleColor.Green);
                    MedbayStory.DisplayMedbayStory();
                }
                else
                {
                    DisplayStyle.WriteLine("You've returned to the Med Bay. The bodies are still.", ConsoleColor.Cyan);
                }
            }
        }

        private void CheckForMechBay()
        {
            RoomType type = Map.GetRoomTypeAt(Player.Location);
            if (type == RoomType.MechBay)
            {
                DisplayUI.ClearMessageHistory();
                if (!VisitedMechBay)
                    VisitedMechBay = true;
                var powerSupply = Player.Inventory.Items.FirstOrDefault(item => item.Name == "Power Supply");
                if (powerSupply != null)
                {
                    DisplayStyle.WriteLine("The Power Supply you found will power the Mech Suit in this room.", ConsoleColor.Cyan);
                    powerSupply.Use(this);
                }
                else
                {
                    DisplayStyle.WriteLine("The Mech Suit stands dormant. You need a Power Supply to activate it.", ConsoleColor.Cyan);
                }
            }
        }

        public void StartBossFight()
        {
            DisplayUI.ClearMessageHistory();
            DisplayStyle.WriteLine("You climb shakily into the Mech Suit. The controls feel intuitive as you take command of the powerful machine.", ConsoleColor.Cyan);
            DisplayStyle.WriteLine("The ship trembles and groans as something inside the sealed area begins to pound on the walls.", ConsoleColor.Cyan);
            DisplayStyle.WriteLine("Your finger hovers over the button to breach the sealed area. This is it.", ConsoleColor.Cyan);
            Console.WriteLine();
            System.Threading.Thread.Sleep(1000);
            DisplayStyle.WriteLine("Press ENTER to continue", ConsoleColor.Yellow);
            Console.ReadLine();
            // Begin the actual boss sequence narrative
            DisplayUI.ClearMessageHistory();
            DisplayStyle.WriteLine("A low, guttural rumble echoes from behind the sealed bulkhead. The hull shivers.", ConsoleColor.Yellow);
            DisplayStyle.WriteLine("You hear the skittering of many legs and a sound like a hundred chests inhaling at once.", ConsoleColor.Yellow);
            DisplayStyle.WriteLine("Something massive moves within. The Queen is awake.", ConsoleColor.Yellow);
            Console.WriteLine();
            DisplayStyle.WriteLine("Press ENTER to continue", ConsoleColor.Yellow);
            Console.ReadLine();

            // Load the ASCII art for the queen
            string queenAscii = "";
            try
            {
                queenAscii = File.ReadAllText(Path.Combine("Aliens", "Queen.txt"));
            }
            catch
            {
                queenAscii = "[Alien Queen]";
            }

            // Prepare boss and player stats for the fight
            Player.MarkBossDiscovered();
            IsBossFightActive = true;
            Player.SetHealth(250); // temporary boss-fight health
            var queen = new AlienQueen(Player.Location);

            // Print the queen art and prompt
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(queenAscii);
            DisplayStyle.WriteLine("Press SPACE to attack!", ConsoleColor.Yellow);

            // Cancellation token to stop boss thread when fight ends
            var bossCts = new CancellationTokenSource();

            // Boss attack thread: deals 20 damage every 1 second
            Thread bossThread = new Thread(() =>
            {
                while (!bossCts.Token.IsCancellationRequested && queen.IsAlive && Player.IsAlive)
                {
                    Thread.Sleep(1000);
                    lock (this)
                    {
                        if (!queen.IsAlive || !Player.IsAlive) break;
                        queen.DealBossDamage(Player);
                    }
                }
            });
            bossThread.IsBackground = true;
            bossThread.Start();

            // Player input loop for attacking with SPACE
            while (queen.IsAlive && Player.IsAlive)
            {
                var keyInfo = Console.ReadKey(true);
                if (keyInfo.Key == ConsoleKey.Spacebar)
                {
                    // Player attacks: deal 25 damage to queen
                    lock (this)
                    {
                        queen.TakeDamage(25);
                    }

                    // flash the ASCII art red for 0.1s
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(queenAscii);
                    Thread.Sleep(100);
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(queenAscii);
                    DisplayStyle.WriteLine("Press SPACE to attack!", ConsoleColor.Yellow);
                }
            }

            // Fight finished: stop boss thread
            bossCts.Cancel();

            if (!Player.IsAlive)
            {
                Console.Clear();
                DisplayStyle.WriteLine("The Alien Queen's maw closes over you. Darkness.", ConsoleColor.Red);
                DisplayStyle.WriteLine($"Cause of death: {Player.CauseOfDeath}", ConsoleColor.Red);
                Console.WriteLine();
                DisplayStyle.WriteLine("Press ENTER to continue:", ConsoleColor.Yellow);
                Console.ReadLine();
                StatsScreen.DisplayStats(this);
                return;
            }

            if (!queen.IsAlive)
            {
                Console.Clear();
                DisplayStyle.WriteLine("You have slain the Alien Queen! The ship goes quiet.", ConsoleColor.Green);
                Console.WriteLine();
                DisplayStyle.WriteLine("Press ENTER to continue:", ConsoleColor.Green);
                Console.ReadLine();
                StatsScreen.DisplayStats(this);
                return;
            }
        }

        public void TrackProgress(GameProgress progress)
        {
            Progress |= progress; // AI helped here
        }
        public bool HasWon => VisitedMedbay && CurrentRoom == RoomType.Airlock;
        public RoomType CurrentRoom => Map.GetRoomTypeAt(Player.Location);
    }
}