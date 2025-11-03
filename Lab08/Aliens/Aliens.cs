namespace Lab08.Aliens
{
    public abstract class Alien
    {
        private static readonly Random _random = new Random();
        public Location Location { get; protected set; }
        public bool IsAlive { get; protected set; } = true;
        int Health { get; set; }
        public string Name { get; protected set; }

        public Alien(Location position, string name)
        {
            Location = position;
            Name = name;
        }

        public virtual void Activate(Game game)
        {
            Act(game, game.Player, game.Map);
        }

        public virtual void MoveRandomly(Game game)
        {
            // override in each alien motion cause they're different
        }

        public virtual void MoveTowardsPlayer(Game game)
        {
            // override in each alien motion cause they're different
        }

        public void Kill(string cause)
        {
            IsAlive = false;
        }

        public virtual void TakeDamage(int amount)
        {
            if (!IsAlive)
                return;
            
            Health -= amount;
            if (Health <= 0)
            {
                Kill("Alien defeated.");
            }
        }

        public void DealDamage(Player player)
        {
            if (!IsAlive)
                return;
            int damage = _random.Next(2, 11);
            player.TakeDamage(damage);
        }
        
        protected bool IsLocationOccupied(Game game, Location newLocation)
        {
            foreach (var alien in game.Aliens)
            {
                if (alien != this && alien.IsAlive && alien.Location.Equals(newLocation))
                {
                    return true;
                }
            }
            return false;
        }

        public abstract void Act(Game game, Player player, Map map);
    }
}