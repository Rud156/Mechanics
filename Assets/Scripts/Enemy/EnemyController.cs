using System;
using Common;
using UnityEngine;

namespace Enemy
{
    public class EnemyController : MonoBehaviour
    {
        [Header("Components")] public EnemyMoveController enemyMoveController;
        public EnemyAttackController enemyAttackController;
        public GroundCollisionDetector enemyGroundCollisionDetector;
        public CollisionNotifier enemyCollisionNotifier;

        [Header("Movement")] public float minIdleTime;
        public float maxIdleTime;
        public float minMovementTime;
        public float maxMovementTime;

        private EnemyStateEnum m_enemyState;
        private float m_currentTimer;

        #region Unity Functions

        private void Update()
        {
            switch (m_enemyState)
            {
                case EnemyStateEnum.Idle:
                    break;

                case EnemyStateEnum.Blocking:
                    break;

                case EnemyStateEnum.Attacking:
                    break;

                case EnemyStateEnum.InAir:
                    break;

                case EnemyStateEnum.Dead:
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion

        #region Utility Functions

        private void SetEnemyState(EnemyStateEnum i_enemyStateEnum) => m_enemyState = i_enemyStateEnum;

        #endregion
    }
}