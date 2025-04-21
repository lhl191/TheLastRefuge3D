using UnityEngine;

public class ArrowDamage : MonoBehaviour
{
    public float damage = 25f;
    private bool hasHit = false;

    void OnTriggerEnter(Collider other)
    {
        if (hasHit) return;

        if (other.CompareTag("Enemy"))
        {
            EnemyHealth enemyHealth = other.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
                hasHit = true;
            }
        }
        else if (other.CompareTag("Boss"))
        {
            BossHealth bossHealth = other.GetComponent<BossHealth>();
            if (bossHealth != null)
            {
                bossHealth.TakeDamage(damage);
                hasHit = true;
            }
        }
        if (other.CompareTag("Animal"))
        {
            AnimalHealth animalHealth = other.GetComponent<AnimalHealth>();
            if (animalHealth != null)
            {
                animalHealth.TakeDamage(damage);
                hasHit = true;
            }
        }

        if (hasHit)
        {
            GetComponent<Collider>().enabled = false;
            Destroy(gameObject);
        }
    }
}
