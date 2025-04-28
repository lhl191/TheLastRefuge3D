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

    [Header("Hit Effect Prefab")]
    public GameObject hitEffectPrefab;
    public Vector3 hitEffectOffset = Vector3.up;

    [Header("Hit Sound")]
    public AudioClip hitSound;

    void Start()
    {
        currentHealth = maxHealth;
        healthBar.maxValue = maxHealth;
        healthBar.value = currentHealth;
        animator = GetComponent<Animator>();
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        healthBar.value = currentHealth;

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
}
