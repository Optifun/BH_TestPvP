using System;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Arena.Character
{
    [RequireComponent(typeof(PlayerInput))]
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private PlayerInput _input;
        [SerializeField] private Transform _cameraLookAt;
        [SerializeField] private Transform _character;
        private InputAction _lookAction;
        [SerializeField] private float _rotationSensitive = 0.3f;

        private void Awake()
        {
            _lookAction = _input.actions["Look"];
        }

        public void AttachCamera(CinemachineVirtualCamera camera)
        {
            camera.Follow = _character;
            camera.LookAt = _cameraLookAt;
        }

        private void Update()
        {
            RotateCamera(_lookAction.ReadValue<Vector2>());
        }

        private void RotateCamera(Vector2 look)
        {
            var rotation = _cameraLookAt.transform.rotation;
            rotation *= Quaternion.AngleAxis(look.x * _rotationSensitive, Vector3.up);
            rotation *= Quaternion.AngleAxis(look.y * _rotationSensitive, Vector3.left);
            _cameraLookAt.transform.rotation = rotation;

            var angles = _cameraLookAt.transform.localEulerAngles;
            angles.z = 0;

            var vAngle = angles.x;

            if (vAngle < 180 && vAngle > 55)
            {
                angles.x = 55;
            }
            else if (vAngle > 180 && vAngle < 320)
            {
                angles.x = 320;
            }

            _cameraLookAt.transform.localEulerAngles = angles;
        }
    }
}