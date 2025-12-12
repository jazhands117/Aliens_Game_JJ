using Lab08.Displays;

namespace Lab08.Aliens
{
    public class Xenomorph : Alien
    {
        private static readonly Random random = new Random();
        private int _health;
        public int Health => _health;

        public Xenomorph(Location position) : base(position, "Xenomorph")
        {
            _health = 20;
            IsAlive = true;
        }

        public override void Act(Game game, Player player, Map map)
        {
            if (Location.Equals(player.Location) && IsAlive)
            {
                game.CurrentAlien = this;

                DisplayStyle.WriteLine("Out of the shadows, the Xenomorph attacks!", ConsoleColor.Red);
                DisplayStyle.WriteLine("It hisses at you, its razor-sharp tail whipping through the air.", ConsoleColor.Red);

                DisplayStyle.WriteLine("Press SPACE to attack the Xenomorph!", ConsoleColor.Yellow);
                ConsoleKeyInfo keyInfo;
                do
                {
                    keyInfo = Console.ReadKey(true);
                } while (keyInfo.Key != ConsoleKey.Spacebar);

                player.DealDamage(this, player.EquippedWeapon);

                if (IsAlive)
                {
                    DisplayStyle.WriteLine("The Xenomorph retaliates, burying its horrible teeth into your flesh.", ConsoleColor.Red);
                    DealDamage(player);
                    ScurryAway(map);
                }
                else
                {
                    DisplayStyle.WriteLine("The Xenomorph collapses in a heap of smoke and sizzling acid.", ConsoleColor.Green);
                    DisplayStyle.WriteLine("Its lifeless body twitches one final time before going still.", ConsoleColor.Green);
                    System.Threading.Thread.Sleep(2000);
                    DisplayStyle.WriteLine("Your relief is short-lived, as the sizzling acid begins to eat through the deck beneath it.", ConsoleColor.Yellow);
                    DisplayStyle.WriteLine("You quickly step back to avoid falling into the growing hole.", ConsoleColor.Yellow);
                    map.SetRoomType(Location, RoomType.Pit);
                    map.DiscoverRoom(Location);
                }

                game.CurrentAlien = null;
            }
        }
        
        public override void TakeDamage(int amount)
        {
            _health -= amount;
            if (_health <= 0)
            {
                _health = 0;
                IsAlive = false;
                DisplayMap.ClearMonsterMark(Location);
            }
            else
            {
                DisplayStyle.WriteLine($"The alien screeches, recoiling with {_health} health remaining.", ConsoleColor.Yellow);
            }
        }

        public override void MoveTowardsPlayer(Game game)
        {
            int dr = Math.Sign(game.Player.Location.Row - Location.Row);
            int dc = Math.Sign(game.Player.Location.Column - Location.Column);

            Location candidate;
            int rowDiff = Math.Abs(game.Player.Location.Row - Location.Row);
            int colDiff = Math.Abs(game.Player.Location.Column - Location.Column);
            if (rowDiff >= colDiff)
            {
                candidate = new Location(Location.Row + dr, Location.Column);
            }
            else
            {
                candidate = new Location(Location.Row, Location.Column + dc);
            }

            if (!game.Map.IsWithinBounds(candidate) || IsLocationOccupied(game, candidate) || candidate.Equals(game.Player.Location))
            {
                MoveRandomly(game);
                return;
            }

            DisplayMap.ClearMonsterMark(Location);
            Location = candidate;
            DisplayStyle.WriteLine("Something in the depths of the ship heard you.", ConsoleColor.Yellow);
        }

        public override void MoveRandomly(Game game)
        {
            Location[] neighbors = game.Map.GetCardinalAdjacentRooms(Location);
            var valid = neighbors.Where(n => game.Map.IsWithinBounds(n) && !IsLocationOccupied(game, n) && !n.Equals(game.Player.Location)).ToArray();
            if (valid.Length > 0)
            {
                Location = valid[random.Next(valid.Length)];
            }
            DisplayMap.ClearMonsterMark(Location);
        }

        private void ScurryAway(Map map)
        {
            Location[] possibleLocations = map.GetCardinalAdjacentRooms(Location);
            List<Location> validLocations = new();
            foreach (var loc in possibleLocations)
            {
                if (map.IsWithinBounds(loc))
                {
                    validLocations.Add(loc);
                }
            }
            if (validLocations.Count > 0)
            {
                var random = new Random();
                Location newLocation = validLocations[random.Next(validLocations.Count)];
                Location = newLocation;
            }
            DisplayMap.ClearMonsterMark(Location);
            DisplayStyle.WriteLine("The Xenomorph scurries away into the shadows.", ConsoleColor.Yellow);
        }
    }
}