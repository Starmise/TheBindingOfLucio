using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeState : BaseState
{
    public enum MeleeSubstate
    {
        BasicAttack,
        Dash,
        AreaAttack
    }

    private MeleeSubstate _meleeSubstate = MeleeSubstate.BasicAttack; 
    private List<MeleeSubstate> _substateHistory = new List<MeleeSubstate>(); // Historial de subestados para el seguimiento de los cambios.
    private BaseEnemy owner; // Referencia al enemigo que posee el estado padre de pap� (pap� eres t�?).
    private int BasicAttackCounter = 0; // Contador para rastrear cu�ntas veces se ha realizado el ataque b�sico.
    private bool SubstateEntered = false; // Indica si el subestado ha sido ingresado.

    public MeleeState()
    {
        Name = "Melee State";
    }

    public override void OnEnter()
    {
        base.OnEnter();
        Setup();
    }

    // Configura la referencia al due�o (el enemigo) del estado.
    public void Setup()
    {
        if (owner == null)
            owner = ((EnemyFSM)FSMRef).Owner; // Se obtiene la referencia al enemigo desde nuestra m�quina de estados
    }

    // Maneja la l�gica de los subestados: BasicAttack, Dash y AreaAttack.
    public override void OnUpdate()
    {
        base.OnUpdate();
        BossEnemy bossOwner = (BossEnemy)owner; // Se hace un cast a BossEnemy para acceder a propiedades espec�ficas.

        switch (_meleeSubstate)
        {
            case MeleeSubstate.BasicAttack:
                if (!SubstateEntered)
                {
                    // El enemigo se mueve hacia la posici�n del jugador y reinicia el contador de ataques b�sicos
                    SubstateEntered = true;
                    owner.navMeshAgent.SetDestination(owner.PlayerRef.transform.position);
                    BasicAttackCounter = 0;
                }

                // Verifica si el jugador est� en el rango necesario pa el ataque b�sico.
                if (owner.IsPlayerInRange(bossOwner.BasicAttackRange))
                {
                    BasicAttackCounter++;

                    // Si se ha realizado el n�mero de ataques b�sicos necesario, se pasa al subestado de Dash.
                    if (BasicAttackCounter >= bossOwner.NumberOfBasicAttacksBeforeExit)
                        GoToDashSubstate();
                }

                break;

            case MeleeSubstate.Dash:
                if (!SubstateEntered)
                {
                    // El enemigo se mueve hacia la posici�n del jugador y se inicia la corrutina para realizar el ataque de dash.
                    SubstateEntered = true;
                    owner.navMeshAgent.SetDestination(owner.PlayerRef.transform.position);
                    StartCoroutine(DashAttack());
                }
                break;

            case MeleeSubstate.AreaAttack:
                if (!SubstateEntered)
                {
                    SubstateEntered = true;

                    // El enemigo se detiene para realizar el ataque de �rea.
                    owner.navMeshAgent.SetDestination(owner.transform.position);
                    StartCoroutine(AreaAttack());
                }
                break;
        }
    }

    // Cambia el subestado actual al subestado de Dash y lo agrega al historial de subestados.
    void GoToDashSubstate()
    {
        _substateHistory.Add(_meleeSubstate);
        _meleeSubstate = MeleeSubstate.Dash;
        SubstateEntered = false;
    }

    // Realiza ataques dash durante cada cierto tiempo y pasa a la fase de cooldown.
    IEnumerator DashAttack()
    {
        BossEnemy bossEnemy = (BossEnemy)owner;
        int counter = 0;
        int NumAttacks = Random.Range(1, 3); // N�mero aleatorio de ataques en el dash.
        float TimeBetweenAttacks = bossEnemy.DashAttackTime / NumAttacks; // Tiempo entre el tiempo del tiempo de los ataques

        while (counter < NumAttacks)
        {
            yield return new WaitForSeconds(TimeBetweenAttacks + Random.Range(-TimeBetweenAttacks / 2.0f, TimeBetweenAttacks / 2));
            counter++;
        }

        // Despu�s de completar los ataques, se inicia el cooldown.
        StartCoroutine(Cooldown(bossEnemy.DashCooldownTime, "Melee Dash"));
    }

    // Realiza ataques de area durante cada cierto tiempo y y pasa a la fase de cooldown.
    IEnumerator AreaAttack()
    {
        BossEnemy bossEnemy = (BossEnemy)owner;
        int counter = 0;
        int NumAttacks = Random.Range(1, 2); // N�mero aleatorio de ataques en el �rea.
        float TimeBetweenAttacks = bossEnemy.AreaAttackTime / NumAttacks;

        while (counter < NumAttacks)
        {
            yield return new WaitForSeconds(TimeBetweenAttacks + Random.Range(-TimeBetweenAttacks / 2.0f, TimeBetweenAttacks / 2));
            counter++;
        }

        StartCoroutine(Cooldown(bossEnemy.AreaAttackCooldownTime, "Melee Area attack"));
    }

    // Corrutina para manejar el tiempo de cooldown despu�s de un subestado de ataque
    IEnumerator Cooldown(float CooldownTime, string ActionOnCooldown)
    {
        yield return new WaitForSeconds(CooldownTime);
        GoToSelectionState();
    }

    // Cambia al subestado de ataque b�sico despu�s del cooldown.
    void GoToSelectionState()
    {
        _substateHistory.Add(_meleeSubstate);
        _meleeSubstate = MeleeSubstate.BasicAttack; // Transici�n de vuelta a BasicAttack porque pues debe regresar al loop
        SubstateEntered = false;
    }
}
