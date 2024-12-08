using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    // Singleton para tener una única instancia de nuestra canción del jefe
    public static BackgroundMusic Instance { get; private set; }

    private void Awake()
    {
        // Si ya existe una instancia, destruye este objeto duplicado
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // Configurar la instancia única y mantenerla entre escenas
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
