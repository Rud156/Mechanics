using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace CustomCamera
{
    public class CameraController : MonoBehaviour
    {
        public List<Transform> targets;
        public Camera mainCamera;

        [Header("Camera Follow")] public float smoothTime = 0.5f;
        public Vector3 offset;

        [Header("Zoom")] public float minZoomDistance;
        public float maxZoomDistance;
        public float minDistanceAmount;
        public float maxDistanceAmount;

        private Vector3 m_velocity;

        #region Unity Functions

        private void LateUpdate()
        {
            MoveCamera();
            ZoomCamera();
        }

        #endregion

        #region Utility Functions

        private void MoveCamera()
        {
            Vector3 centerPosition = GetCenterPosition();
            Vector3 newPosition = centerPosition + offset;

            transform.position = Vector3.SmoothDamp(
                transform.position,
                newPosition,
                ref m_velocity,
                smoothTime
            );
        }

        private void ZoomCamera()
        {
            float mappedDistance = ExtensionFunctions.Map(
                GetGreatestDistance(),
                minDistanceAmount,
                maxDistanceAmount,
                1, 0
            );
            float newZoom = Mathf.Lerp(maxZoomDistance, minZoomDistance, mappedDistance);
            mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, newZoom, Time.deltaTime);
        }

        private float GetGreatestDistance()
        {
            Bounds bounds = new Bounds(targets[0].position, Vector3.zero);
            foreach (Transform target in targets)
            {
                bounds.Encapsulate(target.position);
            }

            return bounds.size.z;
        }

        private Vector3 GetCenterPosition()
        {
            Bounds bounds = new Bounds(targets[0].position, Vector3.zero);
            foreach (Transform target in targets)
            {
                bounds.Encapsulate(target.position);
            }

            return bounds.center;
        }

        #endregion
    }
}