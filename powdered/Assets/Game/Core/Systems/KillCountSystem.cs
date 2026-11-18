using System.Collections.Generic;
using Game.Core.Enemies;

namespace Game.Core.Systems
{
    /// <summary>
    /// PROTOTYPE: counts enemies that land in GameState.DeadEnemies and pushes the total to GameState.KillCount.
    /// Must tick BEFORE EnemyReapSystem, or DeadEnemies will already be empty.
    /// </summary>
    public class KillCountSystem : ISimulationSystem
    {
        readonly GameState _state;

        // Enemies we counted last tick. Guards against double counting if an enemy
        // stays in DeadEnemies for more than one tick.
        readonly HashSet<EnemyObject> _countedLastTick = new();

        public KillCountSystem(GameState state)
        {
            _state = state;
        }

        public void Tick(float dt)
        {
            int newKills = 0;
            foreach (var dead in _state.DeadEnemies)
            {
                if (!_countedLastTick.Contains(dead))
                    newKills++;
            }

            _countedLastTick.Clear();
            _countedLastTick.UnionWith(_state.DeadEnemies);

            if (newKills > 0)
                _state.KillCount.Value += newKills;
        }
    }
}