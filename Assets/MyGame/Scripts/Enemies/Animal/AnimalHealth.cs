using UnityEngine;
using UnityEngine.UI;

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

    [Header("Hit Sound")]
    public AudioClip hitSound;

    [Header("UI")]
    public GameObject healthBarUIPrefab; 
    private GameObject healthBarUIInstance;
    private Slider healthSlider;
    public Transform healthBarAnchor;

    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        animalAI = GetComponent<AnimalAI>();

        if (healthBarUIPrefab != null && healthBarAnchor != null)
        {
            healthBarUIInstance = Instantiate(healthBarUIPrefab, healthBarAnchor.position, Quaternion.identity);
            healthSlider = healthBarUIInstance.GetComponentInChildren<Slider>();
            healthSlider.value = 1f;
        }
    }

    void Update()
    {
        if (healthBarUIInstance != null && healthBarAnchor != null)
        {
            healthBarUIInstance.transform.position = healthBarAnchor.position;
            healthBarUIInstance.transform.forward = Camera.main.transform.forward;
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
            animator?.SetTrigger("Hit");
        }
    }

    void Die()
    {
        if (isDead) return;

        isDead = true;
        animator?.SetBool("Die", true);

        if (healthBarUIInstance != null)
        {
            Destroy(healthBarUIInstance);
        }

        animalAI?.OnDeath();

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
