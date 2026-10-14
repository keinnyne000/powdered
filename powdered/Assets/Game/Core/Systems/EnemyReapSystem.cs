using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game.Core.Systems
{
    /// <summary>
    /// Responsible for cleaning up dead enemies
    /// </summary>
    public class EnemyReapSystem : ISimulationSystem
    {
        readonly GameState _state;
        public EnemyReapSystem(GameState state)
        {
            _state = state;
        }
        
        public void Tick(float deltaTime)
        {
            while (_state.DeadEnemies.Count > 0)
            {
                var enemy = _state.DeadEnemies.Pop();
                _state.Enemies[enemy.type].Remove(enemy);
                Debug.Log($"Destroying: {enemy}");
                Object.Destroy(enemy.gameObject);
            }
        }
    }

    public static class HashSetUtils
    {
        public static T Pop<T>(this HashSet<T> set)
        {
            using var e = set.GetEnumerator();
            if (!e.MoveNext()) throw new InvalidOperationException("Set is empty");
            T item = e.Current;
            set.Remove(item);
            return item;
        }
    }
}