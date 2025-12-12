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
        public bool StoryFinished { get; set; } = false;
        public bool VisitedMechBay { get; private set; } = false;
        public bool IsBossFightActive { get; private set; } = false;
        public GameProgress Progress { get; private set; } = GameProgress.None;
        public Alien? CurrentAlien { get; set; }
        public bool AliensMovedThisTurn { get; set; } = false;
        public Dictionary<Location, List<IItem>> ItemsOnMap { get; } = new();

        public Game()
        {
            Map = new Map();
                Location randomStart = Map.GetRandomLocation();
            Map.SetStart(randomStart);
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
            Player.Inventory.AddItem(new Lab08.Items.Bullets { Quantity = 5 });
            Player.UpdateBulletsFromInventory();
        }

        public void Run()
        {
            while (!HasWon && Player.IsAlive)
            {
                DisplayStatus();
                DisplayUI.DrawMap(this);

                ICommand command = GetUserInput();
                command.Execute(this);

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
                AliensMovedThisTurn = false;
            }
        }

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
                    StoryFinished = true; 
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
            DisplayStyle.WriteLine("A low, guttural rumble echoes from behind the sealed bulkhead. The hull shivers as the massive doors open.", ConsoleColor.Cyan);
            DisplayStyle.WriteLine(" ", ConsoleColor.Black);
            DisplayStyle.WriteLine("From within the depths, a massive xenomorph emerges. The Queen hisses at you, second mouth coming forward menacingly.", ConsoleColor.Cyan);
            DisplayStyle.WriteLine("You grip the mech controls, raising its fists and reloading the hydrolics with a satisfying 'thunk'.", ConsoleColor.Cyan);
            DisplayStyle.WriteLine(" ", ConsoleColor.Black);
            DisplayStyle.WriteLine("Press ENTER to fight!", ConsoleColor.Yellow);
            Console.ReadLine();

            string queenAscii = "";
            try
            {
                string[] possiblePaths = new[]
                {
                    Path.Combine("Aliens", "Queen.txt"),
                    Path.Combine("..", "Lab08", "Aliens", "Queen.txt"),
                    Path.Combine("Lab08", "Aliens", "Queen.txt"),
                    Path.Combine("..", "..", "..", "..", "Lab08", "Aliens", "Queen.txt"),
                    Path.Combine("..", "..", "..", "Aliens", "Queen.txt"),
                };

                foreach (var path in possiblePaths)
                {
                    if (File.Exists(path))
                    {
                        queenAscii = File.ReadAllText(path);
                        break;
                    }
                }

                if (string.IsNullOrEmpty(queenAscii))
                {
                    queenAscii = "[Alien Queen]";
                }
            }
            catch
            {
                queenAscii = "[Alien Queen]";
            }

            Player.MarkBossDiscovered();
            IsBossFightActive = true;

            // snapshot player's health, inventory and equipped weapon so we can restore later
            // int previousHealth = Player.Health;
            // var inventorySnapshot = Player.Inventory.Items.Select(i => new { Type = i.GetType(), Name = i.Name, Quantity = i.Quantity }).ToList();
            // string? previousEquippedName = Player.EquippedWeapon?.Name;

            Player.SetHealth(250); // temporary mech health!
            var queen = new AlienQueen(Player.Location);

            DisplayUI.ClearMessageHistory();
            Console.Clear();
            var bossCts = new CancellationTokenSource();

            void DrawBossFightScreen(bool flashRed = false)
            {
                Console.SetCursorPosition(0, 0);
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(new string(' ', Console.BufferWidth * 10)); 
                
                Console.SetCursorPosition(0, 3);
                Console.ForegroundColor = flashRed ? ConsoleColor.Red : ConsoleColor.White;
                Console.WriteLine(queenAscii);
                Console.SetCursorPosition(0, Console.CursorTop + 1);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Press SPACE to attack!");
                Console.ResetColor();
            }

            DrawBossFightScreen();

            try
            {
                int asciiStartRow = 3;
                int asciiLines = queenAscii?.Split('\n').Length ?? 0;
                int messageStartLine = asciiStartRow + asciiLines + 2;
                DisplayUI.SetMessageStartLine(messageStartLine);
            }
            catch
            {
                // ignore if console operations fail in test environment
            }

            // boss attack thread: deals 20 damage every 1 second
            Thread bossThread = new Thread(() =>
            {
                while (!bossCts.Token.IsCancellationRequested && queen.IsAlive && Player.IsAlive)
                {
                    Thread.Sleep(1000);
                    lock (this)
                    {
                        if (!queen.IsAlive || !Player.IsAlive) break;
                        queen.DealBossDamage(Player);
                        DrawBossFightScreen();
                    }
                }
            });
            bossThread.IsBackground = true;
            bossThread.Start();

            // player input loop for attacking with SPACE
            while (queen.IsAlive && Player.IsAlive)
            {
                ConsoleKeyInfo keyInfo;
                try
                {
                    // wait for a key without blocking indefinitely so we can react to death
                    while (!Console.KeyAvailable)
                    {
                        if (!queen.IsAlive || !Player.IsAlive) break;
                        Thread.Sleep(50);
                    }

                    if (!queen.IsAlive || !Player.IsAlive) break;

                    keyInfo = Console.ReadKey(true);
                }
                catch
                {
                    if (!queen.IsAlive || !Player.IsAlive) break;
                    continue;
                }


                if (keyInfo.Key == ConsoleKey.Spacebar)
                {
                    lock (this)
                    {
                        queen.TakeDamage(25);
                        Player.AddDamageDealt(25);
                        DrawBossFightScreen(flashRed: true);
                    }

                    Thread.Sleep(100);

                    lock (this)
                    {
                        DrawBossFightScreen(flashRed: false);
                    }
                }
            }

            // fight finished: stop boss thread
            bossCts.Cancel();

            // var psItem = Player.Inventory.GetItemByName("Power Supply");
            // if (psItem != null)
            // {
            //     Player.Inventory.RemoveStack(psItem);
            // }

            if (!Player.IsAlive)
            {
                Console.Clear();
                DisplayStyle.WriteLine("The Alien Queen's dripping fangs closes over your mech. The second jaw shoots forward, piercing your chest.", ConsoleColor.Red);
                DisplayStyle.WriteLine($"Cause of death: {Player.CauseOfDeath}", ConsoleColor.Red);
                Console.WriteLine();
                DisplayStyle.WriteLine("Press ENTER to continue:", ConsoleColor.Yellow);
                Console.ReadLine();
                StatsScreen.DisplayStats(this);
                return;
            }

            if (!queen.IsAlive)
            {
            //     foreach (var it in Player.Inventory.Items.ToList())
            //     {
            //         Player.Inventory.RemoveStack(it);
            //     }
            //     foreach (var snap in inventorySnapshot)
            //     {
            //         if (snap.Name.Equals("Power Supply", StringComparison.OrdinalIgnoreCase))
            //             continue;

            //         try
            //         {
            //             var obj = Activator.CreateInstance(snap.Type);
            //             if (obj is IItem newItem)
            //             {
            //                 newItem.Quantity = snap.Quantity;
            //                 Player.Inventory.AddItem(newItem);
            //             }
            //         }
            //         catch
            //         {
            //             // if we can't recreate via reflection, skip restoring that item
            //         }
            //     }
            //     if (!string.IsNullOrEmpty(previousEquippedName))
            //     {
            //         var toEquip = Player.Inventory.GetItemByName(previousEquippedName);
            //         if (toEquip != null)
            //         {
            //             Player.EquipWeapon(toEquip);
            //         }
            //     }

            //     Player.SetHealth(previousHealth);
            //     Player.UpdateBulletsFromInventory();

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
            Progress |= progress;
        }
        public bool HasWon => VisitedMedbay && CurrentRoom == RoomType.Airlock;
        public RoomType CurrentRoom => Map.GetRoomTypeAt(Player.Location);
    }
}