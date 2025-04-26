using UnityEngine;

public class AnimalHealth : MonoBehaviour
{
    public float maxHealth = 50f;
    private float currentHealth;
    private Animator animator;
    public bool isDead = false;
    private AnimalAI animalAI;

    public InteractableObjects.ObjectType objectType = InteractableObjects.ObjectType.Animal;

    public GameObject hitByAxeEffectPrefab;
    public GameObject hitByArrowEffectPrefab;

    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        animalAI = GetComponent<AnimalAI>();
    }

    public void TakeDamage(float damage, string weaponType)
    {
        if (isDead) return;

        currentHealth -= damage;

        // 🛠 Spawn effect theo weaponType
        if (weaponType == "axe" && hitByAxeEffectPrefab != null)
        {
            Instantiate(hitByAxeEffectPrefab, transform.position + Vector3.up * 1f, Quaternion.identity);
        }
        else if (weaponType == "arrow" && hitByArrowEffectPrefab != null)
        {
            Instantiate(hitByArrowEffectPrefab, transform.position + Vector3.up * 1f, Quaternion.identity);
        }

        // 🟰 AnimalHealth riêng biệt: bị đánh xong đuổi player
        if (animalAI != null)
        {
            animalAI.StartChasingPlayer();
        }

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            if (animator != null)
            {
                animator.SetTrigger("Hit");
            }
        }
    }

    void Die()
    {
        if (isDead) return;

        isDead = true;
        if (animator != null)
        {
            animator.SetBool("Die", true);
        }

        if (animalAI != null)
        {
            animalAI.OnDeath(); // 👉 ngừng đuổi khi chết
        }

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints.FreezeAll;
        }

        MissionData mission = MissionManager.Instance.GetCurrentMission();
        if (mission != null && mission.missionType == MissionData.MissionType.HuntAnimal)
        {
            Debug.Log("✅ MISSION PROGRESS: KILL ANIMAL");
            MissionManager.Instance.UpdateProgress();
        }

        Destroy(gameObject, 3f);
    }
}


