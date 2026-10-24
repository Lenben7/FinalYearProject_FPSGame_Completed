using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyPatrolingState : StateMachineBehaviour
{
    float timer;
    public float patrolingTime = 10f;

    NavMeshAgent agent;
    Transform player;

    public float detectionArea = 18f;
    public float patrolSpeed = 2f;

    List<Transform> waypointsList = new List<Transform>();

     override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // --- Initialization --- //

        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent = animator.GetComponent<NavMeshAgent>();

        agent.speed = patrolSpeed;
        timer = 0;

        // --- Move to First Waypoint ---//
        GameObject waypointCluster = animator.GetComponentInParent<Enemy>().waypointCluster;
        foreach (Transform t in waypointCluster.transform)
        {
            waypointsList.Add(t);
        }

        Vector3 nextPosition = waypointsList[Random.Range(0 , waypointsList.Count)].position;
        agent.SetDestination(nextPosition);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(SoundManager.Instance.enemyChannel.isPlaying == false)
        {
            SoundManager.Instance.enemyChannel.PlayOneShot(SoundManager.Instance.enemyWalking);
        }

        // --- If agent arrived at waypoint , move to next waypoint --- //

        if(agent.remainingDistance <= agent.stoppingDistance)
        {
            agent.SetDestination(waypointsList[Random.Range(0 , waypointsList.Count)].position);
        }
        
        // --- Transition to Idle State --- //

        timer += Time.deltaTime;
        if(timer > patrolingTime)
        {
            animator.SetBool("isPatroling", false);
        }

        // --- Transition to Chase State --- //'

        float distanceFromPlayer = Vector3.Distance(player.position, animator.transform.position);
        if(distanceFromPlayer < detectionArea)
        {
            animator.SetBool("isChasing", true);
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       // Stop the agent
       agent.SetDestination(agent.transform.position);

       SoundManager.Instance.enemyChannel.Stop();
    }
}
