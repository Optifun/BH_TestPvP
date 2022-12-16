using System;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Arena.Character
{
    [RequireComponent(typeof(PlayerInput))]
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private Transform _headTransform;
        [SerializeField] private Transform _shouldersTransform;
        [SerializeField] private float _rotationSensitive = 0.3f;

        public void AttachCamera(CinemachineVirtualCamera camera)
        {
            camera.Follow = _headTransform;
            camera.LookAt = null;
        }

        public void RotateCamera(Vector2 look)
        {
            var horizontal = Quaternion.AngleAxis(look.x * _rotationSensitive, Vector3.up);
            var vertical = Quaternion.AngleAxis(look.y * _rotationSensitive, Vector3.left);
            var newRotation = _headTransform.rotation * horizontal * vertical;

            _headTransform.rotation = newRotation;
            _shouldersTransform.rotation = Quaternion.Euler(0, newRotation.eulerAngles.y, 0);

            var angles = _headTransform.localEulerAngles;
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

            _headTransform.transform.localEulerAngles = angles;
        }
    }
}