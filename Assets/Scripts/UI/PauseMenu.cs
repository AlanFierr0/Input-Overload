using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

/// <summary>
/// Script que maneja el menú de pausa del juego.
/// Se activa/desactiva con la tecla Escape.
/// </summary>
public class PauseMenu : MonoBehaviour
{
    [Header("UI References")]
    public GameObject pausePanel;
    public Button resumeButton;
    public Button optionsButton;
    public Button mainMenuButton;
    public Button quitButton;

    [Header("Settings")]
    [Tooltip("Si está activado, el menú de pausa pausará el juego (Time.timeScale = 0)")]
    public bool pauseGameTime = true;

    private bool isPaused = false;
    private LevelUpUI levelUpUI;

    void Start()
    {
        // Buscar LevelUpUI para verificar si está visible
        levelUpUI = FindFirstObjectByType<LevelUpUI>();

        // Asegurar que el panel esté oculto al inicio
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }

        // Configurar botones si existen
        SetupButtons();

        isPaused = false;
    }

    void SetupButtons()
    {
        // Limpiar listeners anteriores antes de agregar nuevos
        if (resumeButton != null)
        {
            resumeButton.onClick.RemoveAllListeners();
            resumeButton.onClick.AddListener(ResumeGame);
            resumeButton.interactable = true;
        }

        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.RemoveAllListeners();
            mainMenuButton.onClick.AddListener(GoToMainMenu);
            mainMenuButton.interactable = true;
        }

        if (quitButton != null)
        {
            quitButton.onClick.RemoveAllListeners();
            quitButton.onClick.AddListener(QuitGame);
            quitButton.interactable = true;
        }

        if (optionsButton != null)
        {
            optionsButton.onClick.RemoveAllListeners();
            optionsButton.onClick.AddListener(ShowOptions);
            optionsButton.interactable = true;
        }
    }

    void Update()
    {
        // Detectar Escape para pausar/reanudar (funciona incluso con tiempo pausado)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // No pausar si el menú de level up está visible
            if (levelUpUI != null && levelUpUI.levelUpPanel != null && levelUpUI.levelUpPanel.activeSelf)
            {
                // El LevelUpUI está visible, no hacer nada
                return;
            }

            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
        
        // Procesar clics y hover manualmente si el tiempo está pausado
        if (isPaused && pausePanel != null && pausePanel.activeSelf)
        {
            ProcessManualInput();
        }
    }
    
    void ProcessManualInput()
    {
        if (UnityEngine.EventSystems.EventSystem.current == null) return;
        
        // Crear un raycast manual
        PointerEventData pointerData = new PointerEventData(UnityEngine.EventSystems.EventSystem.current);
        pointerData.position = Input.mousePosition;
        
        var results = new System.Collections.Generic.List<RaycastResult>();
        UnityEngine.EventSystems.EventSystem.current.RaycastAll(pointerData, results);
        
        // Procesar hover
        Button hoveredButton = null;
        foreach (var result in results)
        {
            Button btn = result.gameObject.GetComponent<Button>();
            if (btn != null && btn.interactable)
            {
                hoveredButton = btn;
                break;
            }
        }
        
        // Actualizar hover en todos los botones
        UpdateButtonHover(resumeButton, hoveredButton == resumeButton);
        UpdateButtonHover(optionsButton, hoveredButton == optionsButton);
        UpdateButtonHover(mainMenuButton, hoveredButton == mainMenuButton);
        UpdateButtonHover(quitButton, hoveredButton == quitButton);
        
        // Procesar clics
        if (Input.GetMouseButtonDown(0) && hoveredButton != null)
        {
            hoveredButton.onClick.Invoke();
        }
    }
    
    void UpdateButtonHover(Button button, bool isHovered)
    {
        if (button == null) return;
        
        Image buttonImage = button.GetComponent<Image>();
        if (buttonImage == null) return;
        
        if (isHovered)
        {
            buttonImage.color = button.colors.highlightedColor;
        }
        else
        {
            buttonImage.color = button.colors.normalColor;
        }
    }

    public void PauseGame()
    {
        if (isPaused) return;

        isPaused = true;

        if (pausePanel != null)
        {
            pausePanel.SetActive(true);
        }

        // Asegurar que los botones estén configurados
        SetupButtons();

        // Asegurar que el EventSystem esté activo ANTES de pausar el tiempo
        UnityEngine.EventSystems.EventSystem eventSystem = UnityEngine.EventSystems.EventSystem.current;
        if (eventSystem == null)
        {
            GameObject eventSystemObj = new GameObject("EventSystem");
            eventSystem = eventSystemObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
        }
        
        if (eventSystem != null)
        {
            eventSystem.enabled = true;
            
            // Asegurar que existe UnscaledTimeInputModule
            UnscaledTimeInputModule unscaledInput = eventSystem.GetComponent<UnscaledTimeInputModule>();
            UnityEngine.EventSystems.StandaloneInputModule standardInput = eventSystem.GetComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            
            if (unscaledInput == null)
            {
                // Si existe un StandaloneInputModule estándar, deshabilitarlo pero no destruirlo
                if (standardInput != null)
                {
                    standardInput.enabled = false;
                }
                unscaledInput = eventSystem.gameObject.AddComponent<UnscaledTimeInputModule>();
            }
            
            // Asegurar que el módulo esté habilitado
            if (unscaledInput != null)
            {
                unscaledInput.enabled = true;
            }
        }
        
        // Asegurar que el Canvas tenga el GraphicRaycaster habilitado
        Canvas canvas = pausePanel.GetComponentInParent<Canvas>();
        if (canvas != null)
        {
            GraphicRaycaster canvasRaycaster = canvas.GetComponent<GraphicRaycaster>();
            if (canvasRaycaster == null)
            {
                canvasRaycaster = canvas.gameObject.AddComponent<GraphicRaycaster>();
            }
            if (canvasRaycaster != null)
            {
                canvasRaycaster.enabled = true;
            }
        }

        if (pauseGameTime)
        {
            Time.timeScale = 0f;
        }

        // Mostrar el cursor
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void ResumeGame()
    {
        if (!isPaused) return;

        isPaused = false;

        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }

        if (pauseGameTime)
        {
            Time.timeScale = 1f;
        }

        // Ocultar el cursor si hay un crosshair
        CrosshairUI crosshair = FindFirstObjectByType<CrosshairUI>();
        if (crosshair != null && crosshair.hideMouseCursor)
        {
            Cursor.visible = false;
        }
    }

    public void GoToMainMenu()
    {
        // Reanudar el tiempo antes de cambiar de escena
        Time.timeScale = 1f;
        
        // Buscar el SceneManager o cargar la escena del menú principal
        GameSceneManager sceneManager = FindFirstObjectByType<GameSceneManager>();
        if (sceneManager != null)
        {
            sceneManager.LoadMainMenu();
        }
        else
        {
            // Fallback: verificar si la escena existe antes de cargarla
            string sceneName = "MainMenu";
            bool sceneExists = false;
            for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
            {
                string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
                string sceneNameInBuild = System.IO.Path.GetFileNameWithoutExtension(scenePath);
                if (sceneNameInBuild == sceneName)
                {
                    sceneExists = true;
                    break;
                }
            }
            
            if (sceneExists)
            {
                SceneManager.LoadScene(sceneName);
            }
            else
            {
                Debug.LogError($"PauseMenu: La escena '{sceneName}' no está en el Build Settings. " +
                              "Por favor, agrega la escena en File -> Build Settings -> Add Open Scenes o arrastra la escena a la lista.");
            }
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
        // TODO: Implementar menú de opciones
        Debug.Log("Opciones - Por implementar");
    }

    public bool IsPaused()
    {
        return isPaused;
    }
}

