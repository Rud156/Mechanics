using UnityEngine;

namespace Common
{
    public class CollisionNotifier : MonoBehaviour
    {
        public delegate void TriggerCollisionEnter(Collider i_other);
        public delegate void TriggerCollisionExit(Collider i_other);
        public delegate void SolidCollisionEnter(Collision i_other);
        public delegate void SolidCollisionExit(Collision i_other);

        public TriggerCollisionEnter OnTriggerCollisionEnter;
        public TriggerCollisionExit OnTriggerCollisionExit;
        public SolidCollisionEnter OnSolidCollisionEnter;
        public SolidCollisionExit OnSolidCollisionExit;

        #region Unity Functions

        private void OnTriggerEnter(Collider i_other)
        {
            OnTriggerCollisionEnter?.Invoke(i_other);
        }

        private void OnTriggerExit(Collider i_other)
        {
            OnTriggerCollisionExit?.Invoke(i_other);
        }

        private void OnCollisionEnter(Collision i_other)
        {
            OnSolidCollisionEnter?.Invoke(i_other);
        }

        private void OnCollisionExit(Collision i_other)
        {
            OnSolidCollisionExit?.Invoke(i_other);
        }

        #endregion
    }
}