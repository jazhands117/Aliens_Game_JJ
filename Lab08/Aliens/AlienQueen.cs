using Lab08.Displays;

namespace Lab08.Aliens
{
	public class AlienQueen : Alien
	{
		private int _health;

		public int Health => _health;

		public AlienQueen(Location position) : base(position, "Alien Queen")
		{
			_health = 200;
			IsAlive = true;
		}

		public override void Act(Game game, Player player, Map map)
		{
			// Boss controlled by Game.StartBossFight; Act not used for the scripted fight.
		}

		public override void TakeDamage(int amount)
		{
			if (!IsAlive)
				return;

			_health -= amount;
			if (_health <= 0)
			{
				_health = 0;
				IsAlive = false;
				DisplayStyle.WriteLine("The Alien Queen collapses with a terrible wail.", ConsoleColor.Green);
				DisplayMap.ClearMonsterMark(Location);
			}
			else
			{
				DisplayStyle.WriteLine($"The Alien Queen recoils. {_health} health remaining.", ConsoleColor.Yellow);
			}
		}

		// Queen's attack is a heavy hit (used by the boss thread)
		public void DealBossDamage(Player player)
		{
			if (!IsAlive)
				return;
			player.TakeDamage(20);
		}
	}
}

