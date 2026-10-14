using UnityEngine;

public class Enemy_Chase_Behavior : StateMachineBehaviour
{
    Transform player;
    Enemy enemy;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       player = GameObject.FindWithTag("Player").transform;
       enemy = animator.GetComponent<Enemy>();
       animator.ResetTrigger("AttackIdle");
       animator.ResetTrigger("Attack");
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
            
        if(Vector2.Distance(player.position, animator.transform.position) < enemy.attackDistance)
        {
            animator.SetTrigger("AttackIdle");
            animator.ResetTrigger("Attack");
            enemy.AgentStop();
        }

        enemy.RanFarEnough();

        if(!enemy.InDark())
        {
            enemy.RunAway();
        }
        
        if(!enemy.runAway)
        {
            enemy.MoveToPlayer();
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
    }
}
