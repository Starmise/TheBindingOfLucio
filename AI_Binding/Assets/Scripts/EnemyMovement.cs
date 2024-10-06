using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField]
    protected float maxSpeed = 5;
    [SerializeField]
    protected float maxAcceleration = 1.0f;
    public int health = 4;

    public Vector2 velocity = Vector2.zero;

    public GameObject targetGameObject = null;

    protected float PursuitTimePrediction = 1.0f;

    protected Vector2 ExternalForces = Vector2.zero;

    public void AddExternalForce(Vector2 ExternalForce)
    {
        ExternalForces += ExternalForce;
    }

    public Vector2 PuntaMenosCola(Vector2 punta, Vector2 cola)
    {
        float x = (punta.x - cola.x);
        float y = (punta.y - cola.y);
        return new Vector2(x, y);

        // Alternativa
        // return new Vector2(punta.x - cola.x, punta.y - cola.y);
    }

    public Vector3 PuntaMenosCola3D(Vector3 punta, Vector3 cola)
    {
        float x = punta.x - cola.x;
        float y = punta.y - cola.y;
        float z = punta.z - cola.z;

        return new Vector3(x, y, z);
    }

    protected void Start()
    {
        Debug.Log("Se está ejecutando " + gameObject.name);
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Enemy"), LayerMask.NameToLayer("Enemy"));
    }

    void Update()
    {
        if (targetGameObject == null)
        {
            return;
        }

        // Se obtiene la direrección del jugador para obtener su velocidad actual
        Vector2 playerDirection = targetGameObject.GetComponent<PlayerMovement>().GetMovementDirection(); 

        // Se multiplica la dirección por la velocidad para obtener la velocidad en Vector2
        Vector2 currentVelocity = playerDirection * targetGameObject.GetComponent<PlayerMovement>().speed;

        // Calcular el tiempo de predicción
        PursuitTimePrediction = CalculatePredictedTime(maxSpeed, transform.position, targetGameObject.transform.position);

        // Predecir la posición futura del jugador
        Vector2 PredictedPosition = PredictPosition(targetGameObject.transform.position, currentVelocity, PursuitTimePrediction);

        // Calcular la dirección hacia la posición que predecimos
        Vector2 PosToTarget = -PuntaMenosCola(PredictedPosition, transform.position); //Flee
        PosToTarget += ExternalForces;

        // Actualizar la velocidad del enemigo
        velocity += PosToTarget.normalized * maxAcceleration * Time.deltaTime;

        // Limitar la velocidad para que no exceda la velocidad máxima
        velocity = Vector2.ClampMagnitude(velocity, maxSpeed);
        transform.position += (Vector3)(velocity * Time.deltaTime);

        // Resetear las fuerzas externas al final para poder usarlas
        ExternalForces = Vector2.zero;
    }

    Vector2 PredictPosition(Vector2 InitialPosition, Vector2 Velocity, float TimePrediction)
    {
        // Con base en la Velocity, calcular la posición del jugador tras X tiempo.
        return InitialPosition + Velocity * TimePrediction;
    }

    float CalculatePredictedTime(float MaxSpeed, Vector2 InitialPosition, Vector2 TargetPosition)
    {
        // Calcular la distancia entre InitialPosition y TargetPosition. 
        float Distance = PuntaMenosCola(TargetPosition, InitialPosition).magnitude;
        return Distance / MaxSpeed;
    }
}
