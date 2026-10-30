using System;
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
            _state.playerSpeed = Vector2.zero;
            _state.maxWalkSpeed = 5.5f;
            _state.playerAcceleration = 0.02f;
            _state.playerFriction = 0.02f;
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

            float currentSpeed = new Vector2(_state.moveDirection.z,_state.moveDirection.x).magnitude;
            
            if (!moveVector.Equals(Vector2.zero) && currentSpeed < _state.maxWalkSpeed)
            {
                Vector3 newMoveDirection = (forward * (moveVector / moveVector.magnitude).y) + (right * (moveVector / moveVector.magnitude).x);
                _state.moveDirection += newMoveDirection / newMoveDirection.magnitude * _state.playerAcceleration;
            }
            else if(currentSpeed != 0)
            {
                Vector3 frictionDirection = new Vector3(_state.moveDirection.x,0,_state.moveDirection.z);
                _state.moveDirection -= frictionDirection / frictionDirection.magnitude * _state.playerFriction;
            }

            // _state.playerSpeed.y += WalkingHelper(moveVector.y, _state.playerSpeed.y);
            // _state.playerSpeed.x += WalkingHelper(moveVector.x, _state.playerSpeed.x);

            // _state.moveDirection = (forward * _state.playerSpeed.y) + (right * _state.playerSpeed.x);

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
            Debug.Log(_state.moveDirection);
        }

        // private float WalkingHelper(float moveVector, float speed)
        // {
        //     if (moveVector != 0 && Math.Abs(speed) < _state.maxWalkSpeed * Math.Abs(moveVector))
        //     {
        //         if(Math.Abs(speed) > Math.Abs(speed + moveVector) && speed != 0)
        //         {
        //             return _state.playerAcceleration * moveVector + (-speed/Math.Abs(speed) * _state.playerFriction);
        //         }
        //         else
        //         {
        //             return _state.playerAcceleration * moveVector;
        //         }
        //     }
        //     else if(speed != 0)
        //     {
        //         return -speed / Math.Abs(speed) * _state.playerFriction;
        //     }
        //     return 0;
        // }

        private void Jumped(InputAction.CallbackContext _)
        {
            _state.jumped = true;
        }
    }
}
