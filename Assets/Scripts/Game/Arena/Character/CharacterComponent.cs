using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Arena.Character
{
    public class CharacterComponent : MonoBehaviour
    {
        [SerializeField] private PlayerInput input;
        [SerializeField] private CharacterMovement movement;
        [SerializeField] private CameraController cameraController;
        [SerializeField] private ChargeAbility chargeAbility;

        private InputAction _walkAction;
        private InputAction _chargeAction;
        private InputAction _lookAction;

        private bool inputEnabled = true;

        private void Awake()
        {
            _walkAction = input.actions["Walk"];
            _chargeAction = input.actions["Charge"];
            _lookAction = input.actions["Look"];
            _chargeAction.performed += OnChargePerformed;
            chargeAbility.Completed += OnChargeCompleted;
        }


        private void FixedUpdate()
        {
            var walk = _walkAction.ReadValue<Vector2>();
            movement.Move(walk);

            cameraController.RotateCamera(_lookAction.ReadValue<Vector2>());
        }

        private void OnChargePerformed(InputAction.CallbackContext _)
        {
            return;
            if (chargeAbility.IsCharging) return;

            inputEnabled = false;
            chargeAbility.TriggerCharge(movement.Movement);
        }

        private void OnChargeCompleted(ChargeAbility _) =>
            inputEnabled = true;
    }
}