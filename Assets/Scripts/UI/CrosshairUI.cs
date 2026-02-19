using UnityEngine;
using UnityEngine.UI;

public class CrosshairUI : MonoBehaviour
{
    public Image crosshairImage;
    public bool useNativeSize = false; // Si es true, usa el tamaño original de la imagen
    public Vector2 crosshairSize = new Vector2(64, 64); // Tamaño del crosshair (solo si useNativeSize es false)
    public float offsetFromCenter = 0f;
    public bool hideMouseCursor = true;
    
    private RectTransform rectTransform;
    private Canvas parentCanvas;
    private Camera mainCamera;
    
    private static CrosshairUI instance;

    void Awake()
    {
        // Implementar Singleton para que persista entre escenas
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        instance = this;
        
        // Hacer que el crosshair persista entre escenas
        // Buscar el canvas raíz para hacerlo persistente
        Transform root = transform.root;
        DontDestroyOnLoad(root.gameObject);
    }

    void Start()
    {
        if (crosshairImage == null) crosshairImage = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
        mainCamera = Camera.main;
        
        // Obtener o crear canvas para el crosshair
        parentCanvas = GetComponentInParent<Canvas>();
        
        if (parentCanvas == null)
        {
            // Si no hay canvas padre, crear uno
            GameObject canvasObj = new GameObject("CrosshairCanvas");
            parentCanvas = canvasObj.AddComponent<Canvas>();
            parentCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            parentCanvas.sortingOrder = 32767;
            parentCanvas.overrideSorting = true;
            
            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            
            canvasObj.AddComponent<GraphicRaycaster>();
            
            transform.SetParent(canvasObj.transform, false);
            rectTransform = GetComponent<RectTransform>();
            parentCanvas = GetComponentInParent<Canvas>();
        }
        else
        {
            // Asegurar que tenga prioridad máxima
            parentCanvas.sortingOrder = 32767;
            parentCanvas.overrideSorting = true;
        }

        SetupCrosshairImage();
        
        if (hideMouseCursor)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.None;
        }
    }
    
    void OnDestroy()
    {
        // NO restaurar el cursor visible - el crosshair debe ser permanente
        // Como el canvas es independiente y persistente, no necesitamos restaurar nada
    }

    void SetupCrosshairImage()
    {
        // Validar que el componente Image está asignado
        if (crosshairImage == null)
        {
            Debug.LogError("CrosshairUI: El componente 'Crosshair Image' no está asignado! Asigna un componente Image en el Inspector.");
            enabled = false;
            return;
        }
        
        // Validar que hay un sprite asignado a la imagen
        if (crosshairImage.sprite == null)
        {
            Debug.LogError("CrosshairUI: No hay ningún sprite asignado al Image! Debes asignar una imagen PNG en el campo 'Source Image' del componente Image.");
            enabled = false;
            return;
        }
        
        // Detectar si está usando el sprite por defecto de Unity (círculo blanco)
        if (crosshairImage.sprite.name == "UISprite" || crosshairImage.sprite.name.Contains("Knob"))
        {
            Debug.LogError($"CrosshairUI: Estás usando el sprite por defecto de Unity '{crosshairImage.sprite.name}'. Debes asignar TU imagen PNG personalizada en el campo 'Source Image' del componente Image!");
            enabled = false;
            return;
        }
        
        // Usar color blanco para mantener los colores originales del sprite
        crosshairImage.color = Color.white;
        
        // No bloquear raycast para que los botones funcionen
        crosshairImage.raycastTarget = false;
        
        // Asegurar que la imagen esté activa
        crosshairImage.gameObject.SetActive(true);
        crosshairImage.enabled = true;
        
        // Configurar tamaño y posición
        RectTransform imageRect = crosshairImage.GetComponent<RectTransform>();
        if (imageRect != null)
        {
            if (useNativeSize)
            {
                // Usar el tamaño original de la imagen
                crosshairImage.SetNativeSize();
            }
            else
            {
                // Usar el tamaño personalizado
                imageRect.sizeDelta = crosshairSize;
            }
            imageRect.anchoredPosition = Vector2.zero;
        }
        
        // Asegurar que el crosshair esté en el layer más alto de UI
        crosshairImage.transform.SetAsLastSibling();
    }

    void Update()
    {
        if (rectTransform == null || parentCanvas == null) return;

        // Verificar si el Game Over está activo
        GameOverManager gameOverManager = FindFirstObjectByType<GameOverManager>();
        bool isGameOver = gameOverManager != null && gameOverManager.IsGameOver();
        
        // Asegurar que el cursor del sistema esté siempre oculto (excepto durante Game Over)
        if (hideMouseCursor && !isGameOver)
        {
            if (Cursor.visible)
            {
                Cursor.visible = false;
            }
        }

        // Asegurar que el sorting order siempre sea el máximo (por si otro canvas intenta tomar prioridad)
        if (parentCanvas.sortingOrder != 32767)
        {
            parentCanvas.sortingOrder = 32767;
        }

        // Asegurar que el crosshair esté siempre activo y visible
        if (crosshairImage != null && !crosshairImage.gameObject.activeSelf)
        {
            crosshairImage.gameObject.SetActive(true);
        }

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentCanvas.transform as RectTransform,
            Input.mousePosition,
            parentCanvas.worldCamera,
            out Vector2 localPoint
        );

        if (offsetFromCenter > 0f)
        {
            localPoint = localPoint.normalized * offsetFromCenter;
        }

        rectTransform.anchoredPosition = localPoint;
    }

    void LateUpdate()
    {
        // Actualizar al final del frame para asegurar que esté siempre visible sobre todo lo demás
        if (crosshairImage != null)
        {
            crosshairImage.transform.SetAsLastSibling();
        }
        
        // Asegurar que el canvas siempre tenga el sorting order máximo
        if (parentCanvas != null)
        {
            // FORZAR AGRESIVAMENTE el sorting order máximo
            parentCanvas.sortingOrder = 32767;
            parentCanvas.overrideSorting = true;
            
            // CRÍTICO: Forzar que este canvas esté después de TODOS los demás canvas en la escena
            // Buscar todos los Canvas y asegurar que el crosshair esté al final
            Canvas[] allCanvases = FindObjectsByType<Canvas>(FindObjectsSortMode.None);
            
            // Primero, bajar cualquier canvas que esté en 32767 o más (excepto este)
            foreach (Canvas canvas in allCanvases)
            {
                if (canvas != parentCanvas && canvas.sortingOrder >= 32767)
                {
                    canvas.sortingOrder = 100; // Forzar por debajo del crosshair
                }
            }
            
            // Ahora asegurar que este canvas esté al final de la jerarquía
            Transform canvasParent = parentCanvas.transform.parent;
            int maxSiblingIndex = -1;
            
            foreach (Canvas canvas in allCanvases)
            {
                // Solo comparar con canvas que tengan el mismo padre (o sean raíz como nosotros)
                if (canvas != parentCanvas && canvas.transform.parent == canvasParent)
                {
                    int siblingIndex = canvas.transform.GetSiblingIndex();
                    if (siblingIndex > maxSiblingIndex)
                    {
                        maxSiblingIndex = siblingIndex;
                    }
                }
            }
            
            // Colocar el crosshair después del último
            if (maxSiblingIndex >= 0 && parentCanvas.transform.GetSiblingIndex() <= maxSiblingIndex)
            {
                parentCanvas.transform.SetAsLastSibling();
            }
        }
    }

    public Vector2 GetMouseWorldPosition()
    {
        if (mainCamera == null) mainCamera = Camera.main;
        if (mainCamera == null) return Vector2.zero;

        Vector3 mousePos = Input.mousePosition;
        mousePos.z = mainCamera.nearClipPlane + 1f;
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(mousePos);
        return new Vector2(worldPos.x, worldPos.y);
    }

    public Vector2 GetDirectionFromPosition(Vector2 fromPosition)
    {
        return (GetMouseWorldPosition() - fromPosition).normalized;
    }

    /// <summary>
    /// Fuerza el crosshair al frente de todos los canvas.
    /// Llamar este método cada vez que se muestre una nueva UI para asegurar que el crosshair esté visible.
    /// </summary>
    public void ForceToFront()
    {
        if (parentCanvas == null) parentCanvas = GetComponentInParent<Canvas>();
        
        if (parentCanvas != null)
        {
            parentCanvas.sortingOrder = 32767;
            parentCanvas.overrideSorting = true;
            parentCanvas.transform.SetAsLastSibling();
        }

        if (crosshairImage != null)
        {
            crosshairImage.transform.SetAsLastSibling();
        }
    }

    /// <summary>
    /// Obtiene la instancia singleton del crosshair.
    /// </summary>
    public static CrosshairUI GetInstance()
    {
        return instance;
    }
}

