using UnityEngine;
using UnityEngine.AI;

public class BossNavMesh : MonoBehaviour
{
    [SerializeField] public Transform player; // Target del jugador
    [SerializeField] private float tiredDuration = 5f;
    [SerializeField] private float activeDuration = 10f;
    [SerializeField] private float restDuration = 2f; // Tiempo de descanso al estar cansado
    [SerializeField] private Animator animator;

    private Renderer renderer;
    private NavMeshAgent agent;
    private float stateTimer;
    private float restTimer;
    private bool isTired = false;
    private bool isResting = false;

    public int health = 6;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        renderer = GetComponent<Renderer>();
        animator = GetComponent<Animator>();

        if (agent == null)
        {
            Debug.LogError("NavMeshAgent no encontrado en el objeto.");
            return;
        }

        agent.acceleration = 10f;
        agent.speed = 10f;
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        if (player == null)
        {
            Debug.LogError("El jugador no ha sido asignado en el inspector.");
        }

        stateTimer = activeDuration;
    }

    void Update()
    {

        if (player == null || agent == null)
            return;
        
        if (health > 0)
        {//a
            RotateTowardsPlayer();

            if (isTired)
            {
                HandleTiredState();
            }
            else if (isResting)
            {
                HandleRestingState();
            }
            else
            {
                HandleActiveState();
            }
        }
    }

    void RotateTowardsPlayer()
{
    Vector3 directionToPlayer = player.transform.position - transform.position;
    if (directionToPlayer.x > 0)
    {
        // Player is to the left
        transform.rotation = Quaternion.Euler(0, 180, 0);
    }
    else if (directionToPlayer.x <= 0)
    {
        // Player is to the right
        transform.rotation = Quaternion.Euler(0, 0, 0);
    }
}

    void HandleTiredState()
    {
        agent.isStopped = true;
        animator.SetBool("IsMoving", false);
        stateTimer -= Time.deltaTime;
        VisualizeTiredState();

        if (stateTimer <= 0)
        {
            isTired = false;
            isResting = true;
            restTimer = restDuration;
        }
    }

    void HandleRestingState()
    {
        agent.isStopped = true;
        restTimer -= Time.deltaTime;

        if (restTimer <= 0)
        {
            isResting = false;
            stateTimer = activeDuration;
        }

    }

    void HandleActiveState()
    {
        stateTimer -= Time.deltaTime;

        if (renderer != null)
        {
            renderer.material.color = Color.white;
        }

        if (stateTimer <= 0)
        {
            EnterTiredState();
            return;
        }

        agent.isStopped = false;
        agent.SetDestination(player.position);
        animator.SetBool("IsMoving", true);
    }

    void EnterTiredState()
    {
        isTired = true;
        stateTimer = tiredDuration;
    }

    void VisualizeTiredState()
    {
        if (renderer != null)
        {
            renderer.material.color = Color.blue;
        }
        else
        {
            Debug.LogWarning("No se encontró Renderer para cambiar el color del enemigo.");
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerBullet"))
        {
            health--;

            animator.SetBool("IsHurt", true);
            if (health <= 0)
            {
                Debug.Log("Boss defeated!");
                animator.SetBool("IsHurt", false);
                animator.SetBool("IsDead", true);
            }
        }
    }
}