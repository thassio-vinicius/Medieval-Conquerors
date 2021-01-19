using UnityEngine;

public class ResetBoolState : StateMachineBehaviour
{
    [Range(0.0f, 1.0f)]
    public float resetPercentage = 0;
    public string boolToReset;
 
    public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        if (resetPercentage == 0) { animator.SetBool(boolToReset, false); }
    }
 
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stateInfo.normalizedTime >= resetPercentage) { animator.SetBool(boolToReset, false); }
    }
 
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (resetPercentage == 1) { animator.SetBool(boolToReset, false); }
    }
}