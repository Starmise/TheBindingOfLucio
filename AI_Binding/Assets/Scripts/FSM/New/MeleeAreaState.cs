using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAreaState : BossState
{
    private Transform player;
    private float attackRange = 3f; // Rango del ataque en área
    private int damage = 20; // Daño del ataque
    private bool hasAttacked = false;

    public MeleeAreaState(BossStateMachine stateMachine, GameObject boss, Transform player)
        : base(stateMachine, boss)
    {
        this.player = player;
    }

    public override void Enter()
    {
        if (!hasAttacked)
        {
            PerformAreaAttack();
            hasAttacked = true;
        }
    }

    private void PerformAreaAttack()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(boss.transform.position, attackRange);
        foreach (var enemy in hitEnemies)
        {
            if (enemy.CompareTag("Player"))
            {
                // Aplicar daño al jugador
                ;
            }
        }
    }

    public override void UpdateLogic()
    {
    }
}
