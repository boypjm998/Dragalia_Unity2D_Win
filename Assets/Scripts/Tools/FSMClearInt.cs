using UnityEngine;

namespace Tools
{
    public class FSMClearInt : StateMachineBehaviour
    {
        
        public string[] clearAtEnter;
        public string[] clearAtExit;
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo,
            int layerIndex)
        {
            foreach(var signal in clearAtEnter) {
                animator.SetInteger(signal,0);
            
           
            }
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo,
            int layerIndex)
        {
            foreach(var signal in clearAtEnter) {
                animator.SetInteger(signal,0);
            
           
            }
        }
        
    }
}