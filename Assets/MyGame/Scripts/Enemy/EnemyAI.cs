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

    public float chaseRange = 10f; // Khoảng cách phát hiện Player
    public float attackRange = 2f; // Khoảng cách tấn công Player
    public float patrolWaitTime = 2f; // Thời gian dừng lại giữa các điểm tuần tra
    private float patrolTimer = 0f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        // Kiểm tra Enemy có đứng trên NavMesh không
        if (!agent.isOnNavMesh)
        {
            Debug.LogError("Enemy is NOT on a NavMesh! Hãy kiểm tra vị trí của Enemy.");
        }
    }

    void Update()
    {
        if (!agent.isOnNavMesh) return; // Nếu chưa đứng trên NavMesh, không làm gì cả

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= chaseRange)
        {
            ChasePlayer();
        }
        else
        {
            Patrol();
        }

        animator.SetFloat("Run", agent.velocity.magnitude);

    }

    // 🏃 Đuổi theo Player nếu trong phạm vi
    void ChasePlayer()
    {
        if (player == null) return;

        isChasing = true;
        agent.isStopped = false;
        agent.SetDestination(player.position);

        // Nếu đến gần Player, tấn công
        if (Vector3.Distance(transform.position, player.position) <= attackRange)
        {
            AttackPlayer();
        }
    }

    // ⚔️ Tấn công khi đến gần Player
    void AttackPlayer()
    {
        agent.isStopped = true; // Dừng di chuyển khi tấn công
        animator.SetTrigger("Attack"); // Chạy animation tấn công
        Debug.Log("Enemy attacks!");
    }

    // 🚶 Tuần tra nếu không thấy Player
    void Patrol()
    {
        if (waypoints.Length == 0) return;

        if (!isChasing)
        {
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
        else
        {
            isChasing = false;
            agent.isStopped = false;
        }
    }
}
