using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyIdleState : StateMachineBehaviour
{
    float timer;
    public float idleTime = 0f;

    Transform player;

    public float detectionAreaRadius = 18f;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timer = 0;
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(SoundManager.Instance.enemyChannel.isPlaying == false)
        {
            SoundManager.Instance.enemyChannel.clip = SoundManager.Instance.enemyTalk;
            SoundManager.Instance.enemyChannel.PlayDelayed(3f);
        }

        // --- Transition to Patrol State --- //

        timer += Time.deltaTime;
        if(timer > idleTime)
        {
            animator.SetBool("isPatroling", true);
        }

        // --- Transition to Chase State --- //

        float distanceFromPlayer = Vector3.Distance(player.position, animator.transform.position);
        if(distanceFromPlayer < detectionAreaRadius)
        {
            animator.SetBool("isChasing", true);
        }
    }

     override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       SoundManager.Instance.enemyChannel.Stop();
    }
}
