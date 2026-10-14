using System;
using System.Collections.Generic;

namespace Game.Core
{
    public sealed class Observable<T> where T : struct
    {
        static readonly EqualityComparer<T> Comparer = EqualityComparer<T>.Default;
        private T _value;
        private readonly List<Action<T>> _listeners = new();

        public Observable(T value = default) => _value = value;
        
        public T Value
        {
            get => _value;
            internal set
            {
                if(Comparer.Equals(value, _value)) return; // dont invoke event if _value = value;
                _value = value;
                Invoke();
            }
        }

        public void Subscribe(Action<T> listener) => _listeners.Add(listener);

        public void Unsubscribe(Action<T> listener) => _listeners.Remove(listener);

        private void Invoke()
        {
            foreach (var action in _listeners)
                action.Invoke(_value);
        }
    }
}