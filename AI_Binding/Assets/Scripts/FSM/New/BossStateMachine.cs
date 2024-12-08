using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossStateMachine : MonoBehaviour
{
    private BossState _currentState;
    private Transform player;
    private int basicAttackCount = 0;
    

    [SerializeField] private GameObject projectilePrefab; // Referencia al prefab del proyectil
    [SerializeField] private Transform firePoint; // Punto de disparo del jefe
    [SerializeField] private GameObject bossPrefab; // Punto de disparo del jefe

    // Inicializar con un estado inicial
    public void Initialize(BossState startingState)
    {
        _currentState = startingState;
        _currentState.Enter();
    }

    void Start()
    {
        BossStateMachine bossStateMachine = GetComponent<BossStateMachine>();
        Transform player = GameObject.FindGameObjectWithTag("Player").transform;
        bossStateMachine.Initialize(new BasicProjectileAttackState(bossStateMachine, gameObject, player, projectilePrefab, firePoint));
    }

    // Cambiar a un nuevo estado
    public void ChangeState(BossState newState)
    {
        _currentState.Exit(); // Salir del estado actual
        _currentState = newState; // Cambiar al nuevo estado
        _currentState.Enter(); // Entrar al nuevo estado
    }

    public void UpdateLogic()
    {
        // Lógica del estado actual
        _currentState?.UpdateLogic();
    }

    private void FixedUpdate()
    {
        // Lógica de física del estado actual
        _currentState?.UpdatePhysics();
    }

    private bool IsInProjectileRange(float distanceToPlayer)
    {
        return distanceToPlayer > 5f && distanceToPlayer <= 10f;
    }

    private bool IsInMeleeRange(float distanceToPlayer)
    {
        return distanceToPlayer <= 5f;
    }

    public void IncrementAttackCount()
    {
        basicAttackCount++;
        if (basicAttackCount >= 2)
        {
            basicAttackCount = 0; // Reiniciar el contador
            ChangeState(new SpecialAttackState(this, bossPrefab, player, projectilePrefab, firePoint));
        }
    }

}

