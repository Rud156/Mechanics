using System.Collections.Generic;
using UnityEngine;

namespace Attack
{
    public class AttackController : MonoBehaviour
    {
        public List<BaseAttack> allowedInAirAttacks;
        public List<BaseAttack> allowedJustLandAttacks;
        public List<BaseAttack> allowedBaseAttacks;
        public List<BaseAttack> allowedComboAttacks;
        public float blockRecoilVelocity;

        private BaseAttack m_currentRunningAttack;
        private Rigidbody m_targetRb;
        private float m_currentAttackStopTime;

        private List<AttackInputEnum> m_attackInputs;
        private bool m_isInAir;

        public delegate void AttackLaunched(AttackEnum i_attackEnum, string i_attackAnimTrigger);
        public delegate void AttackEnded(AttackEnum i_attackEnum, string i_attackAnimTrigger);
        public delegate void ResetAttackInputs();
        public delegate void AttackRecoilStart();
        public delegate void AttackRecoilEnd();

        public AttackLaunched OnAttackLaunched;
        public AttackEnded OnAttackEnded;
        public ResetAttackInputs OnResetAttackInputs;
        public AttackRecoilStart OnAttackRecoilStart;
        public AttackRecoilEnd OnAttackRecoilEnd;

        #region Unity Functions

        private void Start()
        {
            m_attackInputs = new List<AttackInputEnum>();
            m_currentAttackStopTime = 0;
            m_currentRunningAttack = null;
        }

        private void Update()
        {
            if (m_currentRunningAttack != null)
            {
                if (m_targetRb != null)
                {
                    Vector3 targetVelocity = m_currentRunningAttack.GetAttackVelocity();
                    Vector3 velocity = targetVelocity;
                    velocity.y = 0;
                    m_targetRb.velocity = new Vector3(
                        velocity.x,
                        m_targetRb.velocity.y,
                        velocity.z
                    );
                }

                m_currentRunningAttack.UpdateAttack();
            }

            if (m_currentAttackStopTime < 0) // Update frame counter to allow attack buffer time
            {
                m_currentAttackStopTime += Time.deltaTime;

                if (m_currentAttackStopTime >= 0)
                {
                    OnAttackRecoilEnd?.Invoke();
                }

                Vector3 velocity = -m_targetRb.transform.forward * blockRecoilVelocity;
                velocity.y = 0;
                m_targetRb.velocity = new Vector3(
                    velocity.x,
                    m_targetRb.velocity.y,
                    velocity.z
                );
            }
        }

        #endregion

        #region External Functions

        public void AddAttackInput(AttackInputEnum attackInputEnum) => m_attackInputs.Add(attackInputEnum);

        public void LaunchAccumulatedAttack()
        {
            if (m_isInAir)
            {
                CheckAndLaunchInAirAttack();
            }
            else
            {
                CheckAndLaunchGroundAttack();
            }
        }

        public void LaunchJustLandAttacks() => CheckAndLaunchJustLandAttacks();

        public void ForceStopCurrentAttack()
        {
            if (m_currentRunningAttack != null)
            {
                m_currentRunningAttack.ForceEndAttack();
            }
        }

        public void ClearAttackInputs() => m_attackInputs.Clear();

        public void BlockCurrentAttack()
        {
            if (m_currentRunningAttack == null)
            {
                return;
            }

            float attackBlockTime = m_currentRunningAttack.GetBlockStopTime();
            m_attackInputs.Clear(); // Clear inputs as we don't want to launch a next attack
            m_currentRunningAttack.ForceEndAttack();

            m_currentAttackStopTime = -attackBlockTime;
            OnAttackRecoilStart?.Invoke();
        }

        public Rigidbody TargetRb
        {
            get => m_targetRb;
            set => m_targetRb = value;
        }

        public bool IsInAir
        {
            get => m_isInAir;
            set => m_isInAir = value;
        }

        #endregion

        #region Utility Functions

