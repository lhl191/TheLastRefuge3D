using UnityEngine;
using UnityEngine.AI;

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;
    private Animator animator;
    public bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            animator.SetTrigger("Hit"); // Animation bị đánh
        }
    }

    void Die()
    {
        if (isDead) return;

        isDead = true;
        animator.ResetTrigger("Attack");
        animator.ResetTrigger("Hit");
        animator.SetBool("Die", true);

        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        agent.isStopped = true;
        agent.velocity = Vector3.zero;  // Ngừng mọi di chuyển
        agent.enabled = false;  // Tắt luôn NavMeshAgent

        GetComponent<EnemyAI>().enabled = false; // Vô hiệu hóa AI

        // 🔒 Khóa xoay & di chuyển vật lý để tránh lỗi
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints.FreezeAll;
        }
        GameManager.Instance.OnEnemyDied(this);

        // ✅ Cập nhật nhiệm vụ khi giết Enemy
        MissionData mission = MissionManager.Instance.GetCurrentMission();
        if (mission != null && mission.missionType == MissionData.MissionType.KillPlayer)
        {
            Debug.Log("MISSION SUCCES: KILL PLAYER !!");
            MissionManager.Instance.UpdateProgress();
        }
    }


}

