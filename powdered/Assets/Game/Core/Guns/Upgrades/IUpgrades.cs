using UnityEngine;

namespace Game.Core.Guns.Upgrades
{
    public interface IUpgrades
    {
        // a one time run of the upgrade's unique effect.
        // will usually set a bool to true
        void effect(GameState state);
    }
}
