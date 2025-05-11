using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;
    private Animator animator;
    public bool isDead = false;

    public GameObject hitByAxeEffectPrefab;
    public GameObject hitByArrowEffectPrefab;

    [Header("Hit Sound")]
    public AudioClip hitSound;

    [Header("UI")]
    public GameObject healthBarUIPrefab; // Prefab chứa Canvas + Slider
    private GameObject healthBarUIInstance;
    private Slider healthSlider;

    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();

        if (healthBarUIPrefab != null)
        {
            healthBarUIInstance = Instantiate(healthBarUIPrefab, transform.position + Vector3.up * 2f, Quaternion.identity);
            healthSlider = healthBarUIInstance.GetComponentInChildren<Slider>();
            healthSlider.value = 1f;
        }
    }

    void Update()
    {
        if (healthBarUIInstance != null)
        {
            healthBarUIInstance.transform.position = transform.position + Vector3.up * 2f;
            healthBarUIInstance.transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);
        }
    }

    public void TakeDamage(float damage, string weaponType)
    {
        if (isDead) return;

        currentHealth -= damage;

        if (healthSlider != null)
        {
            healthSlider.value = currentHealth / maxHealth;
        }

        if (weaponType == "axe" && hitByAxeEffectPrefab != null)
        {
            Instantiate(hitByAxeEffectPrefab, transform.position + Vector3.up * 1f, Quaternion.identity);
        }
        else if (weaponType == "arrow" && hitByArrowEffectPrefab != null)
        {
            Instantiate(hitByArrowEffectPrefab, transform.position + Vector3.up * 1f, Quaternion.identity);
        }

        if (hitSound != null)
        {
            AudioSource.PlayClipAtPoint(hitSound, transform.position);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            animator.SetTrigger("Hit");
        }
    }

    void Die()
    {
        if (isDead) return;

        isDead = true;

        if (healthBarUIInstance != null)
        {
            Destroy(healthBarUIInstance);
        }

        animator.ResetTrigger("Attack");
        animator.ResetTrigger("Hit");
        animator.SetBool("Die", true);

        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
            agent.enabled = false;
        }

        GetComponent<EnemyAI>().enabled = false;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints.FreezeAll;
        }

        GameManager.Instance.OnEnemyDied(this);

        MissionData mission = MissionManager.Instance.GetCurrentMission();
        if (mission != null && mission.missionType == MissionData.MissionType.KillPlayer)
        {
            Debug.Log("MISSION SUCCESS: KILL PLAYER !!");
            MissionManager.Instance.UpdateProgress();
        }
    }
}
