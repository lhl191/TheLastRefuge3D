// File: Assets/MyGame/Scripts/Companions/PetFollowPlayer.cs

using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(Animator))]
public class PetFollowPlayer : MonoBehaviour
{
    public Transform player;
    public float moveSpeed = 2f;
    public float followDistance = 2f;

    private NavMeshAgent agent;
    private Animator animator;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        agent.speed = moveSpeed;

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance > followDistance)
        {
            agent.SetDestination(player.position);
            animator.SetBool("isMoving", true);
        }
        else
        {
            agent.ResetPath();
            animator.SetBool("isMoving", false);
        }
    }
}
