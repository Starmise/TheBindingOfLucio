using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class MeleeState : BaseState
{
    public enum MeleeSubstate
    {
        SubstateSelection,
        BasicAttack, // A�adir en el diagrama
        Dash,
        Ultimate
    }

    private MeleeSubstate _meleeSubstate = MeleeSubstate.BasicAttack;

    // La lista es decirnos los subestados que se han ejecutado y su orden.
    private List<MeleeSubstate> _substateHistory = new List<MeleeSubstate>();

    // Si el player esta lejos, me acerco hasta estar en X rango.

    // tenemos que tener una referencia al player (u objetivo) al que se va a acercar.
    // Posibilidad 1, que la referencia la tenga la EnemyFSM
    // 
    // este enemigo debe tener una forma de moverse hacia su objetivo.
    // componente NavMeshAgent 
    // tiene que poder detectar si una posicion X esta dentro de un rango Y.


    // Si el player esta dentro de X rango, hago UN ataque basico, 
    // de los 3 disponibles, cada uno tiene 33% de probabilidad, vamos empezando solo con 1.

    private EnemyFSM enemyFSMRef;

    private BaseEnemy owner;

    private bool SubstateEntered = false;

    private int BasicAttackCounter = 0;

    public MeleeState()
    {
        Name = "Melee State";
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
        Debug.Log("On exit del basic attack, aqu� liberar�a los recursos que solamente necesita este subestado.");
    }

    IEnumerator Buildup(float BuildupTime, IEnumerator CoroutineName)
    {
        if (((BossEnemy)owner).EnableDebug)
            Debug.Log("Empec� Build-Up " + CoroutineName.ToString());
        yield return new WaitForSeconds(BuildupTime);
        if (((BossEnemy)owner).EnableDebug)
            Debug.Log("Termin� Build-Up " + CoroutineName.ToString());
        StartCoroutine(CoroutineName);
    }

    IEnumerator AreaAttack()
    {
        if (((BossEnemy)owner).EnableDebug)
            Debug.Log("Empec� Area attack");

        BossEnemy bossEnemy = (BossEnemy)owner;
        int counter = 0;

        int NumAttacks = Random.Range(2, 5);
        float TimeBetweenAttacks = bossEnemy.AreaAttackTime / NumAttacks;

        while (counter < NumAttacks)
        {
            if (((BossEnemy)owner).EnableDebug)
                Debug.Log("Atacando de �rea n�mero: " + counter);

            yield return new WaitForSeconds(TimeBetweenAttacks + Random.Range(-TimeBetweenAttacks / 2.0f, TimeBetweenAttacks / 2));
            counter++;
        }

        if (((BossEnemy)owner).EnableDebug)
            Debug.Log("Termin� area attack");
        StartCoroutine(Cooldown(bossEnemy.AreaAttackCooldownTime, "Melee Area attack"));
    }

    IEnumerator DashAttack()
    {
        if (((BossEnemy)owner).EnableDebug)
            Debug.Log("Empec� Dash attack");

        BossEnemy bossEnemy = (BossEnemy)owner;
        int counter = 0;

        int NumAttacks = Random.Range(2, 5);
        float TimeBetweenAttacks = bossEnemy.DashAttackTime / NumAttacks;

        while (counter < NumAttacks)
        {
            if (((BossEnemy)owner).EnableDebug)
                Debug.Log("Atacando de dash n�mero: " + counter);

            yield return new WaitForSeconds(TimeBetweenAttacks + Random.Range(-TimeBetweenAttacks / 2.0f, TimeBetweenAttacks / 2));
            counter++;
        }

        if (((BossEnemy)owner).EnableDebug)
            Debug.Log("Termin� Dash attack");
        StartCoroutine(Cooldown(bossEnemy.DashCooldownTime, "Melee Dash"));
    }

    IEnumerator Cooldown(float CooldownTime, string ActionOnCooldown)
    {
        if (((BossEnemy)owner).EnableDebug)
            Debug.Log("Empec� Cooldown de " + ActionOnCooldown);
        yield return new WaitForSeconds(CooldownTime);
        if (((BossEnemy)owner).EnableDebug)
            Debug.Log("Termin� cooldown de " + ActionOnCooldown);

        GoToSelectionState();
    }

    // En �rboles de decisiones esto se conoce como GoToParent o GoToRoot.
    void GoToSelectionState()
    {
        // Le decimos que nos guarde este subestado como el m�s reciente que se tuvo.
        _substateHistory.Add(_meleeSubstate);

        switch (_meleeSubstate)
        {
            case MeleeSubstate.BasicAttack:
                // Hacer el OnExit() de Basic Attack.
                OnExitBasicAttack(); // nom�s imprime, pero es para dar la idea de c�mo se llamar�a el OnExit de cada subestado.
                break;
            case MeleeSubstate.Dash:
                // Hacer el OnExit()
                Debug.Log("On Exit de Dash.");
                break;
            case MeleeSubstate.Ultimate:
                // Hacer el OnExit()
                Debug.Log("On Exit de Ultimate.");
                break;
            default:
                // No hace nada.
                Debug.LogWarning("Entr� a GoToSelectionState() sin un Subestado v�lido, fue el subestado: " + _meleeSubstate.ToString());
                break;
        }

        // Ponemos que se vaya al subestado de selecci�n, donde elegiremos el subestado que sigue, seg�n lo que dicte el dise�o.
        _meleeSubstate = MeleeSubstate.SubstateSelection;
        // Ponemos esta bandera a False, para que el nuevo subestado que entre haga su "OnEnter" seg�n lo necesite.
        SubstateEntered = false;
    }

    // En el OnStart de este estado MeleeState, no necesitamos checar en qu� subestado vamos a estar, 
    // porque siempre va a ser en BasicAttack, porque eso nos especifica el diagrama del dise�o.

    public override void OnUpdate()
    {
        base.OnUpdate();

        BossEnemy bossOwner = (BossEnemy)owner;

        // En el OnUpdate es donde s� vamos a diferenciar en qu� subestado (_meleeSubstate) estamos.
        // este switch tal cual va a ser nuestra "FSM" para los subestados.
        switch (_meleeSubstate)
        {
            case MeleeSubstate.SubstateSelection:
                Debug.Log("Entr� al substate selection.");

                // NOTA: Hay que tener cuidado si quieren poner que el subestado seleccionador
                // sea el subestado inicial, por esta consideraci�n de este IF.
                if (_substateHistory.Count == 0)
                {
                    Debug.LogWarning("No hay acciones previas al tratar de seleccionar subestado en Melee. Podr�a causar problemas, man�jese con cuidado.");
                }

                // �C�mo podemos saber cu�l fue el �ltimo Subestado activo antes de SubstateSelection?
                // El _SubstateHistory nos da cu�les fueron las acciones pasadas realizadas y qu� orden.


                // En mi dise�o, el Ultimate se hace cuando ya se realiz� al menos uno de cada uno de los dem�s subestados.
                if (_substateHistory.Contains(MeleeSubstate.BasicAttack) &&
                    _substateHistory.Contains(MeleeSubstate.Dash))
                {
                    // Entonces hacemos el Ultimate.
                    _meleeSubstate = MeleeSubstate.Ultimate;
                    return;
                }


                // Si la �ltima acci�n fue un Dash o un AreaAttack, entonces nos vamos a Basic Attack, seg�n el diagrama establece.
                if (_substateHistory[_substateHistory.Count - 1] == MeleeSubstate.Dash)
                {
                    _meleeSubstate = MeleeSubstate.BasicAttack;
                    return;
                }

                // Checamos posible transici�n:
                // Haber hecho ataque b�sico 3 veces Y
                // Si el player est� dentro del rango X donde s� le alcanzar�a(o casi) a pegar este ataque de �rea.
                if (owner.IsPlayerInRange(bossOwner.AreaAttackRange))
                {
                    _meleeSubstate = MeleeSubstate.Dash;
                    return;
                }

                break;

            case MeleeSubstate.BasicAttack:
                /*
                 Si el player est� lejos, me acerco hasta estar en X rango.
                    Si el player est� dentro de X rango, hago UN ataque b�sico,
                de los 3 disponibles, cada uno tiene 33% de probabilidad.

                 */

                if (!SubstateEntered)
                {
                    SubstateEntered = true;
                    // Le dices que persiga al player que quiere atacar.
                    owner.navMeshAgent.SetDestination(owner.PlayerRef.transform.position);
                    // Cuando entramos a este subestado, reseteamos el contador de ataques b�sicos que ha realizado este enemigo.
                    BasicAttackCounter = 0;
                }

                if (owner.IsPlayerInRange(bossOwner.BasicAttackRange))
                {
                    Debug.Log("Puedo atacar b�sico");
                    BasicAttackCounter++;
                }

                if (BasicAttackCounter >= bossOwner.NumberOfBasicAttacksBeforeExit)
                    GoToSelectionState();

                break;
            case MeleeSubstate.Dash:
                if (!SubstateEntered)  // este if(!SubstateEntered) es b�sicamente el OnEnter de cada subestado.
                {
                    SubstateEntered = true;
                    // Le dices que ahorita no se mueva. (Boss de hades, en sus casos podr�a llegar a variar)
                    owner.navMeshAgent.SetDestination(owner.transform.position);

                    // Iniciar el build-up del ataque.
                    StartCoroutine(Buildup(bossOwner.DashBuildupTime, DashAttack()));
                    // return; pero no es necesario por c�mo est� especificado mi subestado de dash.
                }
                break;
            case MeleeSubstate.Ultimate:
                // Cuando entra a este subestado NO se mueve hacia el jugador (al menos la del juego de Hades)
                if (!SubstateEntered)  // este if(!SubstateEntered) es b�sicamente el OnEnter de cada subestado.
                {
                    SubstateEntered = true;
                    // Le dices que ahorita no se mueva. (Boss de hades, en sus casos podr�a llegar a variar)
                    owner.navMeshAgent.SetDestination(owner.transform.position);


                    Debug.Log("HACIENDO ULTIMATE!");
                    // Iniciar el build-up del ataque.
                    // StartCoroutine(Buildup(bossOwner.AreaAttackBuildupTime, AreaAttack()));
                    // return; pero no es necesario por c�mo est� especificado mi subestado de AreaAttack.
                }
                break;
            default:
                break;
        }

        // Oye, m�quina de estados que es due�a de este script, cambia hacia el estado de Alerta.
        // ESTO NO ME DEJA PORQUE LA FSMRef NO ES DE TIPO EnemyFSM que es la que tiene el AlertState
        // FSMRef.ChangeState( FSMRef.AlertState ); 

        EnemyFSM enemyFSM = (EnemyFSM)(FSMRef);
        if (enemyFSM != null)
        {
            // FSMRef.ChangeState(enemyFSM.AlertState);
            // FSMRef.ChangeState((EnemyFSM)(FSMRef).AlertState); // esta l�nea es lo mismo pero sin el chequeo de seguridad.
        }

    }
}