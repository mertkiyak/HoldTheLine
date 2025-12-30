using UnityEngine;
using UnityEngine.AI;

public class NPCFollower : MonoBehaviour
{
    public Transform player;
    private NavMeshAgent agent;

    public float updateSpeed = 0.1f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        PlayerController playerController = FindAnyObjectByType<PlayerController>();
        if (playerController != null)
        {
            player = playerController.transform;
        }
    }

    void Update()
    {
        if (player != null && agent != null && agent.enabled && agent.isOnNavMesh)
        {
           
            agent.SetDestination(player.position);
        }
    }
}