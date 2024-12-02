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

    private Renderer renderer;
    private NavMeshAgent agent;
    private float stateTimer;
    private float shotTimer;
    private bool isTired = false;
    private bool canSeePlayer = false;
    private bool isFleeing = false; //Comrpobar que est� huyendo

    public int health = 6;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        renderer = GetComponent<Renderer>();

        if (agent == null)
        {
            Debug.LogError("NavMeshAgent no encontrado en el objeto.");
            return;
        }

        // L�gica que ya ten�amos en el script de bullet, pero se a�ade para evitar conflictos
        int bulletLayer = gameObject.layer;
        if (bulletLayer == LayerMask.NameToLayer("BulletEnemy"))
        {
            Physics2D.IgnoreLayerCollision(bulletLayer, LayerMask.NameToLayer("Enemy"));
            Physics2D.IgnoreLayerCollision(bulletLayer, LayerMask.NameToLayer("BulletPlayer"));
        }

        // Configuraci�n del NavMeshAgent para movimiento ligero
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

        // Que el enemigo vuelva a la normalidad al estar activo
        if (renderer != null)
        {
            renderer.material.color = Color.white;
        }

        if (stateTimer <= 0)
        {
            EnterTiredState();
            return;
        }

        if (Vector3.Distance(transform.position, player.position) <= detectionRadius)
        {
            agent.isStopped = false;
            FleeFromPlayer();
        }
        else
        {
            // El enemigo persigue al jugador si est� fuera del rango de detecci�n
            isFleeing = false;
            agent.isStopped = false;
            agent.SetDestination(player.position);

            if (canSeePlayer && !isFleeing)
            {
                // Solo se queda quieto si no est� huyendo Y tiene al jugador en la l�nea de visi�n
                agent.isStopped = true;
            }
        }
    }

    void EnterTiredState()
    {
        isTired = true;
        stateTimer = tiredDuration;
    }

    void FleeFromPlayer()
    {
        isFleeing = true; // Bandera para saber si el enemigo huye
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
        string[] layers = { "Player", "Obstacle" };
        LayerMask layerMask = LayerMask.GetMask(layers);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, (player.position - transform.position).normalized, Mathf.Infinity, layerMask);
        if (hit)
        {

            if (hit.rigidbody != null && hit.rigidbody.gameObject == player.gameObject)
            {
                //Debug.LogWarning("Seeing the player.");
            }
            return hit.rigidbody.gameObject == player.gameObject;
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
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            // Necesitamos poner la gravedad de las balas en 0, por eso se iban hacia abajo
            bullet.GetComponent<Rigidbody2D>().gravityScale = 0;

            rb.velocity = shootDirection * bulletSpeed;

            Debug.DrawRay(transform.position, shootDirection * 10, Color.yellow, 0.2f);
            shotTimer = shotCooldown;
        }
    }

    void VisualizeTiredState()
    {
        //Obtenemos el renderer desde el start para no tener que iniciarlo en cada m�todo 
        if (renderer != null)
        {
            renderer.material.color = Color.blue;
        }
        else
        {
            Debug.LogWarning("No se encontr� Renderer para cambiar el color del enemigo.");
        }
    }

    private void OnDrawGizmos()
    {
        if (player != null)
        {
            Gizmos.color = Color.red;
            // Gizmos.DrawLine(transform.position, player.position);
            Gizmos.DrawLine(transform.position, transform.position + (player.position - transform.position).normalized * detectionRadius);
        }
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerBullet"))
        {
            health--;

            if (health <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
