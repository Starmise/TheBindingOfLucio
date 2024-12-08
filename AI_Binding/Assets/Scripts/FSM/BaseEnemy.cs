using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class BaseEnemy : MonoBehaviour
{
    public NavMeshAgent navMeshAgent;
    public float HP;
    public float VisionRange;
    public GameObject PlayerRef;

    public virtual void Awake()
    {
        PlayerRef = GameObject.Find("Player");
        if (PlayerRef == null)
        {
            Debug.LogError("No Player was found.");
        }

        navMeshAgent = GetComponent<NavMeshAgent>();
        if (navMeshAgent == null)
        {
            Debug.LogError("No navMeshAgent assigned.");
        }
    }

    // Verificamos si el jugador está en el rango de visión
    public bool IsPlayerInRange(float range)
    {
        float distance = Vector3.Distance(transform.position, PlayerRef.transform.position);
        return distance <= range;
    }


    // Korojefe le lanza la bolita de nieve al jugador
    public void FireProjectile(Vector3 targetPosition)
    {
        // Lógica para instanciar y lanzar un proyectil hacia el targetPosition.
        Debug.Log("Lanzando proyectil básico hacia " + targetPosition);
    }

    // Atque tipo abanico al jugador (Como los del Nier Automata :0)
    public void FireFanAttack(Vector3 targetPosition)
    {
        // Aquí va la lógica para instanciar y lanzar un ataque en abanico.
        Debug.Log("Lanzando ataque en abanico hacia " + targetPosition);
    }

    // Método para lanzar el Ultimate
    public void FireLargeProjectile(Vector3 spawnPoint)
    {
        // Aquí va la lógica para instanciar un proyectil grande en un punto de aparición.
        Debug.Log("Activando el Ultimate en el " + spawnPoint);
    }

    // Verifica si el jugador está en el lado horizontal del jefe
    public bool IsPlayerOnHorizontalSide()
    {
        return Mathf.Abs(transform.position.x - PlayerRef.transform.position.x) > Mathf.Abs(transform.position.y - PlayerRef.transform.position.y);
    }

    // Verifica si el jugador está en el lado vertical del jefe
    public bool IsPlayerOnVerticalSide()
    {
        return Mathf.Abs(transform.position.y - PlayerRef.transform.position.y) > Mathf.Abs(transform.position.x - PlayerRef.transform.position.x);
    }
}