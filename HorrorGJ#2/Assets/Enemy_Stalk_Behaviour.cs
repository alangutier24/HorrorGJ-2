using UnityEngine;

public class Enemy_Stalk_Behaviour : StateMachineBehaviour
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


        if (enemy.InDark())
        {
            enemy.Stalk();
        }
        else
        {
            //animator.SetTrigger("Hurt");
        }

        if(enemy.ReachedEdgeOfPlayer() && !enemy.LightOn())
        {
            Debug.Log("Reached edge of player, switching to Chase.");
            animator.SetTrigger("Chase");
        }
        else if(enemy.ReachedEdgeOfPlayer() && enemy.LightOn())
        {
            //enemy.TakeDamage();
            enemy.Stalk();
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger("Hurt");
    }
}
