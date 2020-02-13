using UnityEngine;

namespace CustomCamera
{
    public class CameraController : MonoBehaviour
    {
        public Transform mainCameraObject;

        [Header("Lerping Data")] public float lerpSpeed;
        public float lerpToleranceAmount;

        private Vector3 m_defaultCameraPosition;

        private bool m_cameraShakeActive;
        private float m_shakeTotalTime;
        private float m_shakeMagnitude;
        private float m_shakeFrequency;
        private float m_currentFrequencyTime;

        private Vector3 m_targetPosition;
        private Vector3 m_startPosition;
        private float m_lerpAmount;

        #region Unity Functions

        private void Start()
        {
            m_defaultCameraPosition = mainCameraObject.localPosition;
            m_cameraShakeActive = false;
        }

        private void Update()
        {
            UpdateCameraLerpPosition();

            if (!m_cameraShakeActive)
            {
                return;
            }

            m_shakeTotalTime -= Time.deltaTime;
            m_currentFrequencyTime -= Time.deltaTime;

            if (m_currentFrequencyTime <= 0)
            {
                Vector3 shakePositionOffset = Vector3.one * Random.Range(-m_shakeMagnitude, m_shakeMagnitude);
                shakePositionOffset += m_defaultCameraPosition;

                m_targetPosition = shakePositionOffset;
                m_startPosition = mainCameraObject.localPosition;
                m_lerpAmount = 0;

                m_currentFrequencyTime = m_shakeFrequency;
            }

            if (m_shakeTotalTime <= 0)
            {
                StopShake();
            }
        }

        #endregion

        #region External Functions

        public void StartCameraShake(float i_shakeTime, float i_shakeMagnitude, float i_shakeFrequency)
        {
            if (m_cameraShakeActive)
            {
                return;
            }

            m_cameraShakeActive = true;
            m_shakeTotalTime = i_shakeTime;
            m_shakeMagnitude = i_shakeMagnitude;
            m_shakeFrequency = i_shakeFrequency;
            m_currentFrequencyTime = 0;
        }

        public void ForceStopCameraShake() => StopShake();

        #endregion

        #region Utility Functions

        private void UpdateCameraLerpPosition()
        {
            if (m_lerpAmount < 1)
            {
                m_lerpAmount += lerpSpeed * Time.deltaTime;
            }

            if (m_lerpAmount >= lerpToleranceAmount)
            {
                mainCameraObject.localPosition = m_targetPosition;
                m_lerpAmount = 1;
            }
            else
            {
                mainCameraObject.localPosition = Vector3.Lerp(
                    m_targetPosition,
                    m_startPosition,
                    m_lerpAmount
                );
            }
        }

        private void StopShake()
        {
            m_cameraShakeActive = false;

            m_targetPosition = m_defaultCameraPosition;
            m_startPosition = mainCameraObject.localPosition;
            m_lerpAmount = 0;
        }

        #endregion

        #region Singleton

        private static CameraController _instance;

        public static CameraController Instance => _instance;

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