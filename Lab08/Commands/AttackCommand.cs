namespace Lab08.Commands
{
    public class AttackCommand : ICommand
    {
        private readonly Alien _target;
        public AttackCommand(Alien target)
        {
            _target = target;
        }
        public void Execute(Game game)
        {
            if (!_target.IsAlive)
            {
                DisplayStyle.WriteLine($"The {_target.Name} is already dead.", ConsoleColor.Yellow);
                return;
            }
            game.Player.DealDamage(_target, game.Player.EquippedWeapon);
        }
    }
}