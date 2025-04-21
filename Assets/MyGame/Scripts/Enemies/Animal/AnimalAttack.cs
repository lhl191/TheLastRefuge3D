using UnityEngine;

public class GolemAttack : MonoBehaviour
{
    private Transform player;
    private AnimalAI aiAnimal;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        aiAnimal = GetComponent<AnimalAI>(); 
    }

    public void DealDamageToPlayer()
    {
        if (player == null || aiAnimal == null) return;

        float distance = Vector3.Distance(transform.position, player.position);
        if (distance <= aiAnimal.attackRange)
        {
            PlayerHealth ph = player.GetComponent<PlayerHealth>();
            if (ph != null)
            {
                ph.TakeDamage(aiAnimal.damage); 
            }
        }
    }
}
