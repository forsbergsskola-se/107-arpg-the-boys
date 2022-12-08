using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleAnimationBehaviour : StateMachineBehaviour
{
    public float groundScatterCd;
    public float roarCd;

    private float _timer;

    private float _scatterRounds;

    public int roundsBeforeFart;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _timer = 0;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _timer += Time.deltaTime;
        if (_scatterRounds >= roundsBeforeFart)
        {
            if (_timer > roarCd)
            {
                animator.SetBool("Roar",true);
                _scatterRounds = 0;
                _timer = 0;
            }
        }
        else if (_timer > groundScatterCd)
        {
            animator.SetBool("GroundScatter", true);
            _timer = 0;
            _scatterRounds++;
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    { 
        
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
