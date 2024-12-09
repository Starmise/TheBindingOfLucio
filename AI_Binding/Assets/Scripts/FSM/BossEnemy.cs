using UnityEngine;

public class BossEnemy : BaseEnemy
{
    public int NumberOfBasicAttacksBeforeExit = 3;
    public float BasicAttackRange = 3.0f;
    public float BasicAttackTime = 1.0f;
    public float BasicCooldownTime = 1.0f;

    // Cu�nto tiempo debe durar haciendo el ataque de �rea en el Subestado de Ataque de �rea de MeleeState.
    public float AreaAttackTime = 2.0f;
    public float AreaAttackBuildupTime = 1.0f;
    public float AreaAttackCooldownTime = 1.0f;
    public float AreaAttackRange = 6.0f;

    public float DashAttackRange = 5.0f;
    public float DashAttackTime = 2.0f;
    public float DashBuildupTime = 0.3f;
    public float DashCooldownTime = 1.0f;

    public bool EnableDebug = true;

    public float RangedAttackRange = 10.0f; // Rango de ataque para el RangedState
    public int BasicAtksBeforeAreaAttack = 2; // Ataques b�sicos antes de AreaAttack
    public float UltimateAttackDuration = 2.0f; // Cuanto va a durar el ultimate (Est�s demente Parker)

    GameObject PlayerRef;

    public override void Awake()
    {
        base.Awake();
    }
}