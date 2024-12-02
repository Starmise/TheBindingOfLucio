using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private string nextSceneName;

    void Update()
    {
        // Vamos a detectar a todos los enemigos en escena con su tag de enemy
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        //Debug.Log("Enemigos actuales: " +  enemies.Length);

        // Siel araay de enemigos es 0, pues no hay enemigos y pasa a siguiente escena
        if (enemies.Length == 0)
        {
            LoadNextScene();
        }
    }

    void LoadNextScene()
    {
        // Comprobamos que sí exista la escena y cargamos el nivel nuevo
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogError("No se ha asignado el nombre de la nueva escena en el Inspector.");
        }
    }
}
