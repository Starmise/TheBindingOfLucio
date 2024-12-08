using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashMeleeState : BossState
{
    private Transform player;
    private float dashSpeed;
    private Vector2 dashDirection;
    private float dashDuration = 0.5f; // Duración del dash
    private float dashTimer;

    public DashMeleeState(BossStateMachine stateMachine, GameObject boss, Transform player, float dashSpeed)
        : base(stateMachine, boss)
    {
        this.player = player;
        this.dashSpeed = dashSpeed;
    }

    public override void Enter()
    {
        Vector2 directionToPlayer = player.position - boss.transform.position;
        dashDirection = new Vector2(Mathf.Sign(directionToPlayer.x), 0); // Dirección horizontal
        dashTimer = dashDuration;
    }

    public override void UpdateLogic()
    {
        dashTimer -= Time.deltaTime;

        if (dashTimer <= 0)
        {
            // Evaluar si está en rango melee para el ataque de área
            float distanceToPlayer = Vector2.Distance(player.position, boss.transform.position);
            if (distanceToPlayer <= 5f)
            {
                stateMachine.ChangeState(new MeleeAreaState(stateMachine, boss, player));
            }
        }
    }


    public override void UpdatePhysics()
    {
        boss.GetComponent<Rigidbody2D>().velocity = dashDirection * dashSpeed;
    }

    public override void Exit()
    {
        boss.GetComponent<Rigidbody2D>().velocity = Vector2.zero; // Detener el movimiento
    }
}
