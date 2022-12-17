using UnityEngine;

namespace Core
{
    public struct RigidbodyParams
    {
        public float Drag;
        public float Mass;
        public float AngularDrag;

        public RigidbodyParams(Rigidbody rigidbody)
        {
            Drag = rigidbody.drag;
            Mass = rigidbody.mass;
            AngularDrag = rigidbody.angularDrag;
        }

        public Rigidbody Apply(Rigidbody rigidbody)
        {
            rigidbody.drag = Drag;
            rigidbody.mass = Mass;
            rigidbody.angularDrag = AngularDrag;
            return rigidbody;
        }
    }
}