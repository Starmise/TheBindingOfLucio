using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    public int health;
    public int numOfHearts;

    public Image[] hearts;
    public Sprite fullHeart;
    public Sprite emptyHeart;

    // Reduce la salud cuando hay una colisión
    void OnCollisionEnter2D(Collision2D collision)
    {
        // Verifica si la colisión es con un objeto enemigo
        if (collision.gameObject.CompareTag("Enemy"))
        {
            // Reduce la salud del jugador
            health--;

            // Limita la salud para que no sea menor que 0
            if (health < 0)
            {
                health = 0;
            }

            UpdateHearts();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Projectile"))
        {
            health--;

            if (health < 0)
            {
                health = 0;
            }

            UpdateHearts();

        }
    }


    // Actualiza el estado de los corazones en la UI
    void UpdateHearts()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < health)
            {
                hearts[i].sprite = fullHeart;  // Corazón lleno si la salud es mayor
            }
            else
            {
                hearts[i].sprite = emptyHeart; // Corazón vacío si la salud es menor
            }

            // Controla la visibilidad de los corazones según el número máximo de corazones
            if (i < numOfHearts)
            {
                hearts[i].enabled = true;
            }
            else
            {
                hearts[i].enabled = false;
            }
        }
    }

    // Método para actualizar constantemente el número de corazones (opcional)
    void Update()
    {
        UpdateHearts();
    }
}
