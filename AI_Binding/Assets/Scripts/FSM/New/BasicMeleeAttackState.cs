using UnityEngine;

public class BasicMeleeAttackState : BossState
{
    private Transform player;
    private float meleeRange = 2f;
    private float attackCooldown = 1.5f;
    private float cooldownTimer = 0f;

    public BasicMeleeAttackState(BossStateMachine stateMachine, GameObject boss, Transform player)
        : base(stateMachine, boss)
    {
        this.player = player;
    }

    public override void Enter()
    {
        cooldownTimer = attackCooldown; // Reinicia el temporizador
    }

    public override void UpdateLogic()
    {
        // Verifica si el jugador está fuera del rango melee
        float distanceToPlayer = Vector2.Distance(player.position, boss.transform.position);
        if (distanceToPlayer > 5f) // Cambiar a ataque de proyectiles si el jugador se aleja
        {
            stateMachine.ChangeState(new BasicProjectileAttackState(stateMachine, boss, player, null, null));
            return;
        }

        // Continuar con el ataque melee
        cooldownTimer -= Time.deltaTime;
        if (cooldownTimer <= 0f)
        {
            MeleeAttack();
            cooldownTimer = attackCooldown; // Reinicia el temporizador tras el ataque
        }
    }

    private void MeleeAttack()
    {
        Debug.Log("El jefe realiza un ataque cuerpo a cuerpo.");
        // Aquí puedes añadir animaciones y lógica de daño
    }
}