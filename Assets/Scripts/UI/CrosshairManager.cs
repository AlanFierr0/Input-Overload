using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manager que asegura que siempre exista un crosshair en todas las escenas
/// Se ejecuta automáticamente al cargar cualquier escena
/// </summary>
public class CrosshairManager : MonoBehaviour
{
    private static CrosshairManager instance;
    private static GameObject crosshairCanvasObj;
    
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void OnSceneLoaded()
    {
        // Buscar si ya existe un crosshair en la escena
        CrosshairUI existingCrosshair = FindFirstObjectByType<CrosshairUI>();
        
        if (existingCrosshair == null)
        {
            // No existe crosshair, crear uno
            CreateCrosshair();
        }
        else
        {
            // Ya existe, asegurar que el cursor esté oculto
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.None;
        }
    }
    
    static void CreateCrosshair()
    {
        // Crear canvas para el crosshair
        crosshairCanvasObj = new GameObject("CrosshairCanvas_Auto");
        Canvas canvas = crosshairCanvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 32767; // Máximo sorting order
        canvas.overrideSorting = true;
        
        // Añadir CanvasScaler
        CanvasScaler scaler = crosshairCanvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        
        // Añadir GraphicRaycaster
        crosshairCanvasObj.AddComponent<GraphicRaycaster>();
        
        // No destruir al cambiar de escena
        DontDestroyOnLoad(crosshairCanvasObj);
        
        // Crear el objeto del crosshair
        GameObject crosshairObj = new GameObject("Crosshair");
        crosshairObj.transform.SetParent(crosshairCanvasObj.transform, false);
        
        // Configurar RectTransform
        RectTransform rectTransform = crosshairObj.AddComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.sizeDelta = new Vector2(120, 120);
        rectTransform.anchoredPosition = Vector2.zero;
        
        // Crear el objeto de la imagen del crosshair
        GameObject imageObj = new GameObject("CrosshairImage");
        imageObj.transform.SetParent(crosshairObj.transform, false);
        
        // Configurar RectTransform de la imagen
        RectTransform imageRect = imageObj.AddComponent<RectTransform>();
        imageRect.anchorMin = new Vector2(0.5f, 0.5f);
        imageRect.anchorMax = new Vector2(0.5f, 0.5f);
        imageRect.sizeDelta = new Vector2(120, 120);
        imageRect.anchoredPosition = Vector2.zero;
        
        // Añadir componente Image
        Image image = imageObj.AddComponent<Image>();
        image.color = Color.white;
        image.raycastTarget = false;
        
        // Añadir el componente CrosshairUI
        CrosshairUI crosshairUI = crosshairObj.AddComponent<CrosshairUI>();
        crosshairUI.crosshairImage = image;
        crosshairUI.crosshairColor = Color.white;
        crosshairUI.crosshairSize = new Vector2(120, 120);
        crosshairUI.hideMouseCursor = true;
        
        Debug.Log("CrosshairManager: Crosshair creado automáticamente en la escena");
        
        // Ocultar el cursor del sistema
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.None;
    }
}

