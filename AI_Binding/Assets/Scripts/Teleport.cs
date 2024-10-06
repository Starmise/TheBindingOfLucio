using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour
{
    public Transform teleportPoint; 
    public Transform secondTeleportPoint;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Verifica si el jugador es quien entra al trigger para evitar que los enemigos
        // sean afectados por los puntos de teletransporte
        if (other.CompareTag("Player"))
        {
            // Debug.Log("Jugador teletransportandose");
            // Teletransporta al jugador al segundo punto
            other.transform.position = secondTeleportPoint.position;
        }
    }
}
