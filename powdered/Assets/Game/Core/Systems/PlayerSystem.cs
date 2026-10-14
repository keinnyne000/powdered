using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Core.Systems
{
    public class PlayerSystem : ISimulationSystem
    {
        readonly GameState _state;
        readonly PlayerActions _actions;
        private GameObject _player = new GameObject("player");
        private CharacterController characterController;
        public PlayerSystem(GameState state)
        {
            _state = state;
            _actions = new();
            _actions.Enable();
            _actions.Default.Jump.started += Jumped;
            characterController = _player.AddComponent<CharacterController>();
            characterController.center = new Vector3(0,1,0);

            //Camera
            _state.sensX = 50;
            _state.sensY = -50;
            _state.xRotation = 0;
            _state.yRotation = 0;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            //Movement
            _state.playerSpeed = 5.5f;
            _state.playerJumpForce = 8.0f;
            _state.playerGravity = 20.0f;
            _state.moveDirection = Vector3.zero;
            _state.jumped = false;

            //Knockback
            _state.knockbackVelocity = Vector3.zero;
            _state.knockbackDrag = 10.0f;
        }

        public void Tick(float deltaTime)
        {
            //Camera
            float mouseX = _actions.Default.Mouse.ReadValue<Vector2>().x * deltaTime * _state.sensX;
            float mouseY = _actions.Default.Mouse.ReadValue<Vector2>().y * deltaTime * _state.sensY;

            _state.yRotation += mouseX;
            _state.xRotation += mouseY;
            _state.xRotation = Mathf.Clamp(_state.xRotation, -90f, 90f);

            Camera.main.transform.rotation = Quaternion.Euler(_state.xRotation, _state.yRotation, 0);
            _player.transform.rotation = Quaternion.Euler(0, _state.yRotation, 0);

            //Movement
            Vector2 moveVector = _actions.Default.Move.ReadValue<Vector2>();

            if (!characterController.isGrounded)
            {
                _state.jumped = false;
            }

            Vector3 forward = _player.transform.TransformDirection(Vector3.forward);
            Vector3 right = _player.transform.TransformDirection(Vector3.right);

            float oldY = _state.moveDirection.y;

            Vector2 newSpeed = new Vector2(moveVector.y * _state.playerSpeed, moveVector.x * _state.playerSpeed);

            _state.moveDirection = (forward * newSpeed.x) + (right * newSpeed.y);
            if (_state.jumped && characterController.isGrounded)
                _state.moveDirection.y += _state.playerJumpForce;
            else
                _state.moveDirection.y = oldY;

            if (!characterController.isGrounded)
            {
                
                _state.moveDirection.y -= _state.playerGravity * deltaTime;
            }

            characterController.Move((_state.moveDirection + _state.knockbackVelocity) * deltaTime);
            _state.isGrounded = characterController.isGrounded;
            _state.knockbackVelocity = Vector3.MoveTowards(_state.knockbackVelocity, Vector3.zero, _state.knockbackDrag * deltaTime);
            Camera.main.transform.position = _player.transform.position + new Vector3(0,1,0);
        }

        private void Jumped(InputAction.CallbackContext _)
        {
            _state.jumped = true;
        }
    }
}
