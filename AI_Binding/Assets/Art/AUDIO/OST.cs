using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    // Singleton para tener una �nica instancia de nuestra canci�n del jefe
    public static BackgroundMusic Instance { get; private set; }

    private void Awake()
    {
        // Si ya existe una instancia, destruye este objeto duplicado
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // Configurar la instancia �nica y mantenerla entre escenas
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
