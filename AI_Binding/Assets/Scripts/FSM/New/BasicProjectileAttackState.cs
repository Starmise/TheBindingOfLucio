using UnityEngine;

public class BasicProjectileAttackState : BossState
{
    private Transform player;
    private float attackCooldown = 2f;
    private float cooldownTimer = 0f;
    private GameObject projectilePrefab;
    private Transform firePoint;

    public BasicProjectileAttackState(BossStateMachine stateMachine, GameObject boss, Transform player, GameObject projectilePrefab, Transform firePoint)
        : base(stateMachine, boss)
    {
        this.player = player;
        this.projectilePrefab = projectilePrefab;
        this.firePoint = firePoint;
    }

    public override void Enter()
    {
        cooldownTimer = attackCooldown; // Reinicia el temporizador
    }

    public override void UpdateLogic()
    {
        // Verifica si el jugador está fuera del rango de proyectil
        float distanceToPlayer = Vector2.Distance(player.position, boss.transform.position);
        if (distanceToPlayer <= 5f) // Cambiar a ataque melee si el jugador está cerca
        {
            stateMachine.ChangeState(new BasicMeleeAttackState(stateMachine, boss, player));
            return;
        }

        // Continuar con el ataque de proyectiles
        cooldownTimer -= Time.deltaTime;
        if (cooldownTimer <= 0f)
        {
            ShootProjectiles();
            cooldownTimer = attackCooldown; // Reinicia el temporizador tras el ataque
        }
    }

    private void ShootProjectiles()
    {
        // Instancia dos proyectiles dirigidos hacia el jugador
        for (int i = 0; i < 2; i++)
        {
            GameObject projectile = GameObject.Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
            Vector2 direction = (player.position - firePoint.position).normalized;
            projectile.GetComponent<Rigidbody2D>().velocity = direction * 10f; // Velocidad del proyectil
        }
    }

    private bool IsPlayerInVisionRange()
    {
        float distanceToPlayer = Vector2.Distance(player.position, boss.transform.position);
        return distanceToPlayer > 5f && distanceToPlayer <= 10f; // Rango de visión y distancia mínima
    }
}
