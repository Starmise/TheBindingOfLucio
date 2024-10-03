using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed; 
    Rigidbody2D rigidbody;

    public GameObject bulletPrefab;
    public float bulletSpeed;
    private float lastBullet;
    [SerializeField] private float bulletDelay;
    
    void Start()
    { 
        rigidbody = GetComponent<Rigidbody2D>();
    }

    void Update()
    { 
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        float shootHorizontal = Input.GetAxis("ShootHorizontal");
        float shootVertical = Input.GetAxis("ShootVertical");

        // Comprobar si el jugador quiere disparar y el tiempo desde el último disparo.
        if ((shootHorizontal != 0 || shootVertical != 0) && Time.time > lastBullet + bulletDelay)
        {
            Shooting(shootHorizontal, shootVertical);
            lastBullet = Time.time;
        }

        rigidbody.velocity = new Vector3(horizontal * speed, vertical * speed, 0);
    }

    void Shooting(float x, float y)
    {
        // "Instantiate" crea una copia de "bulletPrefab" en la posición y rotación actual.
        GameObject bullet = Instantiate(bulletPrefab, transform.position, transform.rotation) as GameObject;
        bullet.AddComponent<Rigidbody2D>().gravityScale = 0;

        // Asignamos la velocidad al Rigidbody2D del objeto "bullet".
        bullet.GetComponent<Rigidbody2D>().velocity = new Vector3 (
            // Se usa un operador ternario, que básicamente evalua un booleano con la
            // siguiente estructura: condición ? valor_true : valor_false
            // Si x es menor que 0, se redondea hacia abajo y asegura que el proyectil
            // se mueva correctamente hacia la izquierda. Si no, se redondea hacia arriba,
            // moviendo el proyectil hacia la derecha.
            (x < 0) ? Mathf.Floor(x) * bulletSpeed : Mathf.Ceil(x) * bulletSpeed,
            (y < 0) ? Mathf.Floor(y) * bulletSpeed : Mathf.Ceil(y) * bulletSpeed,
            0);
    }
}
