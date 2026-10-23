using System;
using System.Collections.Generic;

namespace Game.Core.Guns
{
    public class Modifier
    {
        public object Key;
        private readonly List<Action<GameAction, GameState>> _handlers = new();

        public Modifier(object key) => Key = key;

        /// <summary>
        /// Registers a handler to run for game actions of type T
        /// </summary>
        /// <param name="handler">What you want done</param>
        /// <typeparam name="T">Which action should trigger this</typeparam>
        /// <returns></returns>
        public Modifier Register<T>(Action<T, GameState> handler) where T : GameAction
        {
            _handlers.Add((action, state) =>
            {
                if (action is T typed) handler(typed, state);
            });
            return this;
        }

        /// <summary>
        /// Triggers each delegate handler to run; should only be called from GameAction.cs
        /// </summary>
        /// <param name="action"></param>
        /// <param name="state"></param>
        public void Apply(GameAction action, GameState state)
        {
            foreach (var h in _handlers) h(action, state);
        }
    }
}
