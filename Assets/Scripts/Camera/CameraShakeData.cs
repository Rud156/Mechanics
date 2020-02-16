using UnityEngine;

namespace CustomCamera
{
    [CreateAssetMenu(menuName = "Camera/Shaker")]
    public class CameraShakeData : ScriptableObject
    {
        public float shakeMagnitude;
        public float shakeTimer;
    }
}