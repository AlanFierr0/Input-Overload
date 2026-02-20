using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Controlador para la escena de créditos.
/// </summary>
public class CreditsController : MonoBehaviour
{
    [Header("Configuración")]
    [Tooltip("Nombre de la escena del menú principal")]
    public string nombreEscenaMenu = "MainMenu";
    
    [Header("Botón de Retorno (Opcional)")]
    [Tooltip("Arrastra aquí el botón de volver al menú si tienes uno")]
    public Button botonVolver;

    [Header("Audio")]
    [Tooltip("Música de fondo para los créditos")]
    public AudioClip creditsBGM;

    void Start()
    {
        // Reestablecerse el tiempo a velocidad normal (por si viene del juego en pausa)
        Time.timeScale = 1f;
        
        // Usar el crosshair personalizado en lugar del cursor del sistema
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.None;
        
        // Reproducir música de créditos
        if (creditsBGM != null)
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayBGM(creditsBGM, true);
                Debug.Log("CreditsController: Reproduciendo música de créditos");
            }
            else
            {
                Debug.LogWarning("CreditsController: AudioManager.Instance es NULL - intentando obtenerlo");
                // Crear AudioManager si no existe
                AudioManager audioManager = FindFirstObjectByType<AudioManager>();
                if (audioManager == null)
                {
                    Debug.LogError("CreditsController: No se encontró AudioManager en la escena");
                }
                else
                {
                    audioManager.PlayBGM(creditsBGM, true);
                }
            }
        }
        else
        {
            Debug.LogWarning("CreditsController: creditsBGM no está asignado en el Inspector");
        }
        
        // Asignar la función al botón de volver si existe
        if (botonVolver != null)
        {
            botonVolver.onClick.AddListener(VolverAlMenu);
        }
    }

    void Update()
    {
        // Asegurar que el cursor del sistema esté siempre oculto
        if (Cursor.visible)
        {
            Cursor.visible = false;
        }
        
        // Permitir volver al menú con ESC
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            VolverAlMenu();
        }
    }

    /// <summary>
    /// Vuelve al menú principal
    /// </summary>
    public void VolverAlMenu()
    {
        Debug.Log("Volviendo al menú principal...");
        SceneManager.LoadScene(nombreEscenaMenu);
    }
}

