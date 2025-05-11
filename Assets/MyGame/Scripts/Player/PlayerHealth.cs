using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;

    [SerializeField] private Slider healthBar;
    private Animator animator;
    public bool isDead = false;
    public bool isReady = false;

    [Header("Hit Effect Prefab")]
    public GameObject hitEffectPrefab;
    public Vector3 hitEffectOffset = Vector3.up;

    [Header("Hit Sound")]
    public AudioClip hitSound;

    void Start()
    {
        animator = GetComponent<Animator>();
        StartCoroutine(WaitForHealthBarAndReset());
    }

    IEnumerator WaitForHealthBarAndReset()
    {
        int retries = 10;
        while (healthBar == null && retries-- > 0)
        {
            var go = GameObject.FindWithTag("HealthBar");
            if (go != null)
                healthBar = go.GetComponent<Slider>();
            yield return new WaitForSeconds(0.1f);
        }

        if (healthBar == null)
            Debug.LogWarning("❌ PlayerHealth: healthBar is NULL after retries");

        ResetHealth();
        isReady = true;
    }

    public void AssignHealthBar(Slider slider)
    {
        healthBar = slider;
    }

    public void TakeDamage(float damage)
    {
        if (!isReady)
        {
            Debug.LogWarning("⚠️ PlayerHealth: Not ready yet, skipping damage.");
            return;
        }

        if (isDead) return;

        currentHealth -= damage;

        if (healthBar != null)
            healthBar.value = currentHealth;
        else
            Debug.LogWarning("❌ PlayerHealth: healthBar is NULL in TakeDamage!");

        animator.SetBool("isHit", true);

        if (hitEffectPrefab != null)
        {
            GameObject effectInstance = Instantiate(
                hitEffectPrefab,
                transform.position + hitEffectOffset,
                Quaternion.identity
            );
            Destroy(effectInstance, 2f);
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
            StartCoroutine(ResetHitAnimation());
        }
    }

    IEnumerator ResetHitAnimation()
    {
        yield return new WaitForSeconds(0.3f);
        animator.SetBool("isHit", false);
    }

    void Die()
    {
        isDead = true;
        animator.SetBool("Die", true);

        GetComponent<ThirdPersonController>().enabled = false;
        animator.applyRootMotion = true;
        StartCoroutine(StopAnimatorAfterDeath());

        GameManager.Instance.OnPlayerDied();
        this.enabled = false;
    }

    IEnumerator StopAnimatorAfterDeath()
    {
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        animator.speed = 0;
    }

    public void ResetHealth()
    {
        isDead = false;
        currentHealth = maxHealth;

        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }

        animator.SetBool("Die", false);
        animator.speed = 1f;
        GetComponent<ThirdPersonController>().enabled = true;
        animator.applyRootMotion = false;
        this.enabled = true;
    }
}