using System;
using Attack;
using UnityEngine;

namespace Player
{
    public class PlayerAttackController : MonoBehaviour
    {
        private static readonly int BaseAttackParam = Animator.StringToHash("Attack");
        private static readonly int LightPunchAttackParam = Animator.StringToHash("LightP");
        private static readonly int HookPunchAttackParam = Animator.StringToHash("HookP");
        private static readonly int HeavyPunchAttackParam = Animator.StringToHash("HeavyP");
        private static readonly int UpperPunchAttackParam = Animator.StringToHash("UpperP");
        private static readonly int HighKickAttackParam = Animator.StringToHash("HighK");

        public AttackController attackController;
        public Animator playerAnimator;
        public float attackStartDelay; // Given as the player might want to start with combos

        private float _currentAttackDelay;
        private bool _attackLaunched;

        #region Unity Functions

        private void Start()
        {
            attackController.OnAttackLaunched += HandleAttackLaunched;
            attackController.OnAttackEnded += HandleAttackEnded;
            attackController.OnResetAttackInputs += HandleResetAttackInputs;

            _currentAttackDelay = attackStartDelay;
            _attackLaunched = false;
        }

        private void OnDestroy()
        {
            attackController.OnAttackLaunched -= HandleAttackLaunched;
            attackController.OnAttackEnded -= HandleAttackEnded;
            attackController.OnResetAttackInputs -= HandleResetAttackInputs;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                attackController.AddAttackInput(AttackInputEnum.Attack_1);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                attackController.AddAttackInput(AttackInputEnum.Attack_2);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                attackController.AddAttackInput(AttackInputEnum.Attack_3);
            }

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

        private void HandleAttackLaunched(AttackEnum attackEnum)
        {
            _attackLaunched = true;

            playerAnimator.SetBool(BaseAttackParam, true);
            switch (attackEnum)
            {
                case AttackEnum.HeavyPunch:
                    playerAnimator.SetTrigger(HeavyPunchAttackParam);
                    break;

                case AttackEnum.HighKick:
                    playerAnimator.SetTrigger(HighKickAttackParam);
                    break;

                case AttackEnum.HookPunch:
                    playerAnimator.SetTrigger(HookPunchAttackParam);
                    break;

                case AttackEnum.LightPunch:
                    playerAnimator.SetTrigger(LightPunchAttackParam);
                    break;

                case AttackEnum.UpperPunch:
                    playerAnimator.SetTrigger(UpperPunchAttackParam);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(attackEnum), attackEnum, null);
            }
        }

        private void HandleAttackEnded(AttackEnum attackEnum)
        {
            _attackLaunched = false;
            playerAnimator.SetBool(BaseAttackParam, false);
        }

        private void HandleResetAttackInputs()
        {
            _currentAttackDelay = attackStartDelay; // Reset timer to check for inputs
        }

        #endregion
    }
}