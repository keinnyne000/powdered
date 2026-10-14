using UnityEngine;

namespace Game.Core
{
    public interface IInputSource
    {
    Vector2 Move { get; }
    Vector2 Look { get; }
    bool JumpPressed { get; }
    bool AttackHeld { get; }
    bool AttackPressed { get; }

    }
}
