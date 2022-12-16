using UnityEngine;

namespace Game.Arena.Character
{
    public class CharacterRotation : MonoBehaviour
    {
        [SerializeField] private CharacterMovement movement;
        [SerializeField] private Transform model;

        private void FixedUpdate()
        {
            if (movement.Movement != Vector3.zero)
                model.rotation = Quaternion.Lerp(model.rotation, Rotate(movement.Movement), 0.24f);
        }

        private static Quaternion Rotate(Vector3 movementDirection)
        {
            var lookRotation = Quaternion.LookRotation(movementDirection);
            var yAngle = lookRotation.eulerAngles.y;
            return Quaternion.Euler(0, yAngle, 0);
        }
    }
}