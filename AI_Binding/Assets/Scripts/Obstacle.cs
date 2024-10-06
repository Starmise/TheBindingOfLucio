using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{

    [SerializeField] private float ObstacleForceToApply = 1.0f;

    void OnTriggerStay(Collider other)
    {
        Vector3 OriginToAgent = other.transform.position - transform.position;
        EnemyMovement otherSimpleMovement = other.gameObject.GetComponent<EnemyMovement>();

        if (otherSimpleMovement == null)
        {
            return;
        }
        else
        {
            float distance = OriginToAgent.magnitude;

            SphereCollider collider = GetComponent<SphereCollider>();
            if (collider == null)
            {
                return;
            }
            float obstacleColliderRadius = collider.radius;

            float calculatedForce = ObstacleForceToApply * (1.0f - distance / obstacleColliderRadius);

            otherSimpleMovement.AddExternalForce(OriginToAgent.normalized * calculatedForce);
        }
    }
}