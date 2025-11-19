using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manager para gestionar cambios de escena en el juego.
/// </summary>
public class GameSceneManager : MonoBehaviour
{
    [Header("Scene Names")]
    [Tooltip("Nombre de la escena del menú principal")]
    public string mainMenuSceneName = "MainMenu";
    
    [Tooltip("Nombre de la escena del juego")]
    public string gameSceneName = "SampleScene";

    private static GameSceneManager _instance;
    public static GameSceneManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<GameSceneManager>();
            }
            return _instance;
        }
    }

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Carga la escena del menú principal
    /// </summary>
    public void LoadMainMenu()
    {
        Time.timeScale = 1f; // Asegurar que el tiempo esté normalizado
        
        // Verificar si la escena existe en el build
        if (SceneExistsInBuild(mainMenuSceneName))
        {
            SceneManager.LoadScene(mainMenuSceneName);
        }
        else
        {
            Debug.LogError($"GameSceneManager: La escena '{mainMenuSceneName}' no está en el Build Settings. " +
                          "Por favor, agrega la escena en File -> Build Settings -> Add Open Scenes o arrastra la escena a la lista.");
        }
    }
    
    /// <summary>
    /// Verifica si una escena existe en el build
    /// </summary>
    bool SceneExistsInBuild(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneNameInBuild = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            if (sceneNameInBuild == sceneName)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Carga la escena del juego
    /// </summary>
    public void LoadGame()
    {
        Time.timeScale = 1f; // Asegurar que el tiempo esté normalizado
        
        // Verificar si la escena existe en el build
        if (SceneExistsInBuild(gameSceneName))
        {
            SceneManager.LoadScene(gameSceneName);
        }
        else
        {
            Debug.LogError($"GameSceneManager: La escena '{gameSceneName}' no está en el Build Settings. " +
                          "Por favor, agrega la escena en File -> Build Settings -> Add Open Scenes o arrastra la escena a la lista.");
        }
    }

    /// <summary>
    /// Recarga la escena actual
    /// </summary>
    public void ReloadCurrentScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// Carga una escena por nombre
    /// </summary>
    public void LoadScene(string sceneName)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }
}

