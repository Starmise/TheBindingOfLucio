using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Restart : MonoBehaviour
{
	private void Start()
	{
        // Necesito que de sesactive cuando se inicia la escena, ya que solo
        // aparece si el jugador murió.
        gameObject.SetActive(false);
    }

	private void Update()
	{
		if(Input.GetKeyDown(KeyCode.R)){
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		}

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

    }
}
