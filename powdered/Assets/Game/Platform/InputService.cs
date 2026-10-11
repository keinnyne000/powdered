using UnityEngine;
using Game.Core;
namespace Game.Platform
{
    public sealed class InputService : IInputSource
    {
        readonly InputSystem_Actions _actions = new();
        public InputService() => _actions.Player.Enable();
        
        public Vector2 Move => _actions.Player.Move.ReadValue<Vector2>();
        public Vector2 Look => _actions.Player.Look.ReadValue<Vector2>();
        public bool JumpPressed => _actions.Player.Jump.WasPressedThisFrame();
        public bool AttackHeld => _actions.Player.Attack.IsPressed();
    }
}
