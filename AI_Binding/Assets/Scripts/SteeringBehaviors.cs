using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// Definimos tres tipos de enemigos mediante un Enum
public enum EnemyType
{
    Flee,
    Heavy,
    HeavyOG,
    Torret,
    HeavyWithVision
}

public class SteeringBehaviors : EnemyMovement
{
    protected Rigidbody2D rb = null;

    private Vector2 TargetPos = Vector2.zero;

    public EnemyType enemyType;

    private bool isFleeing = false;
    private float fleeTime = 0.0f;

    private float timeToShoot = 3.0f;
    private float lastBullet;
    private bool isShooting = false;
    [Space(3)]
    [Header("Bullet Values")]
    [SerializeField] private float bulletDelay = 0.2f;
    [SerializeField] private float bulletSpeed = 3.0f;
    public GameObject bulletPrefab;

    private bool playerInRoom = false;
    private float timeInRoom = 0.0f;
    [Space(3)]
    [Header("Room Bounds")]
    // Coordenadas inferior izquierda, y superior derecha del cuarto. De esta forma
    // podemos comparar las 4 coordenanas con solo dos variables.
    public Vector2 roomMinBounds;
    public Vector2 roomMaxBounds;

    [Space(3)]
    [Header("Torret Enemy")]
    public float Range;
    private bool Detected = false;
    public GameObject active;
    public GameObject Turret;

    private NavMeshAgent agent;

    private bool playerDetected = false;
    private FieldOfView fov;
    private bool isChasing = false;
    private float chaseTime = 0f;
    [Space(3)]
    [Header("FOV Enemy")]
    public float maxChaseDuration = 4f;
    [SerializeField] private Vector2 startPosition;

    void Start()
    {
        // Debug.Log("SteeringBehaviors Start ejecut�ndose");
        base.Start();
        rb = GetComponent<Rigidbody2D>();
        agent = GetComponent<NavMeshAgent>();

        fov = GetComponentInChildren<FieldOfView>();

        if (agent != null)
        {
            agent.updateRotation = false;
            agent.updateUpAxis = false;
            agent.enabled = false;
        }

        startPosition = transform.position;
    }

    void Update()
    {
        if (targetGameObject == null)
        {
            return;
        }

        // Si la posici�n del jugador en "x" y "y", son mayor o iguales al m�nimo y
        // menores o iguales al m�ximo de los l�mites, est� dentro del cuarto.
        if (targetGameObject.transform.position.x >= roomMinBounds.x &&
            targetGameObject.transform.position.x <= roomMaxBounds.x &&
            targetGameObject.transform.position.y >= roomMinBounds.y &&
            targetGameObject.transform.position.y <= roomMaxBounds.y)
        {
            playerInRoom = true;
        }
        else
        {
            playerInRoom = false;
        }

        // Seleccionar el tipo de enemigo pasando el par�metro del enum a un switch
        switch (enemyType)
        {
            case EnemyType.Flee:
                FleeEnemyLogic();
                break;
            case EnemyType.Heavy:
                HeavyEnemyLogic();
                break;
            case EnemyType.Torret:
                TurretEnemyLogic();
                break;
            case EnemyType.HeavyWithVision:
                HeavyWithVisionLogic();
                break;
            case EnemyType.HeavyOG:
                HeavyEnemyOGLogic();
                break;
            default:
                Debug.LogError("No se ha definido este tipo de enemigo.");
                break;
        }

        //Debug.Log("Velocidad del enemigo: " + rb.velocity.magnitude);
    }

    void FleeEnemyLogic()
    {
        if (targetGameObject == null)
        {
            return;
        }

        if (bulletPrefab == null)
            Debug.LogError("bulletPrefab no est� asignado correctamente en runtime.");



        Vector2 PosToTarget = -PuntaMenosCola(targetGameObject.transform.position, transform.position);

        // Solo aplica fuerza de velocidad si no est� disparando
        if (!isShooting)
        {
            rb.AddForce(PosToTarget.normalized * maxAcceleration, ForceMode2D.Force);
            rb.velocity = Vector2.ClampMagnitude(rb.velocity, maxSpeed);
        }

        // Iniciar l�gica para escapar
        if (!isFleeing)
        {
            isFleeing = true;
        }

        fleeTime += Time.deltaTime;

        // Si pasaron 3 segundos desde que huye, dispara
        if (fleeTime >= timeToShoot)
        {
            // Detener el movimiento antes de disparar
            rb.velocity = Vector2.zero;

            if (Time.time > lastBullet + bulletDelay)
            {
                isShooting = true;
                Shooting(-PosToTarget.x, -PosToTarget.y);
                lastBullet = Time.time;

                // Reiniciar el tiempo de escape y marcar como no disparando despu�s del disparo
                fleeTime = 0.0f;
                isShooting = false;
            }
        }
    }

    void Shooting(float x, float y)
    {
        GameObject bullet = Instantiate(bulletPrefab, transform.position, transform.rotation) as GameObject;
        bullet.GetComponent<Rigidbody2D>().gravityScale = 0;

        bullet.GetComponent<Rigidbody2D>().velocity = new Vector2(
            (x < 0) ? Mathf.Floor(x) * bulletSpeed : Mathf.Ceil(x) * bulletSpeed,
            (y < 0) ? Mathf.Floor(y) * bulletSpeed : Mathf.Ceil(y) * bulletSpeed
        );
    }

