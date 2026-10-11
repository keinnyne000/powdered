using UnityEngine;

namespace Game.Core.Systems
{
    public class PlayerSystem : ISimulationSystem
    {
        readonly GameState _state;
        readonly PlayerActions _actions;
        public PlayerSystem(GameState state)
        {
            _state = state;
            _actions = new();
            _actions.Enable();
            _state.sensX = 50;
            _state.sensY = -50;
            _state.xRotation = 0;
            _state.yRotation = 0;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        public void Tick(float deltaTime)
        {
            float mouseX = _actions.Default.Mouse.ReadValue<Vector2>().x * deltaTime * _state.sensX;
            float mouseY = _actions.Default.Mouse.ReadValue<Vector2>().y * deltaTime * _state.sensY;

            _state.yRotation += mouseX;
            _state.xRotation += mouseY;
            _state.xRotation = Mathf.Clamp(_state.xRotation, -90f, 90f);

            Camera.main.transform.rotation = Quaternion.Euler(_state.xRotation, _state.yRotation, 0);
        }
    }
}
