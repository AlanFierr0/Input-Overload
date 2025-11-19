using UnityEngine;
using UnityEngine.UI;

public class CrosshairUI : MonoBehaviour
{
    public Image crosshairImage;
    public Color crosshairColor = Color.white;
    public Vector2 crosshairSize = new Vector2(80, 80); // Aumentado de 60 a 80
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
            
            Debug.Log("CrosshairUI: Created independent canvas with sorting order 32767");
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
        if (crosshairImage == null) return;
        
        crosshairImage.color = crosshairColor;
        crosshairImage.raycastTarget = false; // No bloquear raycast para que los botones funcionen
        
        RectTransform imageRect = crosshairImage.GetComponent<RectTransform>();
        if (imageRect != null)
        {
            imageRect.sizeDelta = crosshairSize;
            imageRect.anchoredPosition = Vector2.zero;
        }

        if (crosshairImage.sprite == null) crosshairImage.sprite = CreateCrosshairSprite();
        
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
    
    Sprite CreateCrosshairSprite()
    {
        int size = 64;
        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        Color[] pixels = new Color[size * size];

        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = Color.clear;
        }

        int center = size / 2;
        int thickness = 4;
        int length = 20;

        for (int x = center - length; x <= center + length; x++)
        {
            for (int y = center - thickness / 2; y <= center + thickness / 2; y++)
            {
                if (x >= 0 && x < size && y >= 0 && y < size)
                {
                    pixels[y * size + x] = crosshairColor;
                }
            }
        }

        for (int y = center - length; y <= center + length; y++)
        {
            for (int x = center - thickness / 2; x <= center + thickness / 2; x++)
            {
                if (x >= 0 && x < size && y >= 0 && y < size)
                {
                    pixels[y * size + x] = crosshairColor;
                }
            }
        }

        texture.SetPixels(pixels);
        texture.Apply();
        return Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
    }
}

