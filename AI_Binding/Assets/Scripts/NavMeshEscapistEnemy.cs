using UnityEngine;
using UnityEngine.AI;

public class NavMeshEscapistEnemy : MonoBehaviour
{
    [SerializeField] private Transform player; // Target del jugador
    [SerializeField] private float detectionRadius = 10f;
    [SerializeField] private float fleeDistance = 5f;
    [SerializeField] private float tiredDuration = 5f;
    [SerializeField] private float activeDuration = 10f;
    [SerializeField] private float shotCooldown = 2f;
    [SerializeField] private float shootingAccuracyWhenTired = 0.5f;

    [Header("Bullet Values")]
    [SerializeField] private float bulletDelay = 0.2f;
    [SerializeField] private float bulletSpeed = 3.0f;
    [SerializeField] private GameObject bulletPrefab;

    private NavMeshAgent agent;
    private float stateTimer;
    private float shotTimer;
    private bool isTired = false;
    private bool canSeePlayer = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (agent == null)
        {
            Debug.LogError("NavMeshAgent no encontrado en el objeto.");
            return;
        }

        // Configuración del NavMeshAgent para movimiento ligero
        agent.acceleration = 10f;
        agent.speed = 2f;
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
        if (player == null || agent == null|| bulletPrefab == null)
            return;

        canSeePlayer = HasLineOfSightToPlayer();

        if (isTired)
        {
            HandleTiredState();
        }
        else
        {
            HandleActiveState();
        }
    }

    void HandleTiredState()
    {
        agent.isStopped = true;
        stateTimer -= Time.deltaTime;
        VisualizeTiredState();

        if (stateTimer <= 0)
        {
            isTired = false;
            stateTimer = activeDuration;
        }
        else
        {
            ShootAtPlayer(shootingAccuracyWhenTired);
        }
    }

    void HandleActiveState()
    {
        stateTimer -= Time.deltaTime;
        shotTimer -= Time.deltaTime;

        if (stateTimer <= 0)
        {
            EnterTiredState();
            return;
        }

        if (Vector3.Distance(transform.position, player.position) <= detectionRadius)
        {
            if (canSeePlayer)
            {
                agent.isStopped = true;
            }
            else
            {
                agent.isStopped = false;
                agent.SetDestination(player.position);
            }
        }
        else
        {
            FleeFromPlayer();
        }
    }

    void EnterTiredState()
    {
        isTired = true;
        stateTimer = tiredDuration;
    }

    void FleeFromPlayer()
    {
        Vector3 fleeDirection = (transform.position - player.position).normalized;
        Vector3 fleeTarget = transform.position + fleeDirection * fleeDistance;

        if (NavMesh.SamplePosition(fleeTarget, out NavMeshHit hit, fleeDistance, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
            Debug.DrawLine(transform.position, hit.position, Color.red, 0.5f);
        }
        else
        {
            agent.SetDestination(player.position);
        }
    }

    bool HasLineOfSightToPlayer()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, (player.position - transform.position).normalized, out hit, detectionRadius))
        {
            return hit.transform == player;
        }
        return false;
    }

    void ShootAtPlayer(float accuracy)
    {
        if (shotTimer <= 0 && player != null)
        {
            Vector3 shootDirection = (player.position - transform.position).normalized;
            shootDirection += Random.insideUnitSphere * (1f - accuracy);
            GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            Rigidbody2D rb = bullet.AddComponent<Rigidbody2D>();

            Debug.DrawRay(transform.position, shootDirection * 10, Color.yellow, 0.2f);
            shotTimer = shotCooldown;
        }
    }

    void VisualizeTiredState()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = Color.blue;
        }
        else
        {
            Debug.LogWarning("No se encontró Renderer para cambiar el color del enemigo.");
        }
    }
}
