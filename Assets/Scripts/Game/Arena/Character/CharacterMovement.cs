using System;
using UnityEngine;

namespace Game.Arena.Character
{
    public class CharacterMovement : MonoBehaviour
    {
        [SerializeField] private CharacterController _controller;
        [SerializeField] private Transform _shouldersTransform;
        [SerializeField] private float MaximumSpeed;

        public Vector3 Movement { get; private set; }
        private Vector3 _velocity;
        private Vector3 _impulse;

        public void Move(Vector2 input)
        {
            Movement = CorrectDirection(input);
        }

        private void FixedUpdate()
        {
            _velocity = Vector3.Slerp(_velocity, ApplySpeed(Movement), 0.33f);
            _controller.Move((_velocity + _impulse + Physics.gravity) * Time.fixedDeltaTime);
        }

        private void Update()
        {
            _impulse = Vector3.Slerp(_impulse, Vector3.zero, 0.1f * Time.deltaTime);
        }

        private Vector3 ApplySpeed(Vector3 direction) =>
            direction * MaximumSpeed;

        private Vector3 CorrectDirection(Vector2 direction)
        {
            if (direction == Vector2.zero) return Vector3.zero;
            var relativeDirection = _shouldersTransform.right * direction.x + _shouldersTransform.forward * direction.y;
            return relativeDirection.normalized;
        }

        public void SetImpulse(Vector3 impulse) =>
            _impulse = impulse;

        public void AddImpulse(Vector3 pushImpulse) =>
            _impulse += pushImpulse;
    }
}