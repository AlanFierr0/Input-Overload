using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Script helper para crear automáticamente la UI de Level Up si no existe.
/// Coloca este script en un GameObject vacío en la escena.
/// </summary>
[RequireComponent(typeof(Canvas))]
public class LevelUpUISetup : MonoBehaviour
{
    [ContextMenu("Setup Level Up UI")]
    public void SetupLevelUpUI()
    {
        Canvas canvas = GetComponent<Canvas>();
        if (canvas == null)
        {
            canvas = gameObject.AddComponent<Canvas>();
        }

        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100; // Asegurar que esté por encima de todo

        CanvasScaler scaler = GetComponent<CanvasScaler>();
        if (scaler == null)
        {
            scaler = gameObject.AddComponent<CanvasScaler>();
        }
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        GraphicRaycaster raycaster = GetComponent<GraphicRaycaster>();
        if (raycaster == null)
        {
            raycaster = gameObject.AddComponent<GraphicRaycaster>();
        }
        
        // Asegurar que existe un EventSystem para los botones
        if (UnityEngine.EventSystems.EventSystem.current == null)
        {
            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }

        // Buscar o crear LevelUpUI component
        LevelUpUI levelUpUI = FindFirstObjectByType<LevelUpUI>();
        if (levelUpUI == null)
        {
            GameObject uiObj = new GameObject("LevelUpUI");
            uiObj.transform.SetParent(transform, false);
            levelUpUI = uiObj.AddComponent<LevelUpUI>();
        }

        // Crear panel principal
        GameObject panel = new GameObject("LevelUpPanel");
        panel.transform.SetParent(transform, false);
        panel.SetActive(false); // Asegurar que esté oculto al inicio
        RectTransform panelRect = panel.AddComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.sizeDelta = Vector2.zero;
        panelRect.anchoredPosition = Vector2.zero;

        Image panelBg = panel.AddComponent<Image>();
        panelBg.color = new Color(0, 0, 0, 0.7f); // Fondo semi-transparente

        // Título
        GameObject titleObj = new GameObject("Title");
        titleObj.transform.SetParent(panel.transform, false);
        RectTransform titleRect = titleObj.AddComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.5f, 0.8f);
        titleRect.anchorMax = new Vector2(0.5f, 0.8f);
        titleRect.sizeDelta = new Vector2(600, 100);
        titleRect.anchoredPosition = Vector2.zero;

        TextMeshProUGUI titleText = titleObj.AddComponent<TextMeshProUGUI>();
        titleText.text = "¡SUBISTE DE NIVEL!";
        titleText.fontSize = 48;
        titleText.alignment = TextAlignmentOptions.Center;
        titleText.color = Color.yellow;
        titleText.fontStyle = FontStyles.Bold;

        // Contenedor de opciones
        GameObject containerObj = new GameObject("AbilityOptionsContainer");
        containerObj.transform.SetParent(panel.transform, false);
        RectTransform containerRect = containerObj.AddComponent<RectTransform>();
        containerRect.anchorMin = new Vector2(0.5f, 0.5f);
        containerRect.anchorMax = new Vector2(0.5f, 0.5f);
        containerRect.sizeDelta = new Vector2(1200, 400);
        containerRect.anchoredPosition = Vector2.zero;

        HorizontalLayoutGroup layout = containerObj.AddComponent<HorizontalLayoutGroup>();
        layout.spacing = 20;
        layout.padding = new RectOffset(20, 20, 20, 20);
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.childControlWidth = false;
        layout.childControlHeight = false;
        layout.childForceExpandWidth = false;
        layout.childForceExpandHeight = false;

        ContentSizeFitter fitter = containerObj.AddComponent<ContentSizeFitter>();
        fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        // Asignar referencias al LevelUpUI
        levelUpUI.levelUpPanel = panel;
        levelUpUI.levelUpTitle = titleText;
        levelUpUI.abilityOptionsContainer = containerObj.transform;

        // Buscar y asignar automáticamente al AbilityManager si existe
        AbilityManager abilityManager = FindFirstObjectByType<AbilityManager>();
        if (abilityManager != null && abilityManager.levelUpUI == null)
        {
            abilityManager.levelUpUI = levelUpUI;
            levelUpUI.Initialize(abilityManager);
            Debug.Log("LevelUpUI automatically assigned to AbilityManager!");
        }

        Debug.Log("Level Up UI setup complete!");
    }

    void Awake()
    {
        // Auto-setup si no hay UI configurada
        LevelUpUI levelUpUI = FindFirstObjectByType<LevelUpUI>();
        if (levelUpUI != null && levelUpUI.levelUpPanel == null)
        {
            SetupLevelUpUI();
        }
    }
}

