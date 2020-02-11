using UnityEngine;

namespace Player
{
    public class PlayerWalkAnimBehaviour : StateMachineBehaviour
    {
        private static readonly int MoveDirectionParam = Animator.StringToHash("MoveDirection");

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            int movementDirection = animator.GetInteger(MoveDirectionParam);
            if (movementDirection != 0)
            {
                // Don't do anything here
            }
        }

        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.speed = 1;
        }
    }
}