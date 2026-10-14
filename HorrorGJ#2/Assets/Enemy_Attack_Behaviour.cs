using UnityEngine;

public class Enemy_Attack_Behaviour : StateMachineBehaviour
{
    Enemy enemy;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       enemy = animator.GetComponent<Enemy>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        if(Vector2.Distance(enemy.player.position, animator.transform.position) > enemy.attackDistance)
        {
            animator.SetTrigger("Chase");
            animator.ResetTrigger("AttackIdle");
            return;
        }

        if(!enemy.InDark())
        {
            enemy.AgentStop();
        }

        animator.SetTrigger("AttackIdle");
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
    }

}
