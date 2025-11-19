using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Script helper para crear automáticamente la mira en el Canvas.
/// Coloca este script en un GameObject vacío en la escena y ejecuta "Setup Crosshair" desde el menú contextual.
/// </summary>
public class CrosshairSetup : MonoBehaviour
{
    [ContextMenu("Setup Crosshair")]
    public void SetupCrosshair()
    {
        // Buscar si ya existe una mira
        CrosshairUI existingCrosshair = FindFirstObjectByType<CrosshairUI>();
        if (existingCrosshair != null)
        {
            Debug.LogWarning("Ya existe una mira en la escena. Elimínala primero si quieres crear una nueva.");
            return;
        }

        // Buscar o crear un Canvas específico para el crosshair
        Canvas crosshairCanvas = null;
        Canvas[] allCanvases = FindObjectsByType<Canvas>(FindObjectsSortMode.None);
        
        // Buscar un canvas existente con sorting order alto, o crear uno nuevo
        foreach (Canvas c in allCanvases)
        {
            if (c.name.Contains("Crosshair") || c.sortingOrder >= 101)
            {
                crosshairCanvas = c;
                break;
            }
        }
        
        if (crosshairCanvas == null)
        {
            // Crear un Canvas separado para el crosshair
            GameObject canvasObj = new GameObject("CrosshairCanvas");
            crosshairCanvas = canvasObj.AddComponent<Canvas>();
            crosshairCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            crosshairCanvas.sortingOrder = 101; // Por encima del LevelUpUI (100)
            
            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            
            canvasObj.AddComponent<GraphicRaycaster>();
        }
        else
        {
            // Asegurar que el canvas tenga un sorting order alto
            if (crosshairCanvas.sortingOrder < 101)
            {
                crosshairCanvas.sortingOrder = 101;
            }
        }

        // Crear el GameObject de la mira
        GameObject crosshairObj = new GameObject("Crosshair");
        crosshairObj.transform.SetParent(crosshairCanvas.transform, false);

        // Agregar RectTransform
        RectTransform rectTransform = crosshairObj.AddComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.sizeDelta = new Vector2(20, 20);
        rectTransform.anchoredPosition = Vector2.zero;

        // Agregar el componente CrosshairUI
        CrosshairUI crosshairUI = crosshairObj.AddComponent<CrosshairUI>();

        // Crear la imagen de la mira
        GameObject imageObj = new GameObject("CrosshairImage");
        imageObj.transform.SetParent(crosshairObj.transform, false);

        Image crosshairImage = imageObj.AddComponent<Image>();
        crosshairImage.color = Color.white;

        RectTransform imageRect = imageObj.GetComponent<RectTransform>();
        imageRect.anchorMin = new Vector2(0.5f, 0.5f);
        imageRect.anchorMax = new Vector2(0.5f, 0.5f);
        imageRect.pivot = new Vector2(0.5f, 0.5f);
        imageRect.sizeDelta = new Vector2(60, 60); // Tamaño más grande
        imageRect.anchoredPosition = Vector2.zero;

        // Asignar la imagen al componente CrosshairUI
        crosshairUI.crosshairImage = crosshairImage;
        crosshairUI.crosshairSize = new Vector2(60, 60); // Tamaño más grande
        crosshairUI.hideMouseCursor = true; // Ocultar el cursor del mouse

        // Crear un sprite simple de cruz más grande
        Texture2D texture = CreateCrosshairTexture();
        Sprite crosshairSprite = Sprite.Create(texture, new Rect(0, 0, 64, 64), new Vector2(0.5f, 0.5f), 64);
        crosshairImage.sprite = crosshairSprite;

        Debug.Log("¡Mira creada exitosamente! La mira seguirá el cursor del mouse y el cursor está oculto.");
    }

    /// <summary>
    /// Crea una textura simple de cruz más grande para la mira
    /// </summary>
    Texture2D CreateCrosshairTexture()
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
                    pixels[y * size + x] = Color.white;
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
                    pixels[y * size + x] = Color.white;
                }
            }
        }

        texture.SetPixels(pixels);
        texture.Apply();
        return texture;
    }
}

