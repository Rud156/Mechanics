using System.Collections.Generic;
using UnityEngine;

namespace Attack
{
    [CreateAssetMenu(menuName = "Attacks/BaseAttack")]
    public class BaseAttack : ScriptableObject
    {
        public new string name;

        [Header("Attack Info")] public List<BaseAttack> allowedAttacks; // If List is empty, any attack is allowed to be played before this
        public List<AttackInputEnum> attackInputs;
        public int attackBlockFrameLoss;
        public AttackEnum attackEnum;

        [Header("Attack Animation")] public float attackRunTime;
        public string attackAnimTrigger;

        private float _currentRunTime;
        private bool _isAttackActive;

        public delegate void AttackLaunched(AttackEnum attackEnum, string attackAnimTrigger);
        public delegate void AttackEnded(AttackEnum attackEnum, string attackAnimTrigger);

        public AttackLaunched OnAttackLaunched;
        public AttackEnded OnAttackEnded;

        #region External Functions

        public bool CanPlayComboAttack(List<AttackInputEnum> attackInputEnums, BaseAttack previousAttack)
        {
            // Probably Contains Will Not Work Because of Instancing
            if (allowedAttacks.Count != 0 && !allowedAttacks.Contains(previousAttack))
            {
                return false;
            }

            if (attackInputEnums.Count != attackInputs.Count)
            {
                return false;
            }

            for (int i = 0; i < attackInputEnums.Count; i++)
            {
                if (attackInputEnums[i] != attackInputs[i])
                {
                    return false;
                }
            }

            return true;
        }

        public bool CanPlayBasicAttack(List<AttackInputEnum> attackInputEnums, BaseAttack previousAttack)
        {
            if (allowedAttacks.Count != 0 && !allowedAttacks.Contains(previousAttack))
            {
                return false;
            }

            AttackInputEnum attackKey = attackInputs[0];
            return attackInputEnums.Contains(attackKey);
        }

        public int GetBlockFrameCount() => attackBlockFrameLoss;

        public bool IsAttackActive() => _isAttackActive;

        #region LifeCycle

        public void LaunchAttack()
        {
            // Probably do something more here..
            _currentRunTime = attackRunTime;
            _isAttackActive = true;

            OnAttackLaunched?.Invoke(attackEnum, attackAnimTrigger);
        }

        public void UpdateAttack()
        {
            // Probably do something more here..
            _currentRunTime -= Time.deltaTime;

            if (_currentRunTime <= 0)
            {
                EndAttack();
            }
        }

        #endregion

        #endregion

        #region Utility Functions

        #region LifeCycle

        private void EndAttack()
        {
            // Probably do something more here..
            _currentRunTime = 0;
            _isAttackActive = false;

            Debug.Log($"Attack Ended: {name}");

            OnAttackEnded?.Invoke(attackEnum, attackAnimTrigger);
        }

        #endregion

        #endregion
    }
}