using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [SerializeField] private int HP = 100;
    private Animator animator;

    private NavMeshAgent navAgent;

    public bool isDead;
    public float bodyDespawn = 5f;

    public GameObject waypointCluster;

    void Start()
    {
        animator = GetComponent<Animator>();
        navAgent = GetComponent<NavMeshAgent>();

        isDead = false;
    }
    
    public void TakeDamage(int damageAmount)
    {
        HP -= damageAmount;

        if(HP <= 0)
        {
            int randomValue = Random.Range(0, 2); // 0 or 1

            if (randomValue == 0)
            {
                animator.SetTrigger("DIE1");
            }
            else
            {
                animator.SetTrigger("DIE2");
            }

            isDead = true;

            GlobalReferences.Instance.scoreNumber += 5;

            GetComponent<CapsuleCollider>().enabled = false;

            GetComponentInChildren<SphereCollider>().enabled = false;

            // Dead Sound
            SoundManager.Instance.enemyChannel2.PlayOneShot(SoundManager.Instance.enemyDeath);

            Destroy(gameObject, bodyDespawn);
        }
        else
        {
            animator.SetTrigger("DAMAGE");

            // Hurt Sound
            SoundManager.Instance.enemyChannel2.PlayOneShot(SoundManager.Instance.enemyHurt);
        } 
    }

    private void OnDrawGizmos() 
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 2.5f); // Attacking // Stop Attacking

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, 18f); // Detection (Start Chasing)

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 21f); // Stop Chasing
    }
}
