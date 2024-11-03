using UnityEngine;
using UnityEngine.AI;

public class AgentController : MonoBehaviour
{
    [SerializeField] Transform target;

    private  NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    void Update()
    {
        if (target != null) // Verifica que target no sea null
        {
            agent.SetDestination(target.position);
        }
        else
        {
            Debug.LogWarning("El objeto target ha sido destruido o no está asignado.");
        }
    }
}