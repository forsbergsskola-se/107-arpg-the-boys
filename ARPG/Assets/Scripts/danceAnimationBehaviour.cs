using UnityEngine;

public class danceAnimationBehaviour : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    private float _timer;
    public float danceDuration;
    private GrassBoss _bossScript;
    private float _nukeTimer;
    public float nukeSpawnRate;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _timer = 0;
        _nukeTimer = 0;
        _bossScript = FindObjectOfType<GrassBoss>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _timer += Time.deltaTime;
        _nukeTimer += Time.deltaTime;
        if (_timer > danceDuration)
        {
            animator.SetBool("Dance", false);
            _timer = 0;
            _bossScript.switchFromPassive = false;
        }

        if (_nukeTimer > nukeSpawnRate)
        {
            _bossScript.StartCoroutine(_bossScript.CO_Dance());
            _nukeTimer = 0;
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

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
