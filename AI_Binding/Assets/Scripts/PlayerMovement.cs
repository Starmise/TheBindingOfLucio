using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed;
    Rigidbody2D rigidbody;

    public GameObject bulletPrefab;
    [SerializeField] private float bulletSpeed;
    private float lastBullet;
    [SerializeField] private float bulletDelay;

    // Variables para cambiar los sprites
    private SpriteRenderer spriteRenderer;
    public Sprite spriteUp;
    public Sprite spriteDown;
    public Sprite spriteLeft;
    public Sprite spriteRight;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        float shootHorizontal = Input.GetAxis("ShootHorizontal");
        float shootVertical = Input.GetAxis("ShootVertical");

        // Comprobar si el jugador quiere disparar y el tiempo desde el �ltimo disparo.
        if ((shootHorizontal != 0 || shootVertical != 0) && Time.time > lastBullet + bulletDelay)
        {
            Shooting(shootHorizontal, shootVertical);
            lastBullet = Time.time;
        }

        rigidbody.velocity = new Vector3(horizontal * speed, vertical * speed, 0);

        // Actualizar el sprite seg�n la direcci�n del disparo o del movimiento
        UpdateSpriteDirection(horizontal, vertical, shootHorizontal, shootVertical);
    }

    void Shooting(float x, float y)
    {
        // "Instantiate" crea una copia de "bulletPrefab" en la posici�n y rotaci�n actual.
        GameObject bullet = Instantiate(bulletPrefab, transform.position, transform.rotation) as GameObject;
        bullet.AddComponent<Rigidbody2D>().gravityScale = 0;

        // Se usa un operador ternario, que basicamente evalua un booleano con la
        // siguiente estructura: condicion ? valor_true : valor_false
        // Si x es menor que 0, se redondea hacia abajo y asegura que el proyectil
        // se mueva correctamente hacia la izquierda. Si no, se redondea hacia arriba,
        // moviendo el proyectil hacia la derecha.
        bullet.GetComponent<Rigidbody2D>().velocity = new Vector3(
            (x < 0) ? Mathf.Floor(x) * bulletSpeed : Mathf.Ceil(x) * bulletSpeed,
            (y < 0) ? Mathf.Floor(y) * bulletSpeed : Mathf.Ceil(y) * bulletSpeed,
            0);
    }

    void UpdateSpriteDirection(float horizontal, float vertical, float shootHorizontal, float shootVertical)
    {
        // Priorizar el disparo para cambiar el sprite
        if (shootHorizontal != 0 || shootVertical != 0)
        {
            if (shootHorizontal > 0)
            {
                spriteRenderer.sprite = spriteRight;
            }
            else if (shootHorizontal < 0)
            {
                spriteRenderer.sprite = spriteLeft;
            }
            else if (shootVertical > 0)
            {
                spriteRenderer.sprite = spriteUp;
            }
            else if (shootVertical < 0)
            {
                spriteRenderer.sprite = spriteDown;
            }
        }
        // Si no dispara, cambiar seg�n el movimiento
        else
        {
            if (horizontal > 0)
            {
                spriteRenderer.sprite = spriteRight;
            }
            else if (horizontal < 0)
            {
                spriteRenderer.sprite = spriteLeft;
            }
            else if (vertical > 0)
            {
                spriteRenderer.sprite = spriteUp;
            }
            else if (vertical < 0)
            {
                spriteRenderer.sprite = spriteDown;
            }
        }
    }

    public Vector2 GetMovementDirection()
    {
        // Debido a que el Player usa un float para calcular la velocidad y el enemigo
        // usa un Vector2, se ajusta mediante este metodo.
        return new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized;
    }
}
