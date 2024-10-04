using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringBehaviors : EnemyMovement
{
    protected Rigidbody2D rb = null;

    public Vector2 TargetPos = Vector2.zero;

    // Definimos tres tipos de enemigos mediante booleanos, dependiendo de cuál
    // este activo, se activará su lógica correspondiente.
    public bool fleeEnemy = false;
    public bool heavyEnemy = false;
    public bool torretEnemy = false;

    public bool isFleeing = false;
    private float fleeTime = 0.0f;

    [SerializeField] private float timeToShoot = 3.0f;
    [SerializeField] private float bulletSpeed = 3.0f;
    private float lastBullet;
    [SerializeField] private float bulletDelay = 0.2f;
    [SerializeField] private GameObject bulletPrefab;
    private bool isShooting = false;

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

        if (fleeEnemy)
        {
            FleeEnemyLogic();
        }
        else if (heavyEnemy)
        {
            // Método para el enemigo pesado
        }
        else if (torretEnemy)
        {
            // Método para el enemigo de tipo torreta
        }

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
                isShooting = true;  // Marca que está disparando
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
}
