using System.Collections.Generic;
using UnityEngine;

namespace Attack
{
    public class AttackController : MonoBehaviour
    {
        public List<BaseAttack> allowedAttacks;

        private BaseAttack _currentRunningAttack;
        private int _currentFrameCount;

        private List<AttackInputEnum> _attackInputs;

        public delegate void AttackLaunched(AttackEnum attackEnum);
        public delegate void AttackEnded(AttackEnum attackEnum);
        public delegate void ResetAttackInputs();

        public AttackLaunched OnAttackLaunched;
        public AttackEnded OnAttackEnded;
        public ResetAttackInputs OnResetAttackInputs;

        #region Unity Functions

        private void Start()
        {
            _attackInputs = new List<AttackInputEnum>();
            _currentFrameCount = 0;
            _currentRunningAttack = null;
        }

        private void Update()
        {
            if (_currentRunningAttack != null)
            {
                _currentRunningAttack.UpdateAttack();
            }

            if (_currentFrameCount < 0) // Update frame counter to allow attack buffer time
            {
                _currentFrameCount += 1;
            }
        }

        #endregion

        #region External Functions

        public void AddAttackInput(AttackInputEnum attackInputEnum) => _attackInputs.Add(attackInputEnum);

        public void LaunchAccumulatedAttack() => CheckAndLaunchAttack();

        public void AttackBlocked(int blockFrameCount) => _currentFrameCount = blockFrameCount;

        #endregion

        #region Utility Functions

        private void CheckAndLaunchAttack()
        {
            bool attackSelected = false;

            foreach (BaseAttack allowedAttack in allowedAttacks)
            {
                // In case the opponent has blocked the attack, prevent the player from attacking for sometime
                if (allowedAttack.CanPlayAttack(_attackInputs, _currentRunningAttack) && _currentFrameCount >= 0)
                {
                    allowedAttack.OnAttackEnded += HandleAttackEnded;
                    allowedAttack.LaunchAttack();

                    attackSelected = true;
                    _currentRunningAttack = allowedAttack;

                    OnAttackLaunched?.Invoke(allowedAttack.GetAttackEnum());
                    break;
                }
            }

            _attackInputs.Clear();

            if (!attackSelected) // Handle the case where no input or all wrong input was given
            {
                _currentRunningAttack = null;
                OnResetAttackInputs?.Invoke();
            }
        }

        private void HandleAttackEnded()
        {
            _currentRunningAttack.OnAttackEnded -= HandleAttackEnded;
            OnAttackEnded?.Invoke(_currentRunningAttack.GetAttackEnum());

            CheckAndLaunchAttack();
        }

        #endregion
    }
}