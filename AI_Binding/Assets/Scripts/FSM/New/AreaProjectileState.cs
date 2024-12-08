using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaProjectileState : BossState
{
    private GameObject projectilePrefab;
    private Transform firePoint;
    private int projectileCount = 5; // Número de proyectiles por ráfaga
    private float spreadAngle = 45f; // Ángulo de separación entre proyectiles
    private bool hasAttacked = false;
    private Transform player;

    public AreaProjectileState(BossStateMachine stateMachine, GameObject boss, GameObject projectilePrefab, Transform firePoint)
        : base(stateMachine, boss)
    {
        this.projectilePrefab = projectilePrefab;
        this.firePoint = firePoint;
    }

    public override void Enter()
    {
        if (!hasAttacked)
        {
            FireProjectiles();
            hasAttacked = true;
        }
    }

    private void FireProjectiles()
    {
        for (int i = 0; i < projectileCount; i++)
        {
            float angle = -spreadAngle / 2 + (spreadAngle / (projectileCount - 1)) * i;
            Quaternion rotation = Quaternion.Euler(0, 0, angle);
            GameObject projectile = GameObject.Instantiate(projectilePrefab, firePoint.position, rotation);
            projectile.GetComponent<Rigidbody2D>().velocity = rotation * Vector2.right * 10f; // Velocidad del proyectil
        }
    }

    public override void UpdateLogic()
    {
        // Evaluar si está en rango de disparo para activar Ultimate
        float distanceToPlayer = Vector2.Distance(player.position, boss.transform.position);
        if (distanceToPlayer > 5f && distanceToPlayer <= 10f)
        {
            stateMachine.ChangeState(new UltimateState(stateMachine, boss, player, projectilePrefab));
        }
    }
}
