using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;
    [SerializeField]
    private Slider healthBar;
    private Animator animator;
    public bool isDead = false;

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
        animator.speed = 0;  // Dừng Animator để không quay về Idle
    }


}

