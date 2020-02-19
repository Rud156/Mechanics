using UnityEngine;

namespace Enemy
{
    public class EnemyController : MonoBehaviour
    {
        [Header("Components")] public EnemyMoveController enemyMoveController;
        public EnemyAttackController enemyAttackController;

        #region Enums

        public enum EnemyState
        {
        }

        #endregion
    }
}