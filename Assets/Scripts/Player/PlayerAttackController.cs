using Attack;
using UnityEngine;

namespace Player
{
    public class PlayerAttackController : MonoBehaviour
    {
        private static readonly int BaseAttackParam = Animator.StringToHash("Attack");

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

        private void HandleAttackLaunched()
        {
            _attackLaunched = true;
        }

        private void HandleAttackEnded()
        {
            _attackLaunched = false;
        }

        private void HandleResetAttackInputs()
        {
            _currentAttackDelay = attackStartDelay; // Reset timer to check for inputs
        }

        #endregion
    }
}