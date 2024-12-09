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

    public float flashDuration = 0.4f;
    private SpriteRenderer spriteRenderer;

    public GameObject RestartSceen;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Reduce la salud cuando hay una colisi�n
    public void DamagePlayer(int damage)
    {
            // Reduce la salud del jugador
            health = health - damage;
            // Se inicia el couroutine para que el jugador cambie de color
            StartCoroutine(FlashHexColor("#EC6262"));

            // Limita la salud para que no sea menor que 0
            if (health < 0)
            {
                health = 0;
            }

            UpdateHearts();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Projectile"))
        {
            health--;

            StartCoroutine(FlashHexColor("#EC6262"));

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
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        UpdateHearts();

        // Pantalla de reinicio o salir 
        if (health <= 0 || enemies.Length == 0)
        {
            Destroy(gameObject);
            RestartSceen.SetActive(true);
        }
    }

    private IEnumerator FlashHexColor(string hexColor)
    {
        // Definimos el color original para poder regresar a ese
        Color originalColor = spriteRenderer.color;
        Color newColor;
        //Coloreamos de rojo siguiendo la estructura que dice el manual de unity
        if (ColorUtility.TryParseHtmlString(hexColor, out newColor))
        {
            spriteRenderer.color = newColor;  // Cambia al color #EC6262
        }
        //Nos esperamos la cantidad indicada con flash duration
        yield return new WaitForSeconds(flashDuration);
        // Se regresa al color original
        spriteRenderer.color = originalColor;
    }
}
