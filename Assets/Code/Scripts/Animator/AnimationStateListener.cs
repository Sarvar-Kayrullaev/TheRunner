using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class AnimationStateListener : StateMachineBehaviour
{
    [SerializeField] bool DisableWhenExited = false;
    public UnityEvent _onStateEnter;
    //public UnityEvent _onStateUpdate;
    public UnityEvent _onStateExit;
    //public UnityEvent _onStateMove;
    //public UnityEvent _onStateIK;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _onStateEnter.Invoke();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    // override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    // {
    //     _onStateUpdate.Invoke();
    // }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       _onStateExit.Invoke();
       if(DisableWhenExited) animator.gameObject.SetActive(false);
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    // override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    // {
    //     _onStateMove.Invoke();
    // }

    // OnStateIK is called right after Animator.OnAnimatorIK()
    // override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    // {
    //     _onStateIK.Invoke();
    // }
}
