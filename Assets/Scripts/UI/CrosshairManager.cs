using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manager que asegura que siempre exista un crosshair en todas las escenas
/// Se ejecuta automáticamente al cargar cualquier escena
/// </summary>
public class CrosshairManager : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void OnSceneLoaded()
    {
        // Buscar si ya existe un crosshair en la escena
        CrosshairUI existingCrosshair = FindFirstObjectByType<CrosshairUI>();
        
        if (existingCrosshair != null)
        {
            // Ya existe crosshair, asegurar que el cursor esté oculto
            if (existingCrosshair.hideMouseCursor)
            {
                // Verificar si el Game Over está activo antes de ocultar el cursor
                GameOverManager gameOverManager = FindFirstObjectByType<GameOverManager>();
                bool isGameOver = gameOverManager != null && gameOverManager.IsGameOver();
                
                if (!isGameOver)
                {
                    Cursor.visible = false;
                    Cursor.lockState = CursorLockMode.None;
                }
            }
        }
        else
        {
            Debug.LogWarning("CrosshairManager: No se encontró ningún CrosshairUI en la escena. " +
                           "Asegúrate de que el crosshair esté configurado en la primera escena que se carga.");
        }
    }
}

