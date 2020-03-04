using CustomCamera;
using UnityEngine;

namespace Common
{
    public class GroundCollisionDetector : MonoBehaviour
    {
        [Header("RayCast Data")] public float rayCastDistance;
        public Vector3 rayCastPlayerPositionOffset;
        public LayerMask layerMask;

        [Header("Camera Shake")] public CameraShakeData cameraShakeData;

        [Header("Debug")] public bool isDebugActive;
        public Color rayCastColor = Color.red;
        public float rayCastTime;

        public delegate void PlayerLanded();
        public PlayerLanded OnPlayerLanded;

        private bool m_lastGroundStatus;
        private bool _mIsOnGround;

        #region Unity Functions

        private void Update()
        {
            Vector3 rayCastPosition = transform.position + rayCastPlayerPositionOffset;

            if (Physics.Raycast(rayCastPosition, Vector3.down, out RaycastHit hit, rayCastDistance, layerMask))
            {
                // Maybe use hit for something later on...
                _mIsOnGround = true;
            }
            else
            {
                _mIsOnGround = false;
            }

            CheckAndNotifyPlayerLanded();
            HandleDebug(rayCastPosition);

            m_lastGroundStatus = _mIsOnGround;
        }

        #endregion

        #region External Functions

        public bool IsOnGround => _mIsOnGround;

        #endregion

        #region Utility Functions

        private void CheckAndNotifyPlayerLanded()
        {
            if (m_lastGroundStatus != _mIsOnGround && _mIsOnGround)
            {
                CameraShaker.Instance.StartCameraShake(cameraShakeData);
                OnPlayerLanded?.Invoke();
            }
        }

        private void HandleDebug(Vector3 rayCastPosition)
        {
            if (!isDebugActive)
            {
                return;
            }

            Debug.DrawRay(rayCastPosition, Vector3.down * rayCastDistance, rayCastColor, rayCastTime);
        }

        #endregion
    }
}