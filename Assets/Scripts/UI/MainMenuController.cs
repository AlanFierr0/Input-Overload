using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Controlador simple para el menú principal.
/// Arrastra este script a tu Canvas y asigna los botones desde el Inspector.
/// </summary>
public class MainMenuController : MonoBehaviour
{
    [Header("Botones del Menú")]
    [Tooltip("Arrastra aquí el botón de Start desde la Hierarchy")]
    public Button botonStart;
    
    [Tooltip("Arrastra aquí el botón de Options desde la Hierarchy")]
    public Button botonOptions;
    
    [Tooltip("Arrastra aquí el botón de Credits desde la Hierarchy")]
    public Button botonCredits;
    
    [Tooltip("Arrastra aquí el botón de Quit desde la Hierarchy")]
    public Button botonQuit;
    
    [Header("Configuración")]
    [Tooltip("Nombre de la escena del juego (debe estar en Build Settings)")]
    public string nombreEscenaJuego = "SampleScene";
    
    [Tooltip("Nombre de la escena de créditos (debe estar en Build Settings)")]
    public string nombreEscenaCreditos = "Credits";

    [Header("Audio")]
    [Tooltip("Música de fondo para el menú principal")]
    public AudioClip mainMenuBGM;

    void Start()
    {
        // Usar el crosshair personalizado en lugar del cursor del sistema
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.None;
        
        // Asegurar que el tiempo esté corriendo
        Time.timeScale = 1f;

        // Reproducir música del menú principal
        if (mainMenuBGM != null && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayBGM(mainMenuBGM, true);
        }

        // CRÍTICO: Crear el crosshair si no existe
        CrosshairUI crosshair = FindFirstObjectByType<CrosshairUI>();
        if (crosshair == null)
        {
            CreateCrosshair();
            crosshair = FindFirstObjectByType<CrosshairUI>();
        }
        
        if (crosshair != null)
        {
            crosshair.ForceToFront();
        }
        
        // Asegurar que el canvas del menú esté por debajo del crosshair
        Canvas menuCanvas = GetComponent<Canvas>();
        if (menuCanvas != null && menuCanvas.sortingOrder >= 32767)
        {
            menuCanvas.sortingOrder = 100;
        }
        
        // Asignar las funciones a cada botón
        if (botonStart != null)
        {
            botonStart.onClick.AddListener(IniciarJuego);
        }
        else
        {
            Debug.LogWarning("Botón Start no asignado en el Inspector");
        }
        
        if (botonOptions != null)
        {
            botonOptions.onClick.AddListener(AbrirOpciones);
        }
        else
        {
            Debug.LogWarning("Botón Options no asignado en el Inspector");
        }
        
        if (botonCredits != null)
        {
            botonCredits.onClick.AddListener(MostrarCreditos);
        }
        else
        {
            Debug.LogWarning("Botón Credits no asignado en el Inspector");
        }
        
        if (botonQuit != null)
        {
            botonQuit.onClick.AddListener(SalirDelJuego);
        }
        else
        {
            Debug.LogWarning("Botón Quit no asignado en el Inspector");
        }
    }
    
    void Update()
    {
        // Asegurar que el cursor del sistema esté siempre oculto
        if (Cursor.visible)
        {
            Cursor.visible = false;
        }

        // Forzar el crosshair al frente cada frame en el menú
        CrosshairUI crosshair = CrosshairUI.GetInstance();
        if (crosshair != null)
        {
            crosshair.ForceToFront();
        }
    }

    /// <summary>
    /// Inicia el juego cargando la escena especificada
    /// </summary>
    public void IniciarJuego()
    {
        Debug.Log("Iniciando juego - Cargando escena: " + nombreEscenaJuego);
        SceneManager.LoadScene(nombreEscenaJuego);
    }

    /// <summary>
    /// Abre el menú de opciones (por implementar)
    /// </summary>
    public void AbrirOpciones()
    {
        Debug.Log("Options - Por implementar");
        // Aquí puedes agregar código para mostrar un panel de opciones
    }

    /// <summary>
    /// Carga la escena de créditos
    /// </summary>
    public void MostrarCreditos()
    {
        Debug.Log("Cargando escena de créditos: " + nombreEscenaCreditos);
        SceneManager.LoadScene(nombreEscenaCreditos);
    }

    /// <summary>
    /// Sale del juego
    /// </summary>
    public void SalirDelJuego()
    {
        Debug.Log("Saliendo del juego...");
        
        #if UNITY_EDITOR
            // Si estás en el editor, detiene el modo Play
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            // Si es un build, cierra la aplicación
            Application.Quit();
        #endif
    }

    /// <summary>
    /// Crea el Crosshair si no existe
    /// </summary>
    private void CreateCrosshair()
    {
        Debug.Log("Creando Crosshair en Main Menu");
        
        // Crear Canvas para el Crosshair
        GameObject canvasObj = new GameObject("CrosshairCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 32767;
        canvas.overrideSorting = true;
        
        // Hacer el canvas persistente
        DontDestroyOnLoad(canvasObj);
        
        // Crear el Crosshair UI
        GameObject crosshairObj = new GameObject("Crosshair");
        crosshairObj.transform.SetParent(canvasObj.transform, false);
        
        RectTransform rectTransform = crosshairObj.AddComponent<RectTransform>();
        rectTransform.anchoredPosition = Vector2.zero;
        rectTransform.sizeDelta = new Vector2(64, 64);
        
        Image image = crosshairObj.AddComponent<Image>();
        image.color = Color.white;
        
        CrosshairUI crosshairUI = crosshairObj.AddComponent<CrosshairUI>();
        crosshairUI.crosshairImage = image;
        crosshairUI.crosshairSize = new Vector2(64, 64);
        crosshairUI.hideMouseCursor = true;
        
        Debug.Log("Crosshair creado exitosamente");
    }
}

