using System;
using Mirror;
using UnityEngine;

namespace Game.Arena.Character
{
    public class CharacterRotation : NetworkBehaviour
    {
        [SerializeField] private CharacterMovement movement;
        [SerializeField] private Transform model;

        [SyncVar] private Quaternion _rotation = Quaternion.identity;

        public override void OnStartClient()
        {
            _rotation = model.rotation;
        }

        private void FixedUpdate()
        {
            if (!isLocalPlayer)
            {
                model.rotation = LerpRotation(_rotation);
                return;
            }

            if (movement.Movement != Vector3.zero)
                model.rotation = LerpRotation(Rotate(movement.Movement));
        }

        private Quaternion LerpRotation(Quaternion nextValue) =>
            Quaternion.Lerp(model.rotation, nextValue, 0.24f);

        private void Update()
        {
            if (isLocalPlayer)
                _rotation = model.rotation;
        }

        private static Quaternion Rotate(Vector3 movementDirection)
        {
            var lookRotation = Quaternion.LookRotation(movementDirection);
            var yAngle = lookRotation.eulerAngles.y;
            return Quaternion.Euler(0, yAngle, 0);
        }
    }
}