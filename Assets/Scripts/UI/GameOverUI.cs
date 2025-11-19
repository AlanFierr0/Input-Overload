using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Maneja los botones de la UI de Game Over
/// </summary>
public class GameOverUI : MonoBehaviour
{
    [Header("Scene Names")]
    [Tooltip("Nombre de la escena del juego")]
    public string gameSceneName = "GameScene";
    
    [Tooltip("Nombre de la escena del menú principal")]
    public string mainMenuSceneName = "MainMenu";
    
    [Header("Button References (Optional)")]
    [Tooltip("Botón de Retry - se puede asignar manualmente o por código")]
    public Button retryButton;
    
    [Tooltip("Botón de Main Menu - se puede asignar manualmente o por código")]
    public Button mainMenuButton;
    
    [Tooltip("Botón de Quit - se puede asignar manualmente o por código")]
    public Button quitButton;

    void Start()
    {
        // Asignar las funciones a los botones si están asignados
        if (retryButton != null)
        {
            retryButton.onClick.AddListener(Retry);
        }
        
        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.AddListener(LoadMainMenu);
        }
        
        if (quitButton != null)
        {
            quitButton.onClick.AddListener(QuitGame);
        }
    }

    /// <summary>
    /// Reinicia la escena del juego desde cero
    /// </summary>
    public void Retry()
    {
        // Normalizar el tiempo antes de cargar la escena
        Time.timeScale = 1f;
        
        // Cargar la escena del juego
        SceneManager.LoadScene(gameSceneName);
    }

    /// <summary>
    /// Carga el menú principal
    /// </summary>
    public void LoadMainMenu()
    {
        // Normalizar el tiempo antes de cargar la escena
        Time.timeScale = 1f;
        
        // Cargar la escena del menú principal
        SceneManager.LoadScene(mainMenuSceneName);
    }

    /// <summary>
    /// Sale del juego
    /// </summary>
    public void QuitGame()
    {
        // Normalizar el tiempo por si acaso
        Time.timeScale = 1f;
        
        #if UNITY_EDITOR
            // Si estamos en el editor, detener el play mode
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            // Si es una build, cerrar la aplicación
            Application.Quit();
        #endif
    }
}

