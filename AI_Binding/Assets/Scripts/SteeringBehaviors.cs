using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Definimos tres tipos de enemigos mediante un Enum
public enum EnemyType
{
    Flee,
    Heavy,
    Torret
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
    public GameObject Turret;

    void Start()
    {
        Debug.Log("SteeringBehaviors Start ejecutándose");
        base.Start();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (targetGameObject == null)
        {
            return;
        }

        // Si la posición del jugador en "x" y "y", son mayor o iguales al mínimo y
        // menores o iguales al máximo de los límites, está dentro del cuarto.
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

        // Seleccionar el tipo de enemigo pasando el parámetro del enum a un switch
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

        Vector2 PosToTarget = -PuntaMenosCola(targetGameObject.transform.position, transform.position);

        // Solo aplica fuerza de velocidad si no está disparando
        if (!isShooting)
        {
            rb.AddForce(PosToTarget.normalized * maxAcceleration, ForceMode2D.Force);
            rb.velocity = Vector2.ClampMagnitude(rb.velocity, maxSpeed);
        }

        // Iniciar lógica para escapar
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

                // Reiniciar el tiempo de escape y marcar como no disparando después del disparo
                fleeTime = 0.0f;
                isShooting = false; 
            }
        }
    }

    void Shooting(float x, float y)
    {
        GameObject bullet = Instantiate(bulletPrefab, transform.position, transform.rotation) as GameObject;
        bullet.AddComponent<Rigidbody2D>().gravityScale = 0;

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

        if(playerInRoom)
        {
            timeInRoom += Time.deltaTime;
            Vector2 PosToTarget = PuntaMenosCola(targetGameObject.transform.position, transform.position);

            // Multiplicamos por el tiempo que pase en la habitación el jugador
            rb.AddForce(PosToTarget.normalized * maxAcceleration * timeInRoom, ForceMode2D.Force);

            rb.velocity = Vector2.ClampMagnitude(rb.velocity, maxSpeed);
        }
        // Probando me di cuenta de que si el jugador deja el cuarto se ve raro, así que
        // implementamos una lógica sencilla para que desacelere cada segundo.
        else
        {
            float decelerationRate = 2.5f;
            // Se usa Lerp para pasar de la velocidad actual del enemigo a 0, en un lapso
            // determinado por el rango de desaceleración por cada segundo que pasa.
            rb.velocity = Vector2.Lerp(rb.velocity, Vector2.zero, decelerationRate * Time.deltaTime);

            // Si la velocidad ya es muy poca, se pasa a 0 automáticamente. Se usa .magnitud 
            // porque velocity es un vector y necesitamos una magnitud para comparar.
            if (rb.velocity.magnitude <= 0.5f)
            {
                rb.velocity = Vector2.zero;
            }
        }
    }

    void TurretEnemyLogic()
    {
        // Nuevamente PuntaMenosCola, pero cambiamos el nombre de la variable por su nuevo uso
        Vector2 directionToTarget = PuntaMenosCola(targetGameObject.transform.position, transform.position);

        // Necesitamos ignorar la capa de la torreta, por lo que declaramos una layer para
        // que el raycast detecte únicamente objetos el la Layer de Player.
        LayerMask mask = LayerMask.GetMask("Player");

        // Raycast hacia el jugador
        RaycastHit2D rayInfo = Physics2D.Raycast(transform.position, directionToTarget, Range, mask);

        Debug.DrawRay(transform.position, directionToTarget * Range, Color.red);

        // Comprobar si el raycast ha detectado al jugador
        if (rayInfo.collider != null)
        {
            // Debug.Log("Detectó al objeto: " + rayInfo.collider.gameObject.name);

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
            Turret.transform.up = directionToTarget; 

            if (Time.time > lastBullet + bulletDelay)
            {
                Shooting(directionToTarget.x, directionToTarget.y); 
                lastBullet = Time.time;
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
}
