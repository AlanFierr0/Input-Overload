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
    private int originalSortingOrder = -1;

    void Start()
    {
        if (crosshairImage == null) crosshairImage = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
        mainCamera = Camera.main;
        
        // Crear un canvas independiente para el crosshair si no existe uno apropiado
        parentCanvas = GetComponentInParent<Canvas>();
        
        // Si el canvas padre tiene sorting order bajo o está compartido, crear uno nuevo
        if (parentCanvas == null || parentCanvas.sortingOrder < 32767)
        {
            // Crear un nuevo GameObject para el canvas del crosshair
            GameObject canvasObj = new GameObject("CrosshairCanvas");
            parentCanvas = canvasObj.AddComponent<Canvas>();
            parentCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            parentCanvas.sortingOrder = 32767; // Máximo sorting order
            parentCanvas.overrideSorting = true;
            
            // Añadir CanvasScaler
            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            
            // Añadir GraphicRaycaster
            canvasObj.AddComponent<GraphicRaycaster>();
            
            // Mover este GameObject al nuevo canvas
            transform.SetParent(canvasObj.transform, false);
            
            // Actualizar rectTransform después de mover
            rectTransform = GetComponent<RectTransform>();
        }
        else
        {
            originalSortingOrder = parentCanvas.sortingOrder;
            // Asegurar que el crosshair esté SIEMPRE por encima de TODO con el sorting order más alto posible
            parentCanvas.sortingOrder = 32767; // Valor máximo de sorting order
            parentCanvas.overrideSorting = true; // Forzar que este canvas override cualquier otro
        }
        
        // Re-obtener parentCanvas después de posibles cambios
        parentCanvas = GetComponentInParent<Canvas>();

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
        // if (hideMouseCursor) Cursor.visible = true;
        if (parentCanvas != null && originalSortingOrder >= 0) parentCanvas.sortingOrder = originalSortingOrder;
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

        // Asegurar que el cursor del sistema esté siempre oculto
        if (Cursor.visible)
        {
            Cursor.visible = false;
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
        if (parentCanvas != null && parentCanvas.sortingOrder != 32767)
        {
            parentCanvas.sortingOrder = 32767;
            parentCanvas.overrideSorting = true;
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
}

