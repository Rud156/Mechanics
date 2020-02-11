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

        [Header("Jump")] public float jumpLaunchSpeed;

        [Header("Components")] public Rigidbody playerRb;
        public Animator playerAnimator;

        private int m_movementDirection;
        private bool m_lastFrameJumped;

        private bool m_isOnGround; // Later use a collision checker to handle this

        #region Unity Functions

        private void Start()
        {
            m_movementDirection = 0;
            m_lastFrameJumped = false;

            m_isOnGround = true;
        }

        private void Update()
        {
            HandleInputs();
        }

        private void FixedUpdate()
        {
            HandlePlayerHorizontalMovement();
        }

        #endregion

        #region Utility Functions

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
            }
        }

        private void HandlePlayerHorizontalMovement()
        {
            float movementSpeed = m_isOnGround ? moveSpeed : airMoveSpeed;
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
    }
}