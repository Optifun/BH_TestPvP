using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Arena.Character
{
    public class CharacterMovement : MonoBehaviour
    {
        [SerializeField] private PlayerInput _input;
        private InputAction _walkAction;

        private void Awake()
        {
            _walkAction = _input.actions["Walk"];
        }

        private void FixedUpdate()
        {
            var direction = _walkAction.ReadValue<Vector2>();
            var movementDirection = new Vector3(direction.x, 0, direction.y).normalized;

            var position = transform.position;
            position = Vector3.Lerp(position, position + movementDirection * 10 * Time.fixedDeltaTime, 0.1f);
            transform.position = position;
        }
    }
}