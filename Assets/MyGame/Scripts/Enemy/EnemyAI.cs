using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public Transform player; // Player cần đuổi theo
    public Transform[] waypoints; // Các điểm tuần tra
    private NavMeshAgent agent;
    private Animator animator;
    private int currentWaypoint = 0;
    private bool isChasing = false; // Đang đuổi theo Player
    private bool isAttacking = false; // Đang tấn công Player

    public float chaseRange = 10f; // Khoảng cách phát hiện Player
    public float attackRange = 2f; // Khoảng cách tấn công Player
    public float patrolWaitTime = 2f; // Thời gian dừng lại giữa các điểm tuần tra
    private float patrolTimer = 0f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange)
        {
            AttackPlayer(); // Nếu Player vào phạm vi tấn công, dừng lại và đánh
        }
        else if (distanceToPlayer <= chaseRange)
        {
            ChasePlayer(); // Nếu Player trong phạm vi đuổi, bắt đầu đuổi theo
        }
        else
        {
            Patrol(); // Nếu không thấy Player, tuần tra
        }

        animator.SetFloat("Run", agent.velocity.magnitude);
    }

    // 🏃 Enemy chỉ đuổi theo Player nếu Player vào phạm vi Chase
    void ChasePlayer()
    {
        if (player == null || isAttacking) return; // Nếu đang đánh thì không chạy

        isChasing = true;
        agent.isStopped = false;
        agent.SetDestination(player.position);
    }

    // ⚔️ Enemy tấn công Player và không di chuyển khi đánh
    void AttackPlayer()
    {
        if (isAttacking) return;

        isAttacking = true;
        agent.isStopped = true;

        // Quay Enemy hướng về Player
        Vector3 direction = (player.position - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));

        animator.SetTrigger("Attack");
        Invoke("ResetAttack", 1.5f);
    }


    void ResetAttack()
    {
        isAttacking = false;
        agent.isStopped = false; // Sau khi đánh xong, có thể đuổi theo tiếp nếu cần
    }

    // 🚶 Enemy tuần tra nếu không thấy Player
    void Patrol()
    {
        if (waypoints.Length == 0 || isChasing || isAttacking) return; // Không tuần tra nếu đang đuổi hoặc đánh

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
}
