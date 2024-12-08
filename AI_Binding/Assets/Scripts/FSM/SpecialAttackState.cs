using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialAttackState : BossState
{
    private Transform player;
    private GameObject boss;

    private float dashSpeed = 15f; // Velocidad del dash
    private GameObject projectilePrefab; // Prefab del proyectil para el ataque en área
    private Transform firePoint; // Punto de disparo del jefe

    public SpecialAttackState(BossStateMachine stateMachine, GameObject boss, Transform player, GameObject projectilePrefab, Transform firePoint)
        : base(stateMachine, boss)
    {
        this.player = player;
        this.boss = boss;
        this.projectilePrefab = projectilePrefab;
        this.firePoint = firePoint;
    }

    public override void Enter()
    {
        // Determinar si hacer un Dash Melee o un Área Proyectil
        Vector2 directionToPlayer = player.position - boss.transform.position;

        if (Mathf.Abs(directionToPlayer.x) > Mathf.Abs(directionToPlayer.y))
        {
            // Si el jugador está más cerca horizontalmente
            stateMachine.ChangeState(new DashMeleeState(stateMachine, boss, player, dashSpeed));
        }
        else
        {
            // Si el jugador está más cerca verticalmente
            stateMachine.ChangeState(new AreaProjectileState(stateMachine, boss, projectilePrefab, firePoint));
        }
    }
}
