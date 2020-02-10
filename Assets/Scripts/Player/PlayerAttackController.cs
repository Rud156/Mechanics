using Attack;
using Common;
using UnityEngine;
using Utils;

namespace Player
{
    public class PlayerAttackController : MonoBehaviour
    {
        private static readonly int BaseAttackParam = Animator.StringToHash("Attack");
        private static readonly int RecoilImpactParam = Animator.StringToHash("RImpact");

        public CollisionNotifier collisionNotifier;
        public AttackController attackController;
        public Animator playerAnimator;
        public Rigidbody playerRb;
        public float attackStartDelay; // Given as the player might want to start with combos

        private float m_currentAttackDelay;
        private bool m_attackLaunched;

        #region Unity Functions

        private void Start()
        {
            attackController.OnAttackLaunched += HandleAttackLaunched;
            attackController.OnAttackEnded += HandleAttackEnded;
            attackController.OnResetAttackInputs += HandleResetAttackInputs;
            attackController.OnAttackRecoilStart += HandleAttackRecoilStart;
            attackController.OnAttackRecoilEnd += HandleAttackRecoilEnd;

            collisionNotifier.OnSolidCollisionEnter += HandleSwordCollisionEnter;

            m_currentAttackDelay = attackStartDelay;
            m_attackLaunched = false;

            attackController.TargetRb = playerRb;
        }

        private void OnDestroy()
        {
            attackController.OnAttackLaunched -= HandleAttackLaunched;
            attackController.OnAttackEnded -= HandleAttackEnded;
            attackController.OnResetAttackInputs -= HandleResetAttackInputs;
            attackController.OnAttackRecoilStart -= HandleAttackRecoilStart;

            collisionNotifier.OnSolidCollisionEnter -= HandleSwordCollisionEnter;
        }

        private void Update()
        {
            RegisterAttackInputs();

            if (m_currentAttackDelay > 0)
            {
                m_currentAttackDelay -= Time.deltaTime;
            }
            else
            {
                if (!m_attackLaunched)
                {
                    attackController.LaunchAccumulatedAttack();
                }
            }
        }

        #endregion

        #region Utility Functions

        private void RegisterAttackInputs()
        {
            if (Input.GetMouseButtonDown(0))
            {
                attackController.AddAttackInput(AttackInputEnum.BaseAttack);
            }
            else if (Input.GetKeyDown(ControlConstants.Attack_1))
            {
                attackController.AddAttackInput(AttackInputEnum.Attack_1);
            }
            else if (Input.GetKeyDown(ControlConstants.Attack_2))
            {
                attackController.AddAttackInput(AttackInputEnum.Attack_2);
            }
            else if (Input.GetKeyDown(ControlConstants.Attack_3))
            {
                attackController.AddAttackInput(AttackInputEnum.Attack_3);
            }
            else if (Input.GetKeyDown(ControlConstants.Attack_4))
            {
                attackController.AddAttackInput(AttackInputEnum.Attack_4);
            }
        }

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

        private void HandleResetAttackInputs()
        {
            m_currentAttackDelay = attackStartDelay; // Reset timer to check for inputs
        }

        private void HandleAttackRecoilStart()
        {
            playerAnimator.SetTrigger(RecoilImpactParam);
        }

        private void HandleAttackRecoilEnd()
        {
            playerAnimator.ResetTrigger(RecoilImpactParam);
        }

        #endregion
    }
}