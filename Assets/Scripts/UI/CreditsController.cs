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

    void Start()
    {
        // Usar el crosshair personalizado en lugar del cursor del sistema
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.None;
        
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

