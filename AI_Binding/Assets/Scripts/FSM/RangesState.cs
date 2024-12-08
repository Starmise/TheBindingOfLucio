using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangesState : BaseState
{
    public enum RangedSubstate
    {
        BasicAttack,
        AreaAttack,
        Ultimate
    }

    private RangedSubstate _rangedSubstate = RangedSubstate.BasicAttack;
    private List<RangedSubstate> _substateHistory = new List<RangedSubstate>();
    private BaseEnemy owner;
    private int BasicAttackCounter = 0;
    private bool SubstateEntered = false;

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
        if (owner == null)
            owner = ((EnemyFSM)FSMRef).Owner;
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        BossEnemy bossOwner = (BossEnemy)owner;

        switch (_rangedSubstate)
        {
            case RangedSubstate.BasicAttack:
                if (!SubstateEntered)
                {
                    SubstateEntered = true;
                    BasicAttackCounter = 0;
                    owner.navMeshAgent.SetDestination(owner.PlayerRef.transform.position);
                }

                if (owner.IsPlayerInRange(bossOwner.RangedAttackRange))
                {
                    BasicAttackCounter++;
                    if (BasicAttackCounter >= bossOwner.BasicAtksBeforeAreaAttack)
                        GoToAreaAttackSubstate();
                    else
                        StartCoroutine(BasicAttack());
                }

                break;

            case RangedSubstate.AreaAttack:
                if (!SubstateEntered)
                {
                    SubstateEntered = true;
                    StartCoroutine(AreaAttack());
                }
                break;

            case RangedSubstate.Ultimate:
                if (!SubstateEntered)
                {
                    SubstateEntered = true;
                    StartCoroutine(UltimateAttack());
                }
                break;
        }
    }

    void GoToAreaAttackSubstate()
    {
        _substateHistory.Add(_rangedSubstate);
        _rangedSubstate = RangedSubstate.AreaAttack;
        SubstateEntered = false;
    }

    IEnumerator BasicAttack()
    {
        BossEnemy bossEnemy = (BossEnemy)owner;
        float timeBetweenAttacks = bossEnemy.BasicAttackTime / 2; // 2 proyectiles
        for (int i = 0; i < 2; i++)
        {
            // Lógica para lanzar un proyectil hacia el jugador
            owner.FireProjectile(owner.PlayerRef.transform.position);
            yield return new WaitForSeconds(timeBetweenAttacks);
        }

        // Transición automática a AreaAttack después de 2 ataques básicos
        GoToAreaAttackSubstate();
    }

    IEnumerator AreaAttack()
    {
        int numberfanAttacks = 6;
        BossEnemy bossEnemy = (BossEnemy)owner;
        float timeBetweenAttacks = bossEnemy.AreaAttackTime / numberfanAttacks; // Ajuste de la cantidad de ataques
        for (int i = 0; i < numberfanAttacks; i++)
        {
            // Lógica para lanzar un proyectil en abanico hacia el jugador
            owner.FireFanAttack(owner.PlayerRef.transform.position);
            yield return new WaitForSeconds(timeBetweenAttacks);
        }

        // Transición a Ultimate si se han realizado todos los ataques
        if (BasicAttackCounter >= 2) // Verificar si se ha realizado la cantidad requerida de ataques
            GoToUltimateSubstate();
    }

    void GoToUltimateSubstate()
    {
        _substateHistory.Add(_rangedSubstate);
        _rangedSubstate = RangedSubstate.Ultimate;
        SubstateEntered = false;
    }

    IEnumerator UltimateAttack()
    {
        BossEnemy bossEnemy = (BossEnemy)owner;
        Vector2[] spawnPoints = GetRandomSpawnPoints(10); // Generar 10 puntos de aparición aleatorios

        foreach (Vector2 point in spawnPoints)
        {
            // Lógica para lanzar un proyectil grande en un punto específico
            owner.FireLargeProjectile(point);
        }

        yield return new WaitForSeconds(bossEnemy.UltimateAttackDuration);

        GoToSelectionState();
    }

    //[SerializeField] private Vector2 spawnRangeMins = new Vector2(0.0f, 0.0f);
    //[SerializeField] private Vector2 spawnRangeMaxs = new Vector2(10.0f, 10.0f);

    Vector2[] GetRandomSpawnPoints(int numPoints)
    {
        Vector2[] points = new Vector2[numPoints];
        for (int i = 0; i < numPoints; i++)
        {
            float x = Random.Range(-10f, 10f); // Ajustar el rango según sea necesario
            float y = Random.Range(-10f, 10f);
            points[i] = new Vector2(x, y);
        }
        return points;
    }


    void GoToSelectionState()
    {
        _substateHistory.Add(_rangedSubstate);
        _rangedSubstate = RangedSubstate.BasicAttack; // Transición de vuelta a BasicAttack por diseño
        SubstateEntered = false;
    }
}
