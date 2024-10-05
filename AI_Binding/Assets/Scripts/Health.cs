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

    // Reduce la salud cuando hay una colisi�n
    void OnCollisionEnter2D(Collision2D collision)
    {
        // Verifica si la colisi�n es con un objeto enemigo
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

    // Actualiza el estado de los corazones en la UI
    void UpdateHearts()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < health)
            {
                hearts[i].sprite = fullHeart;  // Coraz�n lleno si la salud es mayor
            }
            else
            {
                hearts[i].sprite = emptyHeart; // Coraz�n vac�o si la salud es menor
            }

            // Controla la visibilidad de los corazones seg�n el n�mero m�ximo de corazones
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

    // M�todo para actualizar constantemente el n�mero de corazones (opcional)
    void Update()
    {
        UpdateHearts();
    }
}