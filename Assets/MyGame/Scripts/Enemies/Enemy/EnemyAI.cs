using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public Transform player;
    public Transform[] waypoints; // Các điểm tuần tra
    private NavMeshAgent agent;
    private Animator animator;
    private int currentWaypoint = 0;
    private bool isChasing = false;
    private bool isAttacking = false;

    public float chaseRange = 10f;
    public float attackRange = 2f;
    public float attackDamage = 20f;
    public float patrolWaitTime = 3f;
    private float patrolTimer = 0f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Nếu enemy đã chết, dừng toàn bộ hành động
        if (GetComponent<EnemyHealth>().isDead)
        {
            StopEnemyAI();
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange)
        {
            AttackPlayer();
        }
        else if (distanceToPlayer <= chaseRange)
        {
            ChasePlayer();
        }
        else
        {
            Patrol();
        }

        animator.SetFloat("Run", agent.velocity.magnitude);
    }


    void ChasePlayer()
    {
        if (isAttacking || GetComponent<EnemyHealth>().isDead) return;

        if (!agent.enabled) return; // 🔥 Fix lỗi agent bị tắt mà vẫn gọi isStopped

        isChasing = true;
        agent.isStopped = false;
        agent.SetDestination(player.position);
    }


    void AttackPlayer()
    {
        if (isAttacking || GetComponent<EnemyHealth>().isDead) return; 

        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null && playerHealth.isDead)
        {
            StopChasingAndAttacking();
            return;
        }

        isAttacking = true;
        agent.isStopped = true;

        Vector3 direction = (player.position - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));

        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        {
            animator.SetTrigger("Attack");
        }
        Invoke("ResetAttack", 1f);
    }


    void StopChasingAndAttacking()
    {
        isAttacking = false;
        isChasing = false;
        agent.isStopped = true;
        animator.SetFloat("Run", 0); // Dừng chạy
    }

    void ResetAttack()
    {
        isAttacking = false;

        // 🔥 Kiểm tra nếu agent bị tắt, không thực hiện logic di chuyển nữa
        if (!agent.enabled || GetComponent<EnemyHealth>().isDead) return;

        if (Vector3.Distance(transform.position, player.position) <= chaseRange)
        {
            ChasePlayer();
        }
        else
        {
            Patrol();
        }
    }


    void Patrol()
    {
        if (waypoints.Length == 0 || isChasing || isAttacking) return;

        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            patrolTimer += Time.deltaTime;
            if (patrolTimer >= patrolWaitTime)
            {
                patrolTimer = 0f;
                currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
                agent.SetDestination(waypoints[currentWaypoint].position);
            }
        }
    }

    public void DealDamageToPlayer()
    {
        if (GetComponent<EnemyHealth>().isDead) return; // Không gây sát thương nếu đã chết
        if (player == null) return;

        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(attackDamage);
        }
    }

    void StopEnemyAI()
    {
        CancelInvoke(); // ✅ Dừng Invoke ResetAttack
        isAttacking = false;
        isChasing = false;

        animator.ResetTrigger("Attack");
        animator.ResetTrigger("Hit");

        if (agent.enabled)
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
            agent.enabled = false;
        }

        this.enabled = false; // ✅ Dừng AI hoàn toàn
    }

}