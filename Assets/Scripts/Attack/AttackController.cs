using System.Collections.Generic;
using UnityEngine;

namespace Attack
{
    public class AttackController : MonoBehaviour
    {
        public List<BaseAttack> allowedBaseAttacks;
        public List<BaseAttack> allowedComboAttacks;

        private BaseAttack _currentRunningAttack;
        private Rigidbody _targetRb;
        private int _currentFrameCount;

        private List<AttackInputEnum> _attackInputs;

        public delegate void AttackLaunched(AttackEnum attackEnum, string attackAnimTrigger);
        public delegate void AttackEnded(AttackEnum attackEnum, string attackAnimTrigger);
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
                if (_targetRb != null)
                {
                    float targetVelocity = _currentRunningAttack.GetAttackVelocity();
                    Vector3 velocity = _targetRb.transform.forward * targetVelocity;
                    velocity.y = 0;
                    _targetRb.velocity = new Vector3(
                        velocity.x,
                        _targetRb.velocity.y,
                        velocity.z
                    );
                }

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

        public Rigidbody TargetRb
        {
            get => _targetRb;
            set => _targetRb = value;
        }

        #endregion

        #region Utility Functions

        private void CheckAndLaunchAttack()
        {
            bool attackSelected = false;
            foreach (BaseAttack allowedComboAttack in allowedComboAttacks)
            {
                // In case the opponent has blocked the attack, prevent the player from attacking for sometime
                if (allowedComboAttack.CanPlayComboAttack(_attackInputs, _currentRunningAttack) &&
                    _currentFrameCount >= 0)
                {
                    _currentRunningAttack = allowedComboAttack;

                    allowedComboAttack.OnAttackLaunched += HandleAttackLaunched;
                    allowedComboAttack.OnAttackEnded += HandleAttackEnded;
                    allowedComboAttack.LaunchAttack();

                    attackSelected = true;
                    break;
                }
            }

            if (_currentRunningAttack != null && !attackSelected)
            {
                foreach (BaseAttack sequentialAttack in _currentRunningAttack.GetSequentialAttacks())
                {
                    // This is because Sequential Attacks can be both ComboAttacks and BasicAttacks
                    if ((sequentialAttack.CanPlayBasicAttack(_attackInputs, _currentRunningAttack) ||
                         sequentialAttack.CanPlayComboAttack(_attackInputs, _currentRunningAttack)) &&
                        _currentFrameCount >= 0)
                    {
                        _currentRunningAttack = sequentialAttack;

                        sequentialAttack.OnAttackLaunched += HandleAttackLaunched;
                        sequentialAttack.OnAttackEnded += HandleAttackEnded;
                        sequentialAttack.LaunchAttack();

                        attackSelected = true;
                        break;
                    }
                }
            }

            // Clear and Check Normal Attacks for Input
            if (!attackSelected)
            {
                foreach (BaseAttack allowedBaseAttack in allowedBaseAttacks)
                {
                    // In case the opponent has blocked the attack, prevent the player from attacking for sometime
                    if (allowedBaseAttack.CanPlayBasicAttack(_attackInputs, _currentRunningAttack) &&
                        _currentFrameCount >= 0)
                    {
                        _currentRunningAttack = allowedBaseAttack;

                        allowedBaseAttack.OnAttackLaunched += HandleAttackLaunched;
                        allowedBaseAttack.OnAttackEnded += HandleAttackEnded;
                        allowedBaseAttack.LaunchAttack();

                        attackSelected = true;
                        break;
                    }
                }
            }

            _attackInputs.Clear();
            if (!attackSelected)
            {
                _currentRunningAttack = null;
                OnResetAttackInputs?.Invoke();
            }
        }

        private void HandleAttackLaunched(AttackEnum attackEnum, string attackAnimTrigger)
        {
            _currentRunningAttack.OnAttackLaunched -= HandleAttackLaunched;
            OnAttackLaunched?.Invoke(attackEnum, attackAnimTrigger);
        }

        private void HandleAttackEnded(AttackEnum attackEnum, string attackAnimTrigger)
        {
            _currentRunningAttack.OnAttackEnded -= HandleAttackEnded;
            OnAttackEnded?.Invoke(attackEnum, attackAnimTrigger);

            CheckAndLaunchAttack();
        }

        #endregion
    }
}