using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UltimateState : BossState
{
    private Transform player;
    private GameObject projectilePrefab;
    private float duration = 5f; // Duraci�n del ataque
    private float timer;
    private Vector2 stageBounds = new Vector2(20f, 10f); // Tama�o del escenario

    public UltimateState(BossStateMachine stateMachine, GameObject boss, Transform player, GameObject projectilePrefab)
        : base(stateMachine, boss)
    {
        this.player = player;
        this.projectilePrefab = projectilePrefab;
    }

    public override void Enter()
    {
        // Reproducir animaci�n de salto
        boss.GetComponent<Animator>().SetTrigger("Ultimate");
        timer = duration;

        // Comenzar a generar proyectiles
        boss.GetComponent<MonoBehaviour>().StartCoroutine(SpawnProjectiles());
    }

    private IEnumerator SpawnProjectiles()
    {
        while (timer > 0)
        {
            // Generar un proyectil en una posici�n aleatoria
            Vector2 spawnPosition = new Vector2(
                Random.Range(-stageBounds.x, stageBounds.x),
                Random.Range(-stageBounds.y, stageBounds.y));

            GameObject.Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);

            yield return new WaitForSeconds(0.1f); // Intervalo entre proyectiles
        }
    }

    public override void UpdateLogic()
    {
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            // Cambiar a estado inactivo tras el ataque
            stateMachine.ChangeState(new IdleState(stateMachine, boss, player));
        }
    }
}
