using UnityEngine;

public class AxeDamage : MonoBehaviour
{
    public float damage = 40f;
    private bool hasHit = false;

    private void OnTriggerEnter(Collider other)
    {
        if (hasHit) return; // Ngăn không gây damage nhiều lần trong 1 đòn
        if (other.CompareTag("Enemy"))
        {
            EnemyHealth enemyHealth = other.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
                hasHit = true; // Đánh dấu đã gây damage
            }
        }
    }

    private void OnEnable()
    {
        hasHit = false; // Reset lại để có thể gây damage lần sau
    }
}


