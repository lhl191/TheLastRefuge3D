using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator), typeof(NavMeshAgent))]
public class BossHealth : MonoBehaviour
{
    public float maxHealth = 500f;
    private float currentHealth;

    private Animator animator;
    private NavMeshAgent agent;
    private BossAI bossAI;
    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        bossAI = GetComponent<BossAI>();
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;

        if (currentHealth <= 0f)
        {
            Die();
        }
        else
        {
            animator.SetTrigger("GetHit");
        }
    }

    private void Die()
    {
        if (isDead) return;

        isDead = true;

        animator.ResetTrigger("GetHit");
        animator.SetTrigger("Die");

        // Kiểm tra nếu agent vẫn còn hoạt động (đã được kích hoạt và chưa bị tắt)
        if (agent != null && agent.isActiveAndEnabled)
        {
            agent.isStopped = true;  // Dừng di chuyển khi chết
            agent.velocity = Vector3.zero;  // Đảm bảo không còn chuyển động
            agent.enabled = false;  // Tắt NavMeshAgent khi chết
        }

        if (bossAI != null)
        {
            bossAI.OnDeath(); // Gọi OnDeath để đồng bộ isDead và animation logic
            bossAI.enabled = false;  // Tắt AI sau khi chết
        }

        // Đảm bảo Rigidbody không di chuyển khi chết
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints.FreezeAll;
        }

        MissionData mission = MissionManager.Instance.GetCurrentMission();
        if (mission != null && mission.missionType == MissionData.MissionType.KillBeast)
        {
            Debug.Log("MISSION SUCCESS: KILL BEAST !!");
            MissionManager.Instance.UpdateProgress();
        }
    }


}
