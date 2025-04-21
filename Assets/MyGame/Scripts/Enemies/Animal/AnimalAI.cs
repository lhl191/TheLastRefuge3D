using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class AnimalAI : MonoBehaviour
{
    public float detectionRange = 15f;
    public float attackRange = 2.5f;
    public float attackCooldown = 2f;
    public float damage = 20f;

    private Transform player;
    private NavMeshAgent agent;
    private Animator animator;
    private bool isChasing = false;
    private bool isDead = false;
    private bool isAttacking = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (isDead || player == null || isAttacking) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (isChasing)
        {
            if (distance <= attackRange)
            {
                StartCoroutine(AttackRoutine()); 
            }
            else if (distance <= detectionRange)
            {
                agent.SetDestination(player.position);
                animator.SetBool("Run", true);
            }
            else
            {
                StopChasing();
            }
        }
    }

    public void StartChasingPlayer()
    {
        isChasing = true;
        animator.SetBool("Run", true);
        Debug.Log("Chasing started: Run = " + true);
    }


    public void StopChasing()
    {
        isChasing = false;
        animator.SetBool("Run", false);
        agent.ResetPath();
    }

    public void OnDeath()
    {
        isDead = true;
        StopChasing();
        agent.isStopped = true;
    }

    IEnumerator AttackRoutine()
    {
        isAttacking = true;
        agent.isStopped = true;
        animator.SetBool("Run", false); 

        animator.SetTrigger("Attack");

        yield return new WaitForSeconds(attackCooldown);

        agent.isStopped = false;

        float distance = Vector3.Distance(transform.position, player.position);
        if (distance > detectionRange)
        {
            StopChasing();
        }

        isAttacking = false;
    }

}
