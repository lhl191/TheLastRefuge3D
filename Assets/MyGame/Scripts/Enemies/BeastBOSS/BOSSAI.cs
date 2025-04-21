using UnityEngine;
using UnityEngine.AI;

public class BossAI : MonoBehaviour
{
    [Header("Combat Settings")]
    public float detectionRange = 25f;
    public float attackRange = 0.5f;
    public float attackCooldown = 2f;
    public float damage = 30f;

    [Header("Misc")]
    public float stopDistanceOffset = 0.5f;

    private Transform player;
    private NavMeshAgent agent;
    private Animator animator;

    private bool isDead = false;
    private bool isAttacking = false;
    private float lastAttackTime;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        agent.updateRotation = false; // ⛔ Tắt xoay tự động


        // Ignore collision
        if (player != null)
        {
            Collider bossCol = GetComponent<Collider>();
            Collider playerCol = player.GetComponent<Collider>();
            if (bossCol != null && playerCol != null)
            {
                Physics.IgnoreCollision(bossCol, playerCol);
            }
        }
    }

    void Update()
    {
        if (isDead || player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= attackRange)
        {
            AttackPlayer(); // Boss xoay đúng 1 lần trong AttackPlayer()
        }
        else if (distance <= detectionRange)
        {
            ChasePlayer(); // Không di chuyển khi isAttacking = true
        }
        else
        {
            StopMovement();
        }

        animator.SetBool("isMoving", agent.velocity.magnitude > 0.1f && !isAttacking);
    }



    void ChasePlayer()
    {
        if (!agent.enabled || isAttacking) return; // 🔥 Thêm điều kiện này để không xoay khi đang đánh

        agent.isStopped = false;
        agent.SetDestination(player.position);

        // Xoay theo hướng di chuyển (nếu muốn mềm hơn)
        Vector3 direction = (player.position - transform.position).normalized;
        if (direction.magnitude > 0.1f)
        {
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f);
        }
    }


    void AttackPlayer()
    {
        if (isAttacking || !agent.enabled) return;

        isAttacking = true;
        agent.isStopped = true;
        agent.velocity = Vector3.zero;

        // Xoay VỀ hướng player đúng 1 lần trước khi đánh
        Vector3 direction = (player.position - transform.position).normalized;
        if (direction.magnitude > 0.1f)
        {
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = lookRotation;
        }

        animator.SetTrigger("Attack");
        lastAttackTime = Time.time;

        Invoke(nameof(ResetAttack), attackCooldown);
    }


    void ResetAttack()
    {
        isAttacking = false;

        if (isDead || player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= detectionRange && distance > attackRange + stopDistanceOffset)
        {
            ChasePlayer();
        }
    }

    public void DealDamage()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);
        if (distance <= attackRange + 0.5f)
        {
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
        }
    }

    void StopMovement()
    {
        agent.isStopped = true;
        agent.velocity = Vector3.zero;
    }

    public void OnDeath()
    {
        isDead = true;

        CancelInvoke();
        animator.ResetTrigger("Attack");
        animator.SetBool("isMoving", false);

        if (agent.enabled)
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
            agent.enabled = false;
        }

        this.enabled = false;
    }
}
