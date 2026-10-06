namespace Game.Core
{
    public interface ICommand { }
    
    /* EXAMPLE:
    public sealed class TakeDamage : ICommand
    {
        public readonly int Amount;
        public TakeDamage(int amount) => Amount = amount;
     }
     */
}