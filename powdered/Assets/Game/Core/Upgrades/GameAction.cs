using UnityEngine;

namespace Game.Core.Guns
{
    public abstract class GameAction
    {
        public void Trigger(GameState state)
        {
            foreach (var modifier in state.Modifiers) modifier.Apply(this, state);
            Apply(state);
        }

        protected virtual void Apply(GameState state) { }
    }

    public class ShootGameAction : GameAction
    {
        public Vector3 Origin;
        public Vector3 Direction;
        public float BonusDamage;

        public ShootGameAction(Vector3 origin, Vector3 direction)
        {
            Origin = origin;
            Direction = direction;
            BonusDamage = 0f;
        }

        protected override void Apply(GameState state) => state.CurrentGun.Fire(this, state);
    }
}