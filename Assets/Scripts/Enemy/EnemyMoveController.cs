using System;
using Common;
using UnityEngine;
using Utils;

namespace Enemy
{
    public class EnemyMoveController : MonoBehaviour
    {
        private static readonly int MoveParam = Animator.StringToHash("Move");
        private static readonly int MoveDirectionParam = Animator.StringToHash("MovementDirection");

        [Header("Components")] public EnemyController enemyController;
        public GroundCollisionDetector groundCollisionDetector;
        public Animator enemyAnimator;
        public Rigidbody enemyRb;

        [Header("Movement Speed")] public float moveForce;
        public float airMoveForce;
        public float maxXZMovementAmount;

        private Transform m_player;
        private EnemyMoveState m_enemyMoveState;
        private int m_movementDirection;

        #region Unity Functions

        private void Start() => m_player = GameObject.FindGameObjectWithTag(TagManager.Player).transform;

        private void Update()
        {
            switch (m_enemyMoveState)
            {
                case EnemyMoveState.MoveTowards:
                    UpdateMoveTowardsPlayer();
                    UpdateMoveEnemy();
                    break;

                case EnemyMoveState.MoveAway:
                    UpdateMoveAwayFromPlayer();
                    UpdateMoveEnemy();
                    break;

                case EnemyMoveState.Idle:
                    UpdateIdleEnemyState();
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion

        #region External Functions

        public void MoveTowardsPlayer() => SetEnemyMoveState(EnemyMoveState.MoveTowards);

        public void MoveAwayFromPlayer() => SetEnemyMoveState(EnemyMoveState.MoveAway);

        public void SetEnemyIdle()
        {
            m_movementDirection = 0;

            enemyRb.velocity = Vector3.zero;
            enemyAnimator.SetBool(MoveParam, m_movementDirection != 0);
            enemyAnimator.SetFloat(MoveDirectionParam, m_movementDirection);

            SetEnemyMoveState(EnemyMoveState.Idle);
        }

        #endregion

        #region Utility Functions

        private void UpdateMoveTowardsPlayer()
        {
            float zDiff = m_player.position.z - transform.position.z;
            m_movementDirection = (int) Mathf.Sign(zDiff);
        }

        private void UpdateMoveAwayFromPlayer()
        {
            float zDiff = m_player.position.z - transform.position.z;
            m_movementDirection = -((int) Mathf.Sign(zDiff));
        }

        private void UpdateMoveEnemy()
        {
            float movementSpeed = groundCollisionDetector.IsOnGround ? moveForce : airMoveForce;
            Vector3 movementForce = m_movementDirection * movementSpeed * Vector3.forward; // TODO: Handle when enemy gets behind the player

            if (Mathf.Abs(transform.position.z - m_player.position.z) < enemyController.MinPlayerMoveDistance &&
                m_enemyMoveState == EnemyMoveState.MoveTowards)
            {
                movementForce = Vector3.zero;
                m_movementDirection = 0;

                enemyController.SetForceIdleState();
                SetEnemyMoveState(EnemyMoveState.Idle);
            }

            if (!(enemyRb.velocity.x > maxXZMovementAmount || enemyRb.velocity.z > maxXZMovementAmount))
            {
                enemyRb.AddForce(movementForce, ForceMode.Acceleration);
            }

            enemyAnimator.SetBool(MoveParam, m_movementDirection != 0);
            enemyAnimator.SetFloat(MoveDirectionParam, m_movementDirection);
        }

        private void UpdateIdleEnemyState()
        {
            // Probably do something here later./..
        }

        private void SetEnemyMoveState(EnemyMoveState i_enemyMoveState) => m_enemyMoveState = i_enemyMoveState;

        #endregion

        #region Enums

        private enum EnemyMoveState
        {
            MoveTowards,
            MoveAway,
            Idle
        }

        #endregion
    }
}