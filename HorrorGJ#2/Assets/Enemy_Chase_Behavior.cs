using UnityEngine;

public class Enemy_Chase_Behavior : StateMachineBehaviour
{
    Transform player;
    Enemy enemy;

    int runAwayCount = 0;

    bool runAway = true;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       player = GameObject.FindWithTag("Player").transform;
       enemy = animator.GetComponent<Enemy>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        runAway = enemy.RanFarEnough();

        if(enemy.LightOn() && enemy.PlayerInAttackDistance())
        {
            Debug.Log("Enemy is in the light");
            if(!runAway && runAwayCount < 2)
            {
                Debug.Log("Running away from player.");
                enemy.runAway = false;
                enemy.RunAway();
                runAwayCount++;
                //animator.SetTrigger("Stalk");
            }

        }
        else
        {
            enemy.MoveToPlayer();
            runAwayCount = 0;
        }
        
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger("Chase");
    }
}
