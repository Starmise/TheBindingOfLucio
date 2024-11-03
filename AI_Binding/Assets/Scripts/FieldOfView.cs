using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    public GameObject targetGameObject = null;

    public float viewRadius = 5f;
    [Range(0, 360)]
    public float viewAngle = 90f;
    public int resolution = 30;
    public LayerMask obstacleMask;

    private LineRenderer lineRenderer;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = resolution + 1;
        lineRenderer.useWorldSpace = true;
        lineRenderer.loop = false;
    }

    void Update()
    {
        DrawFieldOfView();
    }

    void DrawFieldOfView()
    {
        lineRenderer.positionCount = resolution + 1;
        float angleStep = viewAngle / resolution;
        List<Vector3> points = new List<Vector3>();

        for (int i = 0; i <= resolution; i++)
        {
            float angle = transform.eulerAngles.z - viewAngle / 2 + angleStep * i;
            Vector3 dir = DirFromAngle(angle);
            Vector3 point = transform.position + dir * viewRadius;

            RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, viewRadius, obstacleMask);
            if (hit.collider != null)
            {
                point = hit.point; // Si hay un obstáculo, limita el punto al obstáculo
            }
            points.Add(point);
        }

        lineRenderer.SetPositions(points.ToArray());
    }

    Vector3 DirFromAngle(float angleInDegrees)
    {
        float rad = angleInDegrees * Mathf.Deg2Rad;
        return new Vector3(Mathf.Cos(rad), Mathf.Sin(rad));
    }

    public bool CheckFieldOfView()
    {
        Vector2 directionToPlayer = (targetGameObject.transform.position - transform.position).normalized;
        float distanceToPlayer = Vector2.Distance(transform.position, targetGameObject.transform.position);

        // Verifica que el jugador esté dentro del rango y ángulo de visión
        if (distanceToPlayer <= viewRadius)
        {
            float angleToPlayer = Vector2.Angle(transform.right, directionToPlayer);
            if (angleToPlayer < viewAngle / 2)
            {
                // Raycast para verificar si hay obstáculos entre el enemigo y el jugador
                RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, distanceToPlayer, obstacleMask);
                if (!hit.collider)
                {
                    return true; // Jugador detectado
                }
            }
        }
        return false;
    }

}
