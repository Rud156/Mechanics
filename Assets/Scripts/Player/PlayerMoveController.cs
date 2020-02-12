using UnityEngine;
using Utils;

namespace Player
{
    public class PlayerMoveController : MonoBehaviour
    {
        private static readonly int MoveParam = Animator.StringToHash("Move");
        private static readonly int MoveDirectionParam = Animator.StringToHash("MoveDirection");
        private static readonly int JumpParam = Animator.StringToHash("Jump");
        private static readonly int FallingParam = Animator.StringToHash("Falling");
        private static readonly int LandParam = Animator.StringToHash("Land");

        [Header("Movement")] public float moveSpeed;
        public float airMoveSpeed;

        [Header("Jump and Fall")] public float jumpLaunchSpeed;
        public float fallSpeedIncrementer;
        public float fallSpeedTolerance;
        public float disableFallAfterLand;

        [Header("Components")] public Rigidbody playerRb;
        public Animator playerAnimator;
        public PlayerCollisionDetector playerCollisionDetector;

        private float m_currentLandTime;
        private int m_movementDirection;
        private bool m_lastFrameJumped;

        #region Unity Functions

        private void Start()
        {
            playerCollisionDetector.OnPlayerLanded += HandlePlayerLanding;

            m_movementDirection = 0;
            m_lastFrameJumped = false;
        }

        private void OnDestroy() => playerCollisionDetector.OnPlayerLanded -= HandlePlayerLanding;

        private void Update()
        {
            if (m_currentLandTime > 0)
            {
                m_currentLandTime -= Time.deltaTime;
            }

            HandleInputs();
            UpdatePlayerJumpState();
        }

        private void FixedUpdate() => HandlePlayerHorizontalMovement();

        #endregion

        #region External Functions

        public void RegisterInputs(int i_moveDirection, bool i_jumpPressed)
        {
            m_movementDirection = i_moveDirection;

            m_lastFrameJumped = false;
            if (i_jumpPressed)
            {
                m_lastFrameJumped = true;
                HandlePlayerJumpActivated();
            }
        }

        #endregion

        #region Utility Functions

        #region Inputs

        private void HandleInputs()
        {
            m_movementDirection = 0;
            if (Input.GetKey(ControlConstants.Forward) || Input.GetKey(ControlConstants.ForwardAlt))
            {
                m_movementDirection = 1;
            }
            else if (Input.GetKey(ControlConstants.Backward) || Input.GetKey(ControlConstants.BackwardAlt))
            {
                m_movementDirection = -1;
            }

            m_lastFrameJumped = false;
            if (Input.GetKeyDown(ControlConstants.Jump))
            {
                m_lastFrameJumped = true;
                HandlePlayerJumpActivated();
            }
        }

        #endregion

        #region Jumping

        private void HandlePlayerJumpActivated()
        {
            if (!m_lastFrameJumped || !playerCollisionDetector.IsPlayerOnGround)
            {
                return;
            }

            playerRb.velocity = new Vector3(
                playerRb.velocity.x,
                jumpLaunchSpeed,
                playerRb.velocity.z
            );

            playerAnimator.SetTrigger(JumpParam);
        }

        private void UpdatePlayerJumpState()
        {
            if (playerRb.velocity.y > -fallSpeedTolerance || m_currentLandTime > 0)
            {
                return;
            }

            // Increase fall speed as the player falls
            playerRb.velocity = new Vector3(
                playerRb.velocity.x,
                playerRb.velocity.y - fallSpeedIncrementer,
                playerRb.velocity.z
            );

            playerAnimator.SetBool(FallingParam, true);
        }

        private void HandlePlayerLanding()
        {
            m_currentLandTime = disableFallAfterLand;

            playerAnimator.SetBool(FallingParam, false);
            playerAnimator.SetTrigger(LandParam);
            playerAnimator.ResetTrigger(JumpParam);
        }

        #endregion

        #region Movement

        private void HandlePlayerHorizontalMovement()
        {
            float movementSpeed = playerCollisionDetector.IsPlayerOnGround ? moveSpeed : airMoveSpeed;
            Vector3 movementVelocity = m_movementDirection * movementSpeed * transform.forward;

            playerRb.velocity = new Vector3(
                movementVelocity.x,
                playerRb.velocity.y,
                movementVelocity.z
            );

            playerAnimator.SetBool(MoveParam, m_movementDirection != 0);
            playerAnimator.SetFloat(MoveDirectionParam, m_movementDirection);
        }

        #endregion

        #endregion
    }
}