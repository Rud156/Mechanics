using Attack;
using UnityEngine;

namespace Player
{
    public class PlayerAttackController : MonoBehaviour
    {
        private static readonly int BaseAttackParam = Animator.StringToHash("Attack");
        private static readonly int RecoilImpactParam = Animator.StringToHash("RImapct");

        public AttackController attackController;
        public Animator playerAnimator;
        public Rigidbody playerRb;
        public float attackStartDelay; // Given as the player might want to start with combos

        private float _currentAttackDelay;
        private bool _attackLaunched;

        #region Unity Functions

        private void Start()
        {
            attackController.OnAttackLaunched += HandleAttackLaunched;
            attackController.OnAttackEnded += HandleAttackEnded;
            attackController.OnResetAttackInputs += HandleResetAttackInputs;
            attackController.OnAttackRecoil += HandleAttackRecoil;

            _currentAttackDelay = attackStartDelay;
            _attackLaunched = false;

            attackController.TargetRb = playerRb;
        }

        private void OnDestroy()
        {
            attackController.OnAttackLaunched -= HandleAttackLaunched;
            attackController.OnAttackEnded -= HandleAttackEnded;
            attackController.OnResetAttackInputs -= HandleResetAttackInputs;
            attackController.OnAttackRecoil -= HandleAttackRecoil;
        }

        private void Update()
        {
            RegisterAttackInputs();

            if (_currentAttackDelay > 0)
            {
                _currentAttackDelay -= Time.deltaTime;
            }
            else
            {
                if (!_attackLaunched)
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

        private void HandleAttackLaunched(AttackEnum attackEnum, string attackAnimTrigger)
        {
            _attackLaunched = true;
            playerAnimator.SetBool(BaseAttackParam, true);
            playerAnimator.SetTrigger(attackAnimTrigger);
        }

        private void HandleAttackEnded(AttackEnum attackEnum, string attackAnimTrigger)
        {
            _attackLaunched = false;
            playerAnimator.SetBool(BaseAttackParam, false);
            playerAnimator.ResetTrigger(attackAnimTrigger);
        }

        private void HandleResetAttackInputs()
        {
            _currentAttackDelay = attackStartDelay; // Reset timer to check for inputs
        }

        private void HandleAttackRecoil()
        {
            playerAnimator.SetTrigger(RecoilImpactParam);
        }

        #endregion
    }
}