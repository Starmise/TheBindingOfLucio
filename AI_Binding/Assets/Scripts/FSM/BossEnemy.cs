using UnityEngine;

public class BossEnemy : BaseEnemy
{
    public int NumberOfBasicAttacksBeforeExit = 3;
    public float BasicAttackRange = 3.0f;

    // Cuánto tiempo debe durar haciendo el ataque de área en el Subestado de Ataque de área de MeleeState.
    public float AreaAttackTime = 2.0f;
    public float AreaAttackBuildupTime = 1.0f;
    public float AreaAttackCooldownTime = 1.0f;
    public float AreaAttackRange = 6.0f;

    public float DashAttackTime = 2.0f;
    public float DashBuildupTime = 0.3f;
    public float DashCooldownTime = 1.0f;

    public bool EnableDebug = true;

    public float RangedAttackRange = 10.0f; // Rango de ataque para el RangedState
    public float BasicAttackTime = 1.0f; // Tiempo entre ataques básicos en RangedState
    public int BasicAtksBeforeAreaAttack = 2; // Ataques básicos antes de AreaAttack
    public float UltimateAttackDuration = 2.0f; // Cuanto va a durar el ultimate (Estás demente Parker)

    public override void Awake()
    {
        base.Awake();
    }
}