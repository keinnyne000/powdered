using UnityEngine;

namespace Game.Core
{
    public interface ICommand { }

    public sealed class MoveInput : ICommand
    {
        public readonly Vector2 Direction;
        public MoveInput(Vector2 direction) => Direction = direction;
    }
    /* EXAMPLE:
    public sealed class TakeDamage : ICommand
    {
        public readonly int Amount;
        public TakeDamage(int amount) => Amount = amount;
     }
     */
}