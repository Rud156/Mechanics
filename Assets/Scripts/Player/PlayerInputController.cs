using System.Collections.Generic;
using Attack;
using UnityEngine;
using Utils;

namespace Player
{
    public class PlayerInputController : MonoBehaviour
    {
        public AttackController attackController;
        public PlayerAttackController playerAttackController;
        public PlayerMoveController playerMoveController;
        public float attackStartDelay; // Given as the player might want to start with combos

        private List<AttackInputEnum> m_attackInputs;
        private float m_currentAttackDelay;
        private bool m_attackLaunched;

        private bool m_lastFrameJumped;

        #region Unity Functions

        private void Start()
        {
            attackController.OnAttackLaunched += HandleAttackLaunched;
            attackController.OnAttackEnded += HandleAttackEnded;
            attackController.OnResetAttackInputs += HandleResetAttackInputs;

            m_attackInputs = new List<AttackInputEnum>();
            m_currentAttackDelay = attackStartDelay;
        }

        private void OnDestroy()
        {
            attackController.OnAttackLaunched -= HandleAttackLaunched;
            attackController.OnAttackEnded -= HandleAttackEnded;
            attackController.OnResetAttackInputs -= HandleResetAttackInputs;
        }

        private void Update()
        {
            RegisterInputs();

            if (m_currentAttackDelay > 0)
            {
                m_currentAttackDelay -= Time.deltaTime;
            }
            else
            {
                if (!m_attackLaunched)
                {
                    CheckAndNotifyInputs();
                }
            }
        }

        #endregion

        #region Utility Functions

        #region Inputs

        private void RegisterInputs()
        {
            RegisterAndHandleMoveControllerInput();
            RegisterOtherInputs();
        }

        private void RegisterAndHandleMoveControllerInput()
        {
            int movementDirection = 0;
            if (Input.GetKey(ControlConstants.Forward) || Input.GetKey(ControlConstants.ForwardAlt))
            {
                movementDirection = 1;
            }
            else if (Input.GetKey(ControlConstants.Backward) || Input.GetKey(ControlConstants.BackwardAlt))
            {
                movementDirection = -1;
            }

            m_lastFrameJumped = false;
            if (Input.GetKeyDown(ControlConstants.Jump))
            {
                m_lastFrameJumped = true;
            }

            playerMoveController.RegisterInputs(movementDirection, m_lastFrameJumped);
        }

        private void RegisterOtherInputs()
        {
            if (Input.GetKeyDown(ControlConstants.BaseAttack) ||
                Input.GetMouseButtonDown(0)) // TODO: Remove mouse later on...
            {
                attackController.AddAttackInput(AttackInputEnum.BaseAttack);
                m_attackInputs.Add(AttackInputEnum.BaseAttack);
            }
            else if (Input.GetKeyDown(ControlConstants.Attack_1))
            {
                attackController.AddAttackInput(AttackInputEnum.Attack_1);
                m_attackInputs.Add(AttackInputEnum.Attack_1);
            }
            else if (Input.GetKeyDown(ControlConstants.Attack_2))
            {
                attackController.AddAttackInput(AttackInputEnum.Attack_2);
                m_attackInputs.Add(AttackInputEnum.Attack_2);
            }
            else if (Input.GetKeyDown(ControlConstants.Attack_3))
            {
                attackController.AddAttackInput(AttackInputEnum.Attack_3);
                m_attackInputs.Add(AttackInputEnum.Attack_3);
            }
            else if (Input.GetKeyDown(ControlConstants.Attack_4))
            {
                attackController.AddAttackInput(AttackInputEnum.Attack_4);
                m_attackInputs.Add(AttackInputEnum.Attack_4);
            }
        }

        private void CheckAndNotifyInputs()
        {
            if (!m_attackLaunched)
            {
                attackController.LaunchAccumulatedAttack();
            }

            m_attackInputs.Clear();
        }

        #endregion

        #region Event Handlers

        private void HandleAttackLaunched(AttackEnum i_attackEnum, string i_attackAnimTrigger) =>
            m_attackLaunched = true;

        private void HandleAttackEnded(AttackEnum i_attackEnum, string i_attackAnimTrigger) => m_attackLaunched = false;

        private void HandleResetAttackInputs()
        {
            m_currentAttackDelay = attackStartDelay;
            m_attackInputs.Clear();
        }

        #endregion

        #endregion
    }
}