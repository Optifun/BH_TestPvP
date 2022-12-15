using System;
using UnityEngine;

namespace Core
{
    public class CollisionDetector : MonoBehaviour
    {
        public event Action<GameObject, Collision> CollisionEnter;
        public event Action<GameObject, Collision> CollisionExit;
        public event Action<GameObject, Collision> CollisionStay;
        public event Action<GameObject, Collider> TriggerEnter;
        public event Action<GameObject, Collider> TriggerExit;
        public event Action<GameObject, Collider> TriggerStay;

        private void OnCollisionEnter(Collision other) =>
            CollisionEnter?.Invoke(gameObject, other);

        private void OnCollisionExit(Collision other) =>
            CollisionExit?.Invoke(gameObject, other);

        private void OnCollisionStay(Collision other) =>
            CollisionStay?.Invoke(gameObject, other);

        private void OnTriggerEnter(Collider other) =>
            TriggerEnter?.Invoke(gameObject, other);

        private void OnTriggerExit(Collider other) =>
            TriggerExit?.Invoke(gameObject, other);

        private void OnTriggerStay(Collider other) =>
            TriggerStay?.Invoke(gameObject, other);
    }
}