        private void CheckAndLaunchInAirAttack()
        {
            bool attackSelected = false;
            foreach (BaseAttack allowedInAirAttack in allowedInAirAttacks)
            {
                if ((allowedInAirAttack.CanPlayComboAttack(m_attackInputs, m_currentRunningAttack) ||
                     allowedInAirAttack.CanPlayBasicAttack(m_attackInputs, m_currentRunningAttack)) &&
                    m_currentAttackStopTime >= 0)
                {
                    m_currentRunningAttack = allowedInAirAttack;

                    allowedInAirAttack.OnAttackLaunched += HandleAttackLaunched;
                    allowedInAirAttack.OnAttackEnded += HandleAttackEnded;
                    allowedInAirAttack.LaunchAttack();

                    attackSelected = true;
                    break;
                }
            }

            m_attackInputs.Clear();
            if (!attackSelected)
            {
                m_currentRunningAttack = null;
                OnResetAttackInputs?.Invoke();
            }
        }

        private void CheckAndLaunchGroundAttack()
        {
            bool attackSelected = false;

            if (!attackSelected)
            {
                foreach (BaseAttack allowedComboAttack in allowedComboAttacks)
                {
                    // In case the opponent has blocked the attack, prevent the player from attacking for sometime
                    if (allowedComboAttack.CanPlayComboAttack(m_attackInputs, m_currentRunningAttack) &&
                        m_currentAttackStopTime >= 0)
                    {
                        m_currentRunningAttack = allowedComboAttack;

                        allowedComboAttack.OnAttackLaunched += HandleAttackLaunched;
                        allowedComboAttack.OnAttackEnded += HandleAttackEnded;
                        allowedComboAttack.LaunchAttack();

                        attackSelected = true;
                        break;
                    }
                }
            }

            if (m_currentRunningAttack != null && !attackSelected)
            {
                foreach (BaseAttack sequentialAttack in m_currentRunningAttack.GetSequentialAttacks())
                {
                    // This is because Sequential Attacks can be both ComboAttacks and BasicAttacks
                    if ((sequentialAttack.CanPlayBasicAttack(m_attackInputs, m_currentRunningAttack) ||
                         sequentialAttack.CanPlayComboAttack(m_attackInputs, m_currentRunningAttack)) &&
                        m_currentAttackStopTime >= 0)
                    {
                        m_currentRunningAttack = sequentialAttack;

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
                    if (allowedBaseAttack.CanPlayBasicAttack(m_attackInputs, m_currentRunningAttack) &&
                        m_currentAttackStopTime >= 0)
                    {
                        m_currentRunningAttack = allowedBaseAttack;

                        allowedBaseAttack.OnAttackLaunched += HandleAttackLaunched;
                        allowedBaseAttack.OnAttackEnded += HandleAttackEnded;
                        allowedBaseAttack.LaunchAttack();

                        attackSelected = true;
                        break;
                    }
                }
            }

            m_attackInputs.Clear();
            if (!attackSelected)
            {
                m_currentRunningAttack = null;
                OnResetAttackInputs?.Invoke();
            }
        }

        private void CheckAndLaunchJustLandAttacks()
        {
            bool attackSelected = false;
            foreach (BaseAttack allowedInAirAttack in allowedJustLandAttacks)
            {
                if ((allowedInAirAttack.CanPlayComboAttack(m_attackInputs, m_currentRunningAttack) ||
                     allowedInAirAttack.CanPlayBasicAttack(m_attackInputs, m_currentRunningAttack)) &&
                    m_currentAttackStopTime >= 0)
                {
                    if (m_currentRunningAttack != null)
                    {
                        m_currentRunningAttack.ForceEndAttack();
                    }

                    m_currentRunningAttack = allowedInAirAttack;

                    allowedInAirAttack.OnAttackLaunched += HandleAttackLaunched;
                    allowedInAirAttack.OnAttackEnded += HandleAttackEnded;
                    allowedInAirAttack.LaunchAttack();

                    attackSelected = true;
                    break;
                }
            }

            m_attackInputs.Clear();
            if (!attackSelected)
            {
                OnResetAttackInputs?.Invoke();
            }
        }

        private void HandleAttackLaunched(AttackEnum i_attackEnum, string i_attackAnimTrigger)
        {
            m_currentRunningAttack.OnAttackLaunched -= HandleAttackLaunched;
            OnAttackLaunched?.Invoke(i_attackEnum, i_attackAnimTrigger);
        }

        private void HandleAttackEnded(AttackEnum i_attackEnum, string i_attackAnimTrigger)
        {
            m_currentRunningAttack.OnAttackEnded -= HandleAttackEnded;
            OnAttackEnded?.Invoke(i_attackEnum, i_attackAnimTrigger);

            LaunchAccumulatedAttack();
        }

        #endregion
    }
}