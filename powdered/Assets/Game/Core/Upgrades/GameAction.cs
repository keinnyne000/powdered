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
        public Vector3 Direction;
        public Vector3 Origin;
        public float BonusDamage;
        // 1 = normal fire rate. 1.25 = 25% faster. Applied to the cooldown after the shot
        public float FireRateMultiplier;

        public ShootGameAction(Vector3 origin, Vector3 direction)
        {
            Direction = direction;
            Origin = origin;
            BonusDamage = 0f;
            FireRateMultiplier = 1f;
        }

        protected override void Apply(GameState state) => state.CurrentGun.Fire(this, state);
    }
}