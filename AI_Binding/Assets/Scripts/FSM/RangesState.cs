using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangesState : BaseState // Me dio dislexia, es Ranged
{
    public enum RangedSubstate
    {
        SubstateSelection,
        BasicAttack, // Añadir en el diagrama
        AreaAttack
    }

    private RangedSubstate _meleeSubstate = RangedSubstate.BasicAttack;

    // La lista es decirnos los subestados que se han ejecutado y su orden.
    private List<RangedSubstate> _substateHistory = new List<RangedSubstate>();

    private EnemyFSM enemyFSMRef;

    private BaseEnemy owner;

    private bool SubstateEntered = false;

    private int BasicAttackCounter = 0;

    public RangesState()
    {
        Name = "Ranged State";
    }

    public override void OnEnter()
    {
        base.OnEnter();

        Setup();
    }

    public void Setup()
    {
        if (enemyFSMRef == null)
            enemyFSMRef = (EnemyFSM)FSMRef;

        if (owner == null)
            owner = enemyFSMRef.Owner;

        if (enemyFSMRef != null && owner != null)
        {
            //StartCoroutine(Buildup());
        }
    }

    void OnExitBasicAttack()
    {
        Debug.Log("On exit del basic attack, aquí liberaría los recursos que solamente necesita este subestado.");
    }

    IEnumerator Buildup(float BuildupTime, IEnumerator CoroutineName)
    {
        if (((BossEnemy)owner).EnableDebug)
            Debug.Log("Empecé Build-Up " + CoroutineName.ToString());
        yield return new WaitForSeconds(BuildupTime);
        if (((BossEnemy)owner).EnableDebug)
            Debug.Log("Terminé Build-Up " + CoroutineName.ToString());
        StartCoroutine(CoroutineName);
    }

    IEnumerator AreaAttack()
    {
        if (((BossEnemy)owner).EnableDebug)
            Debug.Log("Empecé Area attack");

        BossEnemy bossEnemy = (BossEnemy)owner;
        int counter = 0;

        int NumAttacks = Random.Range(2, 5);
        float TimeBetweenAttacks = bossEnemy.AreaAttackTime / NumAttacks;

        while (counter < NumAttacks)
        {
            if (((BossEnemy)owner).EnableDebug)
                Debug.Log("Atacando de área número: " + counter);

            yield return new WaitForSeconds(TimeBetweenAttacks + Random.Range(-TimeBetweenAttacks / 2.0f, TimeBetweenAttacks / 2));
            counter++;
        }

        if (((BossEnemy)owner).EnableDebug)
            Debug.Log("Terminé area attack");
        StartCoroutine(Cooldown(bossEnemy.AreaAttackCooldownTime, "Melee Area attack"));
    }

    IEnumerator DashAttack()
    {
        if (((BossEnemy)owner).EnableDebug)
            Debug.Log("Empecé Dash attack");

        BossEnemy bossEnemy = (BossEnemy)owner;
        int counter = 0;

        int NumAttacks = Random.Range(2, 5);
        float TimeBetweenAttacks = bossEnemy.DashAttackTime / NumAttacks;

        while (counter < NumAttacks)
        {
            if (((BossEnemy)owner).EnableDebug)
                Debug.Log("Atacando de dash número: " + counter);

            yield return new WaitForSeconds(TimeBetweenAttacks + Random.Range(-TimeBetweenAttacks / 2.0f, TimeBetweenAttacks / 2));
            counter++;
        }

        if (((BossEnemy)owner).EnableDebug)
            Debug.Log("Terminé Dash attack");
        StartCoroutine(Cooldown(bossEnemy.DashCooldownTime, "Melee Dash"));
    }

    IEnumerator Cooldown(float CooldownTime, string ActionOnCooldown)
    {
        if (((BossEnemy)owner).EnableDebug)
            Debug.Log("Empecé Cooldown de " + ActionOnCooldown);
        yield return new WaitForSeconds(CooldownTime);
        if (((BossEnemy)owner).EnableDebug)
            Debug.Log("Terminé cooldown de " + ActionOnCooldown);

        GoToSelectionState();
    }

    // En árboles de decisiones esto se conoce como GoToParent o GoToRoot.
    void GoToSelectionState()
    {
        // Le decimos que nos guarde este subestado como el más reciente que se tuvo.
        _substateHistory.Add(_meleeSubstate);

        switch (_meleeSubstate)
        {
            case RangedSubstate.BasicAttack:
                // Hacer el OnExit() de Basic Attack.
                OnExitBasicAttack(); // nomás imprime, pero es para dar la idea de cómo se llamaría el OnExit de cada subestado.
                break;
            case RangedSubstate.AreaAttack:
                // Hacer el OnExit()
                Debug.Log("On Exit de Dash.");
                break;
            default:
                // No hace nada.
                Debug.LogWarning("Entré a GoToSelectionState() sin un Subestado válido, fue el subestado: " + _meleeSubstate.ToString());
                break;
        }

        // Ponemos que se vaya al subestado de selección, donde elegiremos el subestado que sigue, según lo que dicte el diseño.
        _meleeSubstate = RangedSubstate.SubstateSelection;
        // Ponemos esta bandera a False, para que el nuevo subestado que entre haga su "OnEnter" según lo necesite.
        SubstateEntered = false;
    }

    // En el OnStart de este estado MeleeState, no necesitamos checar en qué subestado vamos a estar, 
    // porque siempre va a ser en BasicAttack, porque eso nos especifica el diagrama del diseño.

    public override void OnUpdate()
    {
        base.OnUpdate();

        BossEnemy bossOwner = (BossEnemy)owner;

        // En el OnUpdate es donde sí vamos a diferenciar en qué subestado (_meleeSubstate) estamos.
        // este switch tal cual va a ser nuestra "FSM" para los subestados.
        switch (_meleeSubstate)
        {
            case RangedSubstate.SubstateSelection:
                Debug.Log("Entré al substate selection.");

                // NOTA: Hay que tener cuidado si quieren poner que el subestado seleccionador
                // sea el subestado inicial, por esta consideración de este IF.
                if (_substateHistory.Count == 0)
                {
                    Debug.LogWarning("No hay acciones previas al tratar de seleccionar subestado en Melee. Podría causar problemas, manéjese con cuidado.");
                }

                // Si la última acción fue un Dash o un AreaAttack, entonces nos vamos a Basic Attack, según el diagrama establece.
                if (_substateHistory[_substateHistory.Count - 1] == RangedSubstate.AreaAttack)
                {
                    _meleeSubstate = RangedSubstate.BasicAttack;
                    return;
                }

                
                if (owner.IsPlayerInRange(bossOwner.AreaAttackRange))
                {
                    _meleeSubstate = RangedSubstate.AreaAttack;
                    return;
                }

                break;

            case RangedSubstate.BasicAttack:
                
                if (!SubstateEntered)
                {
                    SubstateEntered = true;
                    // Le dices que persiga al player que quiere atacar.
                    owner.navMeshAgent.SetDestination(owner.PlayerRef.transform.position);
                    // Cuando entramos a este subestado, reseteamos el contador de ataques básicos que ha realizado este enemigo.
                    BasicAttackCounter = 0;
                }

                if (owner.IsPlayerInRange(bossOwner.BasicAttackRange))
                {
                    Debug.Log("Puedo atacar básico");
                    BasicAttackCounter++;
                }

                if (BasicAttackCounter >= bossOwner.NumberOfBasicAttacksBeforeExit)
                    GoToSelectionState();

                break;
            case RangedSubstate.AreaAttack:
                if (!SubstateEntered)  // este if(!SubstateEntered) es básicamente el OnEnter de cada subestado.
                {
                    SubstateEntered = true;
                    // Le dices que ahorita no se mueva. (Boss de hades, en sus casos podría llegar a variar)
                    owner.navMeshAgent.SetDestination(owner.transform.position);

                    // Iniciar el build-up del ataque.
                    StartCoroutine(Buildup(bossOwner.DashBuildupTime, DashAttack()));
                    // return; pero no es necesario por cómo está especificado mi subestado de dash.
                }
                break;
            default:
                break;
        }

        EnemyFSM enemyFSM = (EnemyFSM)(FSMRef);
        if (enemyFSM != null)
        {
            // FSMRef.ChangeState(enemyFSM.AlertState);
            // FSMRef.ChangeState((EnemyFSM)(FSMRef).AlertState); // esta línea es lo mismo pero sin el chequeo de seguridad.
        }

    }
}
