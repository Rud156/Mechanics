using System;
using Common;
using UnityEngine;
using Utils;
using Random = UnityEngine.Random;

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
        public float minDistanceFromPlayer;
        public float moveAwayProbability;
        public float idleNotSwitchProbability;

        [Header("Attack")] public float attackStartRange;

        private Transform m_playerTransform;
        private EnemyStateEnum m_enemyState;
        private float m_currentTimer;

        #region Unity Functions

        private void Start()
        {
            m_playerTransform = GameObject.FindGameObjectWithTag(TagManager.Player).transform;
        }

        private void Update()
        {
            switch (m_enemyState)
            {
                case EnemyStateEnum.Idle:
                {
                    m_currentTimer -= Time.deltaTime;
                    if (m_currentTimer <= 0)
                    {
                        HandleIdleEnd();
                    }
                }
                    break;

                case EnemyStateEnum.Moving:
                {
                    m_currentTimer -= Time.deltaTime;
                    if (m_currentTimer <= 0)
                    {
                        HandleMovementEnd();
                    }
                }
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

        #region External Functions

        public void SetForceIdleState()
        {
            m_currentTimer = Random.Range(minIdleTime, maxIdleTime);
            SetEnemyState(EnemyStateEnum.Idle);
        }

        public float MinPlayerMoveDistance => minDistanceFromPlayer;

        #endregion

        #region Utility Functions

        #region Idle

        private void HandleIdleEnd()
        {
            bool shouldMoveTowardsPlayer = Mathf.Abs(transform.position.z - m_playerTransform.position.z) > minDistanceFromPlayer;
            float randomDecisionValue = Random.value;

            if (randomDecisionValue <= moveAwayProbability)
            {
                shouldMoveTowardsPlayer = false;
            }

            if (shouldMoveTowardsPlayer)
            {
                enemyMoveController.MoveTowardsPlayer();
            }
            else
            {
                enemyMoveController.MoveAwayFromPlayer();
            }

            m_currentTimer = Random.Range(minMovementTime, maxMovementTime);
            SetEnemyState(EnemyStateEnum.Moving);
        }

        #endregion

        #region Movement

        private void HandleMovementEnd()
        {
            float randomValue = Random.value;
            if (randomValue <= idleNotSwitchProbability)
            {
                m_currentTimer = Random.Range(minMovementTime, maxMovementTime);
                SetEnemyState(EnemyStateEnum.Moving);
            }
            else
            {
                m_currentTimer = Random.Range(minIdleTime, maxIdleTime);
                enemyMoveController.SetEnemyIdle();
                SetEnemyState(EnemyStateEnum.Idle);
            }
        }

        #endregion

        private void SetEnemyState(EnemyStateEnum i_enemyStateEnum) => m_enemyState = i_enemyStateEnum;

        #endregion
    }
}