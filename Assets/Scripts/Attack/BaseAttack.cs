using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Attack
{
    [CreateAssetMenu(menuName = "Attacks/BaseAttack")]
    public class BaseAttack : ScriptableObject
    {
        public new string name;

        [Header("Attack Info")]
        public List<BaseAttack> allowedAttacks; // If List is empty, any attack is allowed to be played before this

        public List<AttackInputEnum> attackInputs;
        public float attackBlockStopTime;
        [FormerlySerializedAs("attackEnum")] public PlayerAttackEnum playerAttackEnum;

        [Header("Sequential Attack")] public List<BaseAttack> sequentialAttacks;

        [Header("Attack Animation")] public float attackRunTime;
        public string attackAnimTrigger;
        public Vector3 attackLifeTimeForce;

        private float m_currentRunTime;
        private bool m_isAttackActive;

        public delegate void AttackLaunched(PlayerAttackEnum i_playerAttackEnum, string i_attackAnimTrigger);
        public delegate void AttackEnded(PlayerAttackEnum i_playerAttackEnum, string i_attackAnimTrigger);

        public AttackLaunched OnAttackLaunched;
        public AttackEnded OnAttackEnded;

        #region External Functions

        public bool CanPlayComboAttack(List<AttackInputEnum> i_attackInputEnums, BaseAttack i_previousAttack)
        {
            // Probably Contains Will Not Work Because of Instancing
            if (allowedAttacks.Count != 0 && !allowedAttacks.Contains(i_previousAttack))
            {
                return false;
            }

            if (i_attackInputEnums.Count != attackInputs.Count)
            {
                return false;
            }

            for (int i = 0; i < i_attackInputEnums.Count; i++)
            {
                if (i_attackInputEnums[i] != attackInputs[i])
                {
                    return false;
                }
            }

            return true;
        }

        public bool CanPlayBasicAttack(List<AttackInputEnum> i_attackInputEnums, BaseAttack i_previousAttack)
        {
            if (allowedAttacks.Count != 0 && !allowedAttacks.Contains(i_previousAttack))
            {
                return false;
            }

            AttackInputEnum attackKey = attackInputs[0];
            return i_attackInputEnums.Contains(attackKey);
        }

        public Vector3 GetAttackVelocity() => attackLifeTimeForce;

        public List<BaseAttack> GetSequentialAttacks() => sequentialAttacks;

        public float GetBlockStopTime() => attackBlockStopTime;

        public bool IsAttackActive() => m_isAttackActive;

        #region LifeCycle

        public void LaunchAttack()
        {
            m_currentRunTime = attackRunTime;
            m_isAttackActive = true;

            OnAttackLaunched?.Invoke(playerAttackEnum, attackAnimTrigger);

            Debug.Log($"Attack Launched: {name}");
        }

        public void UpdateAttack()
        {
            m_currentRunTime -= Time.deltaTime;

            if (m_currentRunTime <= 0)
            {
                EndAttack();
            }
        }

        public void ForceEndAttack() => EndAttack();

        #endregion

        #endregion

        #region Utility Functions

        #region LifeCycle

        private void EndAttack()
        {
            m_currentRunTime = 0;
            m_isAttackActive = false;

            Debug.Log($"Attack Ended: {name}");

            OnAttackEnded?.Invoke(playerAttackEnum, attackAnimTrigger);
        }

        #endregion

        #endregion
    }
}