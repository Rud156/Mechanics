using Attack;
using Common;
using UnityEngine;

namespace Player
{
    public class PlayerAttackController : MonoBehaviour
    {
        private static readonly int BaseAttackParam = Animator.StringToHash("Attack");
        private static readonly int RecoilImpactParam = Animator.StringToHash("RImpact");

        public PlayerCollisionDetector playerCollisionDetector;
        public CollisionNotifier collisionNotifier;
        public AttackController attackController;
        public Animator playerAnimator;
        public Rigidbody playerRb;

        private bool m_attackLaunched;

        #region Unity Functions

        private void Start()
        {
            attackController.OnAttackLaunched += HandleAttackLaunched;
            attackController.OnAttackEnded += HandleAttackEnded;
            attackController.OnAttackRecoilStart += HandleAttackRecoilStart;
            attackController.OnAttackRecoilEnd += HandleAttackRecoilEnd;

            collisionNotifier.OnSolidCollisionEnter += HandleSwordCollisionEnter;
            playerCollisionDetector.OnPlayerLanded += HandlePlayerLanding;

            m_attackLaunched = false;

            attackController.TargetRb = playerRb;
        }

        private void OnDestroy()
        {
            attackController.OnAttackLaunched -= HandleAttackLaunched;
            attackController.OnAttackEnded -= HandleAttackEnded;
            attackController.OnAttackRecoilStart -= HandleAttackRecoilStart;
            attackController.OnAttackRecoilEnd -= HandleAttackRecoilEnd;

            collisionNotifier.OnSolidCollisionEnter -= HandleSwordCollisionEnter;
            playerCollisionDetector.OnPlayerLanded -= HandlePlayerLanding;
        }

        private void Update()
        {
            attackController.IsInAir = !playerCollisionDetector.IsPlayerOnGround;
        }

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

        private void HandleAttackLaunched(AttackEnum i_attackEnum, string i_attackAnimTrigger)
        {
            m_attackLaunched = true;
            playerAnimator.SetBool(BaseAttackParam, true);
            playerAnimator.SetTrigger(i_attackAnimTrigger);
        }

        private void HandleAttackEnded(AttackEnum i_attackEnum, string i_attackAnimTrigger)
        {
            m_attackLaunched = false;
            playerAnimator.SetBool(BaseAttackParam, false);
            playerAnimator.ResetTrigger(i_attackAnimTrigger);
        }

        private void HandlePlayerLanding()
        {
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