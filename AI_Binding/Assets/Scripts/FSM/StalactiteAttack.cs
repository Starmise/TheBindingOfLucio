using System.Collections;
using UnityEngine;

public class StalactiteAttack : MonoBehaviour
{
    public GameObject shadowPrefab; // Prefab para las sombras
    public GameObject stalactitePrefab; // Prefab para las estalactitas
    public Vector2 minSpawnRange; // Coordenadas mínimas del rango de aparición
    public Vector2 maxSpawnRange; // Coordenadas máximas del rango de aparición
    public int numberOfPoints = 10; // Cantidad de puntos de aparición

    public GameObject projectilePrefab; // Prefab del proyectil

    public float attackInterval = 10f; // Intervalo en segundos para que el ataque se repita (modificable en el Inspector)

    // Método que inicia el ataque al comenzar el juego
    private void Start()
    {
        // Comienza el ataque de manera repetitiva cada 'attackInterval' segundos
        InvokeRepeating("StartUltimateAttack", 0f, attackInterval);
    }

    // Método para iniciar el ataque
    public void StartUltimateAttack()
    {
        StartCoroutine(UltimateAttackRoutine());
    }

    private IEnumerator UltimateAttackRoutine()
    {
        Vector2[] spawnPoints = GetRandomSpawnPoints(numberOfPoints); // Generar puntos de aparición

        GameObject[] shadows = new GameObject[numberOfPoints];

        // Mostrar sombras en los puntos de aparición usando prefabs
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            shadows[i] = CreateObjectAtPosition(spawnPoints[i], shadowPrefab);
        }

        // Esperar 2 segundos
        yield return new WaitForSeconds(2f);

        // Reemplazar sombras con estalactitas y lanzar proyectiles
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            Destroy(shadows[i]); // Eliminar la sombra
            CreateObjectAtPosition(spawnPoints[i], stalactitePrefab);

            // Lógica para lanzar un proyectil grande
            FireLargeProjectile(spawnPoints[i]);
        }
    }

    private Vector2[] GetRandomSpawnPoints(int numPoints)
    {
        Vector2[] points = new Vector2[numPoints];
        for (int i = 0; i < numPoints; i++)
        {
            float x = Random.Range(minSpawnRange.x, maxSpawnRange.x);
            float y = Random.Range(minSpawnRange.y, maxSpawnRange.y);
            points[i] = new Vector2(x, y);
        }
        return points;
    }

    private GameObject CreateObjectAtPosition(Vector2 position, GameObject prefab)
    {
        if (prefab != null)
        {
            GameObject obj = Instantiate(prefab, position, Quaternion.identity);
            return obj;
        }
        return null;
    }

    private void FireLargeProjectile(Vector2 position)
    {
        if (projectilePrefab != null)
        {
            Instantiate(projectilePrefab, position, Quaternion.identity);
        }
    }
}
