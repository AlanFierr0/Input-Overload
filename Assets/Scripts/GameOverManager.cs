using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Gestiona el Game Over cuando el jugador pierde toda la vida.
/// Muestra un panel de Game Over y pausa el juego.
/// </summary>
public class GameOverManager : MonoBehaviour
{
    [Header("UI Configuration")]
    [Tooltip("Panel de Game Over que se activará (debe estar en la escena y desactivado al inicio)")]
    public GameObject gameOverPanel;
    
    [Header("Player Reference")]
    [Tooltip("Referencia al componente Health del jugador (se busca automáticamente si no se asigna)")]
    public Health playerHealth;
    
    [Header("Optional Delay")]
    [Tooltip("Segundos de espera antes de mostrar el Game Over (0 = instantáneo)")]
    public float delayBeforeGameOver = 0f;

    private bool gameOverTriggered = false;

    void Start()
    {
        // Validar que el panel de Game Over esté asignado
        if (gameOverPanel == null)
        {
            Debug.LogError("GameOverManager: No se asignó el panel de Game Over! " +
                          "Asigna un GameObject con el UI de Game Over en el Inspector.");
            enabled = false;
            return;
        }
        
        // Asegurar que el panel esté desactivado al inicio
        gameOverPanel.SetActive(false);
        
        // Si no se asignó manualmente, buscar el Health del jugador
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            if (playerHealth == null)
            {
                playerHealth = player.GetComponent<Health>();
            }
        }
        
        if (playerHealth == null)
        {
            Debug.LogError("GameOverManager: No se encontró el componente Health del jugador! " +
                          "Asegúrate de que el jugador tenga el tag 'Player' y el componente Health.");
            enabled = false;
            return;
        }
        
        // Suscribirse al evento de muerte del jugador
        playerHealth.OnDeath += HandlePlayerDeath;
    }

    void OnDestroy()
    {
        // Desuscribirse del evento para evitar errores
        if (playerHealth != null)
        {
            playerHealth.OnDeath -= HandlePlayerDeath;
        }
    }

    /// <summary>
    /// Se llama cuando el jugador muere
    /// </summary>
    void HandlePlayerDeath()
    {
        if (gameOverTriggered) return; // Evitar múltiples llamadas
        
        gameOverTriggered = true;
        
        if (delayBeforeGameOver > 0f)
        {
            Invoke(nameof(ShowGameOver), delayBeforeGameOver);
        }
        else
        {
            ShowGameOver();
        }
    }

    /// <summary>
    /// Muestra el panel de Game Over y pausa el juego
    /// </summary>
    void ShowGameOver()
    {
        if (gameOverPanel == null)
        {
            Debug.LogError("GameOverManager: El panel de Game Over es null!");
            return;
        }
        
        // Cerrar el menú de pausa si está abierto
        PauseMenu pauseMenu = FindFirstObjectByType<PauseMenu>();
        if (pauseMenu != null && pauseMenu.IsPaused())
        {
            pauseMenu.ResumeGame();
        }

        // Resetear habilidades para la proxima partida
        AbilityManager abilityManager = FindFirstObjectByType<AbilityManager>();
        if (abilityManager != null)
        {
            abilityManager.ResetAllAbilities();
        }
        
        // Pausar el juego
        Time.timeScale = 0f;
        
        // Mostrar el cursor
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        
        // Activar el panel de Game Over
        gameOverPanel.SetActive(true);

        // CRÍTICO: Forzar el crosshair al frente después de mostrar el panel
        CrosshairUI crosshair = CrosshairUI.GetInstance();
        if (crosshair != null)
        {
            crosshair.ForceToFront();
        }
    }

    /// <summary>
    /// Verifica si el Game Over está activo
    /// </summary>
    public bool IsGameOver()
    {
        return gameOverTriggered;
    }
}

