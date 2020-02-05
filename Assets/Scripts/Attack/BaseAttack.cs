using System.Collections.Generic;
using UnityEngine;

namespace Attack
{
    [CreateAssetMenu(menuName = "Attacks/BaseAttack")]
    public class BaseAttack : ScriptableObject
    {
        public new string name;

        public List<BaseAttack> allowedAttacks; // If List is empty, any attack is allowed to be played before this
        public List<AttackInputEnum> attackInputs;
        public int attackBlockFrameLoss;
        public float attackRunTime;

        private float _currentRunTime;
        private bool _isAttackActive;

        public delegate void AttackEnded();
        public AttackEnded OnAttackEnded;

        #region External Functions

        public bool CanPlayAttack(List<AttackInputEnum> attackInputEnums, BaseAttack previousAttack)
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

        public int GetBlockFrameCount() => attackBlockFrameLoss;

        public bool IsAttackActive() => _isAttackActive;

        #region LifeCycle

        public void LaunchAttack()
        {
            // Probably do something more here..
            _currentRunTime = attackRunTime;
            _isAttackActive = true;

            Debug.Log(name);
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

            OnAttackEnded?.Invoke();
        }

        #endregion

        #endregion
    }
}