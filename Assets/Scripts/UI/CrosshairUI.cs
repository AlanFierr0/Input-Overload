using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Script que maneja la mira en pantalla que sigue el cursor del mouse.
/// Debe estar en un GameObject hijo del Canvas.
/// </summary>
public class CrosshairUI : MonoBehaviour
{
    [Header("Crosshair Settings")]
    [Tooltip("Imagen de la mira (se creará automáticamente si está vacío)")]
    public Image crosshairImage;
    
    [Tooltip("Color de la mira")]
    public Color crosshairColor = Color.white;
    
    [Tooltip("Tamaño de la mira en píxeles")]
    public Vector2 crosshairSize = new Vector2(60, 60);
    
    [Tooltip("Distancia desde el centro de la pantalla (0 = sigue el mouse exactamente)")]
    public float offsetFromCenter = 0f;
    
    [Tooltip("Ocultar el cursor del mouse (reemplazarlo con la mira)")]
    public bool hideMouseCursor = true;
    
    private RectTransform rectTransform;
    private Canvas parentCanvas;
    private Camera mainCamera;
    private int originalSortingOrder = -1;

    void Start()
    {
        // Obtener o crear la imagen de la mira
        if (crosshairImage == null)
        {
            crosshairImage = GetComponent<Image>();
            if (crosshairImage == null)
            {
                GameObject imageObj = new GameObject("CrosshairImage");
                imageObj.transform.SetParent(transform, false);
                crosshairImage = imageObj.AddComponent<Image>();
            }
        }

        rectTransform = GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            rectTransform = gameObject.AddComponent<RectTransform>();
        }

        // Buscar el canvas padre
        parentCanvas = GetComponentInParent<Canvas>();
        if (parentCanvas == null)
        {
            parentCanvas = FindFirstObjectByType<Canvas>();
        }
        
        // Asegurar que el crosshair esté por encima de todo (incluyendo la UI de habilidades)
        if (parentCanvas != null)
        {
            originalSortingOrder = parentCanvas.sortingOrder;
            // El LevelUpUI usa sortingOrder 100, así que usamos 101 o más para estar por encima
            parentCanvas.sortingOrder = Mathf.Max(101, originalSortingOrder + 1);
        }

        // Obtener la cámara principal
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            mainCamera = FindFirstObjectByType<Camera>();
        }

        // Configurar la imagen de la mira
        SetupCrosshairImage();
        
        // Ocultar el cursor del mouse si está habilitado
        if (hideMouseCursor)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.None; // Mantener el mouse desbloqueado para que funcione
        }
    }
    
    void OnDestroy()
    {
        // Restaurar el cursor cuando se destruye el objeto
        if (hideMouseCursor)
        {
            Cursor.visible = true;
        }
        
        // Restaurar el sorting order original del canvas si es necesario
        if (parentCanvas != null && originalSortingOrder >= 0)
        {
            parentCanvas.sortingOrder = originalSortingOrder;
        }
    }

    void SetupCrosshairImage()
    {
        if (crosshairImage != null)
        {
            crosshairImage.color = crosshairColor;
            
            RectTransform imageRect = crosshairImage.GetComponent<RectTransform>();
            if (imageRect != null)
            {
                imageRect.sizeDelta = crosshairSize;
                imageRect.anchoredPosition = Vector2.zero;
            }

            // Si no hay sprite, crear uno simple (cruz más grande)
            if (crosshairImage.sprite == null)
            {
                crosshairImage.sprite = CreateCrosshairSprite();
            }
        }
    }

    void Update()
    {
        UpdateCrosshairPosition();
    }

    void UpdateCrosshairPosition()
    {
        if (rectTransform == null || parentCanvas == null) return;

        Vector2 mousePosition = Input.mousePosition;
        
        // Convertir la posición del mouse a coordenadas del canvas
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentCanvas.transform as RectTransform,
            mousePosition,
            parentCanvas.worldCamera,
            out localPoint
        );

        // Aplicar offset si es necesario
        if (offsetFromCenter > 0f)
        {
            Vector2 center = Vector2.zero;
            Vector2 direction = (localPoint - center).normalized;
            localPoint = center + direction * offsetFromCenter;
        }

        rectTransform.anchoredPosition = localPoint;
    }

    /// <summary>
    /// Obtiene la posición del mouse en el mundo (2D)
    /// </summary>
    public Vector2 GetMouseWorldPosition()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
            if (mainCamera == null)
            {
                return Vector2.zero;
            }
        }

        Vector3 mousePos = Input.mousePosition;
        mousePos.z = mainCamera.nearClipPlane + 1f; // Distancia desde la cámara
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(mousePos);
        return new Vector2(worldPos.x, worldPos.y);
    }

    /// <summary>
    /// Obtiene la dirección desde una posición hacia el mouse (normalizada)
    /// </summary>
    public Vector2 GetDirectionFromPosition(Vector2 fromPosition)
    {
        Vector2 mouseWorldPos = GetMouseWorldPosition();
        Vector2 direction = (mouseWorldPos - fromPosition).normalized;
        return direction;
    }
    
    /// <summary>
    /// Crea un sprite de cruz más grande y visible
    /// </summary>
    Sprite CreateCrosshairSprite()
    {
        int size = 64; // Textura más grande
        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        Color[] pixels = new Color[size * size];

        // Rellenar con transparente
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = Color.clear;
        }

        int center = size / 2;
        int thickness = 4; // Líneas más gruesas
        int length = 20; // Líneas más largas

        // Dibujar línea horizontal
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

        // Dibujar línea vertical
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

