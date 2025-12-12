using System.Linq;
using Lab08.Displays;
namespace Lab08.Aliens
{
    public class Facehugger : Alien
    {
        private Random _random;
        
        public Facehugger(Location position) : base(position, "Facehugger") 
        { 
            _random = new Random();
        }

        public Facehugger(Location position, Random random) : base(position, "Facehugger")
        {
            _random = random ?? new Random();
        }

        public override void Activate(Game game)
        {
            if (!Location.Equals(game.Player.Location))
                return;

            Encounter(game, game.Player, game.Map, allowAliens: true);
        }

        public override void Act(Game game, Player player, Map map)
        {
            if (!Location.Equals(player.Location))
                return;

            Encounter(null, player, map, allowAliens: false);
        }

        private void Encounter(Game? game, Player player, Map map, bool allowAliens)
        {
            int possibleAction = _random.Next(2); // 0 = facehug, 1 = transport

            if (possibleAction == 0)
            {
                DisplayStyle.WriteLine("A horrible creature with splindly legs hisses at you angrily. Suddenly, it leaps right at your face.", ConsoleColor.Red);
                DisplayStyle.WriteLine("The facehugger lands on your face, wrapping its horrible appendage around your throat.", ConsoleColor.Red);
                DisplayStyle.WriteLine("Your vision blurs as you struggle to breathe. Consciousness fades.", ConsoleColor.Red);
                player.TakeDamage(10);
                DisplayStyle.WriteLine(string.Empty, ConsoleColor.White);
                DisplayStyle.WriteLine("You awaken later, disoriented and in pain, but alive.", ConsoleColor.Red);
                DisplayStyle.WriteLine("The odd creature lays dead beside you, its splindly legs twitching faintly.", ConsoleColor.Red);
                DisplayStyle.WriteLine("You feel a strange sensation in your chest, as if something is moving beneath your skin...", ConsoleColor.Red);
                Console.WriteLine();
                Kill("Fulfilled its purpose.");
                System.Threading.Thread.Sleep(1000);
                DisplayStyle.WriteLine("Press ENTER to continue", ConsoleColor.Red);
                Console.ReadLine();
                DisplayUI.ClearMessageHistory();
                DisplayStyle.WriteLine("---- TIME CONSTRAINT ----", ConsoleColor.Yellow);
                DisplayStyle.WriteLine("You must reach the MedBay.", ConsoleColor.Yellow);
                player.Infect(25);

                return;
            }
            else
            {
                DisplayStyle.WriteLine("A horrible creature with splindly legs hisses at you angrily. Suddenly, it leaps right at your face.", ConsoleColor.Red);
                DisplayStyle.WriteLine("As the creature lunges at you, you duck and sprint blindly through the corridors.", ConsoleColor.Cyan);
                DisplayStyle.WriteLine("After a dizzying series of turns, you're no longer sure where you are, or if you've even been here before. You consult your map.", ConsoleColor.Cyan);

                Location newPlayerLocation;
                Location startLocation = player.Location;
                int manhattanDistance;
                int attempts = 0;
                const int maxAttempts = 100; 

                do
                {
                    newPlayerLocation = startLocation;
                    int targetDistance = _random.Next(2, 7); 

                    for (int i = 0; i < targetDistance; i++)
                    {
                        int dr = newPlayerLocation.Row - startLocation.Row;
                        int dc = newPlayerLocation.Column - startLocation.Column;
                        
                        Direction dir;
                        if (Math.Abs(dr) + Math.Abs(dc) < 2 || _random.Next(2) == 0)
                        {
                            if (Math.Abs(dr) < Math.Abs(dc))
                                dir = dr < 0 ? Direction.South : Direction.North;
                            else
                                dir = dc < 0 ? Direction.East : Direction.West;
                        }
                        else
                        {
                            dir = DirectionHelper.GetRandomDirection(allowDiagonals: false);
                        }

                        Location test = newPlayerLocation.Move(dir);
                        if (map.IsWithinBounds(test))
                        {
                            newPlayerLocation = test;
                        }
                    }

                    manhattanDistance = Math.Abs(newPlayerLocation.Row - startLocation.Row) + 
                                      Math.Abs(newPlayerLocation.Column - startLocation.Column);
                    
                    attempts++;
                } while (manhattanDistance < 2 && attempts < maxAttempts);

                if (manhattanDistance < 2)
                {
                    newPlayerLocation = startLocation;
                    Direction backupDir = _random.Next(2) == 0 ? Direction.North : Direction.East;
                    for (int i = 0; i < 2; i++)
                    {
                        Location test = newPlayerLocation.Move(backupDir);
                        if (map.IsWithinBounds(test))
                            newPlayerLocation = test;
                        else
                            break;
                    }
                }

                RoomType landedRoom = map.GetRoomTypeAt(newPlayerLocation);
                bool wasDiscovered = map.IsDiscovered(newPlayerLocation);
                switch (landedRoom)
                {
                    case RoomType.MechBay:
                        Console.WriteLine();
                        DisplayStyle.WriteLine("You land in the MechBay.", ConsoleColor.Green);
                        Console.WriteLine();
                        break;
                    case RoomType.Airlock:
                        Console.WriteLine();
                        DisplayStyle.WriteLine("You were transported to the airlock. Finally, something you can recognize.", ConsoleColor.Green);
                        Console.WriteLine();
                        break;
                    case RoomType.Pit:
                        Console.WriteLine();
                        DisplayStyle.WriteLine("You rush blindly into a room. Suddenly, your foot misses the floor.", ConsoleColor.Red);
                        DisplayStyle.WriteLine("You fall straight into one of the holes in the floor, wrenching your ankle.", ConsoleColor.Red);
                        if (!wasDiscovered)
                        {
                            player.TakeDamage(5);
                        }
                        Console.WriteLine();
                        break;
                    case RoomType.MedBay:
                        Console.WriteLine();
                        DisplayStyle.WriteLine("You stumble into the MedBay, disoriented but safe.", ConsoleColor.Green);
                        Console.WriteLine();
                        break;
                }

                player.Location = newPlayerLocation;
                map.DiscoverRoom(newPlayerLocation);
                Console.WriteLine();
                DisplayStyle.WriteLine("You finally stop running. The sudden silence is deafening.", ConsoleColor.Green);
                Console.WriteLine();
                if (game != null)
                    MoveRandomly(game);

            }
        }
        
    public override void MoveTowardsPlayer(Game game)
        {
            var player = game.Player;
            int dr = Math.Sign(player.Location.Row - Location.Row);
            int dc = Math.Sign(player.Location.Column - Location.Column);

            Location candidate = new Location(Location.Row + dr, Location.Column + dc);

            if (!game.Map.IsWithinBounds(candidate) || IsLocationOccupied(game, candidate) || candidate.Equals(player.Location))
            {
                MoveRandomly(game);
                return;
            }

            DisplayMap.ClearMonsterMark(Location);
            Location = candidate;
        }

        public override void MoveRandomly(Game game)
        {
            Direction possibleDirection = DirectionHelper.GetRandomDirection(allowDiagonals: true);
            Location candidate = Location.Move(possibleDirection);
            if (game.Map.IsWithinBounds(candidate) && !IsLocationOccupied(game, candidate) && !candidate.Equals(game.Player.Location))
            {
                DisplayMap.ClearMonsterMark(Location);
                Location = candidate;
            }
        }
    }
}