using System;
using System.Collections;
using UnityEngine;

public class IdleAnimationBehaviour : StateMachineBehaviour
{
    private GrassBoss _bossScript;
    public float groundScatterCd;
    public float roarCd;
    public float danceCd;

    private float _timer;
    private float _passiveTimer;

    public int roundsBeforeSpecial;
    private bool _switchSpecial;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _timer = 0;
        _bossScript = FindObjectOfType<GrassBoss>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_bossScript.switchFromPassive)
        {
            _timer += Time.deltaTime;
            if (_timer > groundScatterCd && _bossScript.scatterRounds < roundsBeforeSpecial)
                MainAttack();
            
            if (_timer > danceCd && _bossScript.scatterRounds >= roundsBeforeSpecial && _switchSpecial)
                DanceAttack();
            
            if (_timer > roarCd && _bossScript.scatterRounds >= roundsBeforeSpecial && !_switchSpecial)
                FartAttack();
        }

        if (!_bossScript.switchFromPassive && !_bossScript.passiveStageActive)
            _bossScript.StartCoroutine(_bossScript.CO_PassiveStage());

        void MainAttack()
        {
            animator.SetBool("GroundScatter",true);
            _timer = 0;
            _bossScript.scatterRounds++;
        }

        void FartAttack()
        {
            animator.SetBool("Roar",true);
            _bossScript.scatterRounds = 0;
            _timer = 0;
            _switchSpecial = true;
        }

        void DanceAttack()
        {
            animator.SetBool("Dance",true);
            _bossScript.scatterRounds = 0;
            _timer = 0;
            _switchSpecial = false;
        }
    }

    

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    // public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    // { 
    //     
    // }

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
