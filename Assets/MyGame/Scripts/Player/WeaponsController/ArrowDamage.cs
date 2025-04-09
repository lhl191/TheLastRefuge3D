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
            }
        }
        GetComponent<Collider>().enabled = false;
        Destroy(gameObject);
    }
}

