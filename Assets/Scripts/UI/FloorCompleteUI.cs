using UnityEngine;

/// <summary>
/// UI que aparece al completar todas las habitaciones del piso.
/// Ofrece dos opciones: siguiente piso (reinicia GameScene) o volver al menú principal.
///
/// Setup en el Inspector:
///   - Asignar este script al panel FloorComplete.
///   - Conectar los botones del panel a NextFloor() y ReturnToMainMenu().
/// </summary>
public class FloorCompleteUI : MonoBehaviour
{
    /// <summary>
    /// Reinicia la escena de juego (siguiente piso).
    /// </summary>
    public void NextFloor()
    {
        if (GameSceneManager.Instance != null)
        {
            GameSceneManager.Instance.LoadGame();
        }
        else
        {
            Debug.LogError("FloorCompleteUI: No se encontró GameSceneManager.Instance.");
        }
    }

    /// <summary>
    /// Vuelve al menú principal.
    /// </summary>
    public void ReturnToMainMenu()
    {
        if (GameSceneManager.Instance != null)
        {
            GameSceneManager.Instance.LoadMainMenu();
        }
        else
        {
            Debug.LogError("FloorCompleteUI: No se encontró GameSceneManager.Instance.");
        }
    }
}
