using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class PauseMenu : MonoBehaviour
{
    public GameObject pausePanel;
    public Button resumeButton;
    public Button optionsButton;
    public Button mainMenuButton;
    public Button quitButton;
    public bool pauseGameTime = true;

    private bool isPaused = false;
    private LevelUpUI levelUpUI;
    private GameOverManager gameOverManager;

    void Start()
    {
        levelUpUI = FindFirstObjectByType<LevelUpUI>();
        gameOverManager = FindFirstObjectByType<GameOverManager>();
        if (pausePanel != null) pausePanel.SetActive(false);
        SetupButtons();
        isPaused = false;
        
        // Asegurar que el cursor del sistema esté siempre oculto
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.None;
    }

    void SetupButtons()
    {
        if (resumeButton != null)
        {
            resumeButton.onClick.RemoveAllListeners();
            resumeButton.onClick.AddListener(ResumeGame);
        }

        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.RemoveAllListeners();
            mainMenuButton.onClick.AddListener(GoToMainMenu);
        }

        if (quitButton != null)
        {
            quitButton.onClick.RemoveAllListeners();
            quitButton.onClick.AddListener(QuitGame);
        }

        if (optionsButton != null)
        {
            optionsButton.onClick.RemoveAllListeners();
            optionsButton.onClick.AddListener(ShowOptions);
        }
    }

    void Update()
    {
        // Asegurar que el cursor del sistema esté siempre oculto (excepto si el Game Over está activo)
        if (gameOverManager != null && gameOverManager.IsGameOver())
        {
            // No hacer nada con el cursor durante Game Over
            return;
        }
        
        if (Cursor.visible)
        {
            Cursor.visible = false;
        }
        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // No permitir pausar si el Level Up está activo
            if (levelUpUI != null && levelUpUI.levelUpPanel != null && levelUpUI.levelUpPanel.activeSelf) return;
            
            // No permitir pausar si el Game Over está activo
            if (gameOverManager != null && gameOverManager.IsGameOver()) return;

            if (isPaused) ResumeGame();
            else PauseGame();
        }
        
        if (isPaused && pausePanel != null && pausePanel.activeSelf) ProcessManualInput();
    }
    
    void ProcessManualInput()
    {
        if (UnityEngine.EventSystems.EventSystem.current == null) return;
        
        if (Input.GetMouseButtonDown(0))
        {
            PointerEventData pointerData = new PointerEventData(UnityEngine.EventSystems.EventSystem.current);
            pointerData.position = Input.mousePosition;
            
            var results = new System.Collections.Generic.List<RaycastResult>();
            UnityEngine.EventSystems.EventSystem.current.RaycastAll(pointerData, results);
            
            foreach (var result in results)
            {
                Button btn = result.gameObject.GetComponent<Button>();
                if (btn != null && btn.interactable)
                {
                    btn.onClick.Invoke();
                    break;
                }
            }
        }
    }

    public void PauseGame()
    {
        if (isPaused) return;

        isPaused = true;
        if (pausePanel != null) pausePanel.SetActive(true);
        SetupButtons();

        UnityEngine.EventSystems.EventSystem eventSystem = UnityEngine.EventSystems.EventSystem.current;
        if (eventSystem != null)
        {
            UnscaledTimeInputModule unscaledInput = eventSystem.GetComponent<UnscaledTimeInputModule>();
            if (unscaledInput == null)
            {
                UnityEngine.EventSystems.StandaloneInputModule standardInput = eventSystem.GetComponent<UnityEngine.EventSystems.StandaloneInputModule>();
                if (standardInput != null) standardInput.enabled = false;
                unscaledInput = eventSystem.gameObject.AddComponent<UnscaledTimeInputModule>();
            }
            unscaledInput.enabled = true;
        }

        if (pauseGameTime) Time.timeScale = 0f;

        // Mantener el cursor oculto para usar el crosshair personalizado
        // Cursor.visible = true;
        // Cursor.lockState = CursorLockMode.None;
    }

    public void ResumeGame()
    {
        if (!isPaused) return;

        isPaused = false;
        if (pausePanel != null) pausePanel.SetActive(false);
        if (pauseGameTime) Time.timeScale = 1f;

        // El cursor ya está oculto, no necesitamos hacer nada
        // CrosshairUI crosshair = FindFirstObjectByType<CrosshairUI>();
        // if (crosshair != null && crosshair.hideMouseCursor) Cursor.visible = false;
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        
        GameSceneManager sceneManager = FindFirstObjectByType<GameSceneManager>();
        if (sceneManager != null)
        {
            sceneManager.LoadMainMenu();
        }
        else
        {
            SceneManager.LoadScene("MainMenu");
        }
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    public void ShowOptions()
    {
    }

    public bool IsPaused() => isPaused;
}

