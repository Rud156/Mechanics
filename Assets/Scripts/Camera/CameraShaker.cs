using UnityEngine;

namespace CustomCamera
{
    public class CameraShaker : MonoBehaviour
    {
        public Transform mainCameraObject;

        [Header("Lerping Data")] public float lerpSpeed;
        public float lerpToleranceAmount;

        private Vector3 m_defaultCameraPosition;

        private bool m_cameraShakeActive;
        private float m_shakeTotalTime;
        private float m_shakeMagnitude;

        #region Unity Functions

        private void Start()
        {
            m_defaultCameraPosition = mainCameraObject.localPosition;
            m_cameraShakeActive = false;
        }

        private void Update()
        {
            if (!m_cameraShakeActive)
            {
                return;
            }

            m_shakeTotalTime -= Time.deltaTime;

            Vector3 shakePositionOffset = Vector3.one * Random.Range(-m_shakeMagnitude, m_shakeMagnitude);
            shakePositionOffset += m_defaultCameraPosition;
            mainCameraObject.localPosition = shakePositionOffset;

            if (m_shakeTotalTime <= 0)
            {
                StopShake();
            }
        }

        #endregion

        #region External Functions

        public void StartCameraShake(CameraShakeData i_cameraShakeData)
        {
            if (m_cameraShakeActive)
            {
                return;
            }

            float shakeTime = i_cameraShakeData.shakeTimer;
            float shakeMagnitude = i_cameraShakeData.shakeMagnitude;

            m_cameraShakeActive = true;
            m_shakeTotalTime = shakeTime;
            m_shakeMagnitude = shakeMagnitude;
        }

        public void ForceStopCameraShake() => StopShake();

        #endregion

        #region Utility Functions

        private void StopShake()
        {
            m_cameraShakeActive = false;
            mainCameraObject.localPosition = m_defaultCameraPosition;
        }

        #endregion

        #region Singleton

        private static CameraShaker _instance;

        public static CameraShaker Instance => _instance;

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
            }

            if (_instance != this)
            {
                Destroy(gameObject);
            }
        }

        #endregion
    }
}