using UnityEngine;

namespace Game.Core.Guns.Upgrades
{
    public class IncreaseDamage : IUpgrades
    {
        public void effect(GameState state)
        {
            state.damageUp += 1;
        }
    }
}