    void HeavyEnemyLogic()
    {
        if (targetGameObject == null)
        {
            return;
        }

        if (playerInRoom)
        {
            if (!agent.enabled)
            {
                agent.enabled = true;
                agent.SetDestination(targetGameObject.transform.position);
            }

            // Usa el siguiente punto en el camino calculado por el agente en NavMesh
            Vector2 nextPoint = agent.steeringTarget;
            Vector2 PosToTarget = PuntaMenosCola(nextPoint, transform.position);

            timeInRoom += Time.deltaTime;
            rb.AddForce(PosToTarget.normalized * maxAcceleration * timeInRoom, ForceMode2D.Force);
            rb.velocity = Vector2.ClampMagnitude(rb.velocity, maxSpeed);
        }
        // Probando me di cuenta de que si el jugador deja el cuarto se ve raro, as� que
        // implementamos una l�gica sencilla para que desacelere cada segundo.
        else
        {
            // Desactiva el agente si el jugador sale de la habitaci�n
            if (agent.enabled)
            {
                agent.enabled = false;
            }

            float decelerationRate = 2.5f;
            // Se usa Lerp para pasar de la velocidad actual del enemigo a 0, en un lapso
            // determinado por el rango de desaceleraci�n por cada segundo que pasa.
            rb.velocity = Vector2.Lerp(rb.velocity, Vector2.zero, decelerationRate * Time.deltaTime);

            // Si la velocidad ya es muy poca, se pasa a 0 autom�ticamente. Se usa .magnitud 
            // porque velocity es un vector y necesitamos una magnitud para comparar.
            if (rb.velocity.magnitude <= 0.5f)
            {
                rb.velocity = Vector2.zero;
            }
        }
    }

    void HeavyEnemyOGLogic()
    {
        if (playerInRoom)
        {
            timeInRoom += Time.deltaTime;
            Vector2 PosToTarget = PuntaMenosCola(targetGameObject.transform.position, transform.position);
            rb.AddForce(PosToTarget.normalized * maxAcceleration * timeInRoom, ForceMode2D.Force);

            rb.velocity = Vector2.ClampMagnitude(rb.velocity, maxSpeed);
        }
        else
        {
            float decelerationRate = 2.5f;
            rb.velocity = Vector2.Lerp(rb.velocity, Vector2.zero, decelerationRate * Time.deltaTime);
            if (rb.velocity.magnitude <= 0.5f)
            {
                rb.velocity = Vector2.zero;
            }
        }
    }

    void TurretEnemyLogic()
    {
        active.SetActive(false);

        // Nuevamente PuntaMenosCola, pero cambiamos el nombre de la variable por su nuevo uso
        Vector2 directionToTarget = PuntaMenosCola(targetGameObject.transform.position, transform.position);

        // Necesitamos ignorar la capa de la torreta, por lo que declaramos una layer para
        // que el raycast detecte �nicamente objetos el la Layer de Player.
        LayerMask mask = LayerMask.GetMask("Player");

        // Raycast hacia el jugador
        RaycastHit2D rayInfo = Physics2D.Raycast(transform.position, directionToTarget, Range, mask);

        Debug.DrawRay(transform.position, directionToTarget * Range, Color.red);

        // Comprobar si el raycast ha detectado al jugador
        if (rayInfo.collider != null)
        {
            // Debug.Log("Detect� al objeto: " + rayInfo.collider.gameObject.name);

            if (rayInfo.collider.CompareTag("Player"))
            {
                Detected = true;
            }
            else
            {
                Detected = false;
            }
        }

        if (Detected)
        {
            active.SetActive(true);
            Turret.transform.up = directionToTarget;

            if (Time.time > lastBullet + bulletDelay)
            {
                Shooting(directionToTarget.x, directionToTarget.y);
                lastBullet = Time.time;
            }
        }
    }

    void HeavyWithVisionLogic()
    {
        // Revisa si el jugador est� dentro del FOV
        playerDetected = fov.CheckFieldOfView();
        if (playerDetected)
        {
            StartChase();
        }

        if (isChasing)
        {
            ChasePlayer();

            chaseTime += Time.deltaTime;

            // Si ya persigu�i al jugador por 4 segundos, el enemigo vuelve a la posici�n inicial
            if (chaseTime >= maxChaseDuration)
            {
                StopChase();
            }
        }
        else
        {
            ReturnToStartPosition();
        }
    }

    void StartChase()
    {
        if (!isChasing)
        {
            isChasing = true;
            chaseTime = 0f;
            agent.enabled = true;
            Debug.Log("Iniciando persecuci�n al jugador.");
        }
    }

    void ChasePlayer()
    {
        if (agent.enabled)
        {
            agent.SetDestination(targetGameObject.transform.position);
        }
    }

    void StopChase()
    {
        isChasing = false;
        agent.enabled = false;
        Debug.Log("Deteniendo persecuci�n. Regresando a la posici�n inicial.");
    }

    void ReturnToStartPosition()
    {
        if (Vector2.Distance(transform.position, startPosition) > 0.1f)
        {
            if (!agent.enabled)
            {
                agent.enabled = true;
                agent.SetDestination(startPosition);
            }
        }
        else
        {
            // Si ya estamos en la posici�n inicial, desactiva el agente para detener el movimiento
            if (agent.enabled)
            {
                agent.enabled = false;
            }
        }
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

    private void OnDrawGizmosSelected()
    {
        // Establece el color del Gizmo
        Gizmos.color = Color.red;

        // Dibuja un c�rculo representando el rango de la torreta
        Gizmos.DrawWireSphere(transform.position, Range);
    }
}
