using Attack;
using Common;
using UnityEngine;

namespace Player
{
    public class PlayerAttackController : MonoBehaviour
    {
        private static readonly int BaseAttackParam = Animator.StringToHash("Attack");
        private static readonly int BlockParam = Animator.StringToHash("Block");
        private static readonly int RecoilImpactParam = Animator.StringToHash("RImpact");

        [Header("Components")] public GroundCollisionDetector groundCollisionDetector;
        public CollisionNotifier collisionNotifier;
        public AttackController attackController;
        public Animator playerAnimator;
        public Rigidbody playerRb;

        private bool m_attackLaunched;
        private bool m_blockingActive; // In case this is required later on...

        #region Unity Functions

        private void Start()
        {
            attackController.OnAttackLaunched += HandleAttackLaunched;
            attackController.OnAttackEnded += HandleAttackEnded;
            attackController.OnAttackRecoilStart += HandleAttackRecoilStart;
            attackController.OnAttackRecoilEnd += HandleAttackRecoilEnd;

            attackController.OnAttackBlockingStart += HandleAttackBlockingInitiated;
            attackController.OnAttackBlockingEnd += HandleAttackBlockingCanceled;

            collisionNotifier.OnSolidCollisionEnter += HandleSwordCollisionEnter;
            groundCollisionDetector.OnPlayerLanded += HandlePlayerLanding;

            m_attackLaunched = false;

            attackController.TargetRb = playerRb;
        }

        private void OnDestroy()
        {
            attackController.OnAttackLaunched -= HandleAttackLaunched;
            attackController.OnAttackEnded -= HandleAttackEnded;
            attackController.OnAttackRecoilStart -= HandleAttackRecoilStart;
            attackController.OnAttackRecoilEnd -= HandleAttackRecoilEnd;

            attackController.OnAttackBlockingStart -= HandleAttackBlockingInitiated;
            attackController.OnAttackBlockingEnd -= HandleAttackBlockingCanceled;

            collisionNotifier.OnSolidCollisionEnter -= HandleSwordCollisionEnter;
            groundCollisionDetector.OnPlayerLanded -= HandlePlayerLanding;
        }

        private void Update() => attackController.IsInAir = !groundCollisionDetector.IsPlayerOnGround;

        #endregion

        #region Utility Functions

        private void HandleSwordCollisionEnter(Collision i_other)
        {
            if (!m_attackLaunched)
            {
                return;
            }

            AttackBlock attackBlock = i_other.gameObject.GetComponent<AttackBlock>();
            if (attackBlock)
            {
                Debug.Log("Attack Recoil Hit");
                attackController.BlockCurrentAttack();
            }
        }

        private void HandleAttackBlockingInitiated()
        {
            m_blockingActive = true;
            playerAnimator.SetBool(BaseAttackParam, false); // Ideally this should not be required as the attack has already stopped playing
            playerAnimator.SetBool(BlockParam, true);
        }

        private void HandleAttackBlockingCanceled()
        {
            m_blockingActive = false;
            playerAnimator.SetBool(BlockParam, false);
        }

        private void HandleAttackLaunched(PlayerAttackEnum i_playerAttackEnum, string i_attackAnimTrigger)
        {
            m_attackLaunched = true;
            playerAnimator.SetBool(BaseAttackParam, true);
            playerAnimator.SetTrigger(i_attackAnimTrigger);
        }

        private void HandleAttackEnded(PlayerAttackEnum i_playerAttackEnum, string i_attackAnimTrigger)
        {
            m_attackLaunched = false;
            playerAnimator.SetBool(BaseAttackParam, false);
            playerAnimator.ResetTrigger(i_attackAnimTrigger);
        }

        private void HandlePlayerLanding()
        {
            attackController.IsInAir = !groundCollisionDetector.IsPlayerOnGround;

            if (m_attackLaunched)
            {
                attackController.ForceStopCurrentAttack();
                Debug.Log("Forcing Current AttackStop");
            }

            attackController.LaunchJustLandAttacks();
        }

        #region Recoil

        private void HandleAttackRecoilStart() => playerAnimator.SetTrigger(RecoilImpactParam);

        private void HandleAttackRecoilEnd() => playerAnimator.ResetTrigger(RecoilImpactParam);

        #endregion

        #endregion
    }
}