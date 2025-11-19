using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Script helper para crear automáticamente el menú de pausa.
/// Coloca este script en un GameObject vacío en la escena y ejecuta "Setup Pause Menu" desde el menú contextual.
/// </summary>
[RequireComponent(typeof(Canvas))]
public class PauseMenuSetup : MonoBehaviour
{
    [ContextMenu("Setup Pause Menu")]
    public void SetupPauseMenu()
    {
        Canvas canvas = GetComponent<Canvas>();
        if (canvas == null)
        {
            canvas = gameObject.AddComponent<Canvas>();
        }

        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 99; // Por debajo del crosshair (101) pero por encima del juego

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
        raycaster.enabled = true; // Asegurar que esté habilitado

        // Asegurar que existe un EventSystem
        UnityEngine.EventSystems.EventSystem eventSystem = UnityEngine.EventSystems.EventSystem.current;
        if (eventSystem == null)
        {
            GameObject eventSystemObj = new GameObject("EventSystem");
            eventSystem = eventSystemObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
        }
        
        // Usar UnscaledTimeInputModule en lugar de StandaloneInputModule para que funcione con tiempo pausado
        UnscaledTimeInputModule unscaledInputModule = eventSystem.GetComponent<UnscaledTimeInputModule>();
        UnityEngine.EventSystems.StandaloneInputModule standardInputModule = eventSystem.GetComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        
        // Si existe un StandaloneInputModule estándar, reemplazarlo
        if (standardInputModule != null && unscaledInputModule == null)
        {
            DestroyImmediate(standardInputModule);
        }
        
        if (unscaledInputModule == null)
        {
            unscaledInputModule = eventSystem.gameObject.AddComponent<UnscaledTimeInputModule>();
        }

        // Buscar o crear PauseMenu component
        PauseMenu pauseMenu = FindFirstObjectByType<PauseMenu>();
        if (pauseMenu == null)
        {
            GameObject pauseMenuObj = new GameObject("PauseMenu");
            pauseMenuObj.transform.SetParent(transform, false);
            pauseMenu = pauseMenuObj.AddComponent<PauseMenu>();
        }

        // Crear panel principal
        GameObject panel = new GameObject("PausePanel");
        panel.transform.SetParent(transform, false);
        panel.SetActive(false);
        RectTransform panelRect = panel.AddComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.sizeDelta = Vector2.zero;
        panelRect.anchoredPosition = Vector2.zero;

        Image panelBg = panel.AddComponent<Image>();
        panelBg.color = new Color(0, 0, 0, 0.8f); // Fondo semi-transparente oscuro
        panelBg.raycastTarget = true; // Asegurar que reciba raycasts (para bloquear clics detrás)

        // Contenedor vertical para los botones
        GameObject containerObj = new GameObject("ButtonContainer");
        containerObj.transform.SetParent(panel.transform, false);
        RectTransform containerRect = containerObj.AddComponent<RectTransform>();
        containerRect.anchorMin = new Vector2(0.5f, 0.5f);
        containerRect.anchorMax = new Vector2(0.5f, 0.5f);
        containerRect.sizeDelta = new Vector2(400, 500);
        containerRect.anchoredPosition = Vector2.zero;

        VerticalLayoutGroup layout = containerObj.AddComponent<VerticalLayoutGroup>();
        layout.spacing = 20;
        layout.padding = new RectOffset(20, 20, 20, 20);
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.childControlWidth = true;
        layout.childControlHeight = false;
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = false;

        // Título
        GameObject titleObj = new GameObject("Title");
        titleObj.transform.SetParent(containerObj.transform, false);
        TextMeshProUGUI titleText = titleObj.AddComponent<TextMeshProUGUI>();
        titleText.text = "PAUSA";
        titleText.fontSize = 48;
        titleText.alignment = TextAlignmentOptions.Center;
        titleText.color = Color.white;
        titleText.fontStyle = FontStyles.Bold;

        RectTransform titleRect = titleObj.GetComponent<RectTransform>();
        titleRect.sizeDelta = new Vector2(0, 80);

        // Botón Reanudar
        Button resumeBtn = CreateButton("Reanudar", containerObj.transform);
        pauseMenu.resumeButton = resumeBtn;

        // Botón Opciones
        Button optionsBtn = CreateButton("Opciones", containerObj.transform);
        pauseMenu.optionsButton = optionsBtn;

        // Botón Menú Principal
        Button mainMenuBtn = CreateButton("Menú Principal", containerObj.transform);
        pauseMenu.mainMenuButton = mainMenuBtn;

        // Botón Salir
        Button quitBtn = CreateButton("Salir", containerObj.transform);
        pauseMenu.quitButton = quitBtn;

        // Asignar el panel al PauseMenu
        pauseMenu.pausePanel = panel;

        Debug.Log("Menú de pausa creado exitosamente! Presiona Escape para pausar/reanudar.");
    }

    Button CreateButton(string text, Transform parent)
    {
        GameObject buttonObj = new GameObject($"Button_{text}");
        buttonObj.transform.SetParent(parent, false);

        RectTransform rect = buttonObj.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(0, 60);

        Image bg = buttonObj.AddComponent<Image>();
        bg.color = new Color(0.2f, 0.2f, 0.3f, 1f);
        bg.raycastTarget = true; // Asegurar que reciba raycasts

        Button button = buttonObj.AddComponent<Button>();
        button.targetGraphic = bg; // IMPORTANTE: Asignar el targetGraphic
        button.interactable = true; // Asegurar que sea interactuable
        
        ColorBlock colors = button.colors;
        Color baseColor = new Color(0.2f, 0.2f, 0.3f, 1f);
        Color highlightColor = new Color(0.4f, 0.5f, 0.7f, 1f); // Azul brillante al hacer hover
        colors.normalColor = baseColor;
        colors.highlightedColor = highlightColor;
        colors.pressedColor = new Color(0.3f, 0.4f, 0.6f, 1f); // Un poco más oscuro al presionar
        colors.selectedColor = highlightColor; // Mismo que highlighted
        colors.disabledColor = new Color(0.1f, 0.1f, 0.1f, 0.5f);
        colors.colorMultiplier = 1f;
        colors.fadeDuration = 0.15f; // Transición suave
        button.colors = colors;
        button.transition = Selectable.Transition.ColorTint;
        
        // Agregar el handler manual para que funcione incluso con tiempo pausado
        PauseMenuButtonHandler handler = buttonObj.AddComponent<PauseMenuButtonHandler>();

        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform, false);
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;
        textRect.anchoredPosition = Vector2.zero;

        TextMeshProUGUI buttonText = textObj.AddComponent<TextMeshProUGUI>();
        buttonText.text = text;
        buttonText.fontSize = 24;
        buttonText.alignment = TextAlignmentOptions.Center;
        buttonText.color = Color.white;
        buttonText.fontStyle = FontStyles.Bold;
        buttonText.raycastTarget = false; // IMPORTANTE: El texto no debe bloquear los clics

        return button;
    }
}

