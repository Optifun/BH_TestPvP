using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Arena.Character
{
    public class CharacterMovement : MonoBehaviour
    {
        [SerializeField] private PlayerInput _input;
        [SerializeField] private CharacterController _controller;
        [SerializeField] private Transform _shouldersTransform;
        [SerializeField] private Transform _model;
        [SerializeField] private float MaximumSpeed;

        private Vector3 _velocity;
        private InputAction _walkAction;

        private void Awake()
        {
            _walkAction = _input.actions["Walk"];
        }

        private void FixedUpdate()
        {
            var direction = _walkAction.ReadValue<Vector2>();
            var movementVelocity = MovementImpulse(direction);

            _velocity = Vector3.Slerp(_velocity, movementVelocity, 0.33f);

            if (_velocity != Vector3.zero)
                _model.rotation = Quaternion.Lerp(_model.rotation, Rotate(_velocity), 0.24f);

            _controller.Move((_velocity + Physics.gravity) * Time.fixedDeltaTime);
        }

        private Vector3 MovementImpulse(Vector2 direction)
        {
            if (direction != Vector2.zero)
            {
                var impulse = _shouldersTransform.right * direction.x + _shouldersTransform.forward * direction.y;
                return impulse.normalized * MaximumSpeed;
            }

            return Vector3.zero;
        }

        private static Quaternion Rotate(Vector3 movementDirection)
        {
            var lookRotation = Quaternion.LookRotation(movementDirection);
            var yAngle = lookRotation.eulerAngles.y;
            return Quaternion.Euler(0, yAngle, 0);
        }
    }
}