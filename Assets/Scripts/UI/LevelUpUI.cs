using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class LevelUpUI : MonoBehaviour
{
    [Header("UI References")]
    public GameObject levelUpPanel;
    public TextMeshProUGUI levelUpTitle;
    public Transform abilityOptionsContainer;
    public GameObject abilityOptionPrefab;

    [Header("Ability Options")]
    private List<AbilityOption> currentOptions = new List<AbilityOption>();
    private AbilityManager abilityManager;
    private bool isUIVisible = false;

    [System.Serializable]
    public class AbilityOption
    {
        public Ability ability;
        public KeyCode keybind;
    }

    void Awake()
    {
        if (levelUpPanel != null)
        {
            levelUpPanel.SetActive(false);
        }
        isUIVisible = false;
    }

    void Start()
    {
        // Asegurar que el panel esté oculto al inicio
        if (levelUpPanel != null)
        {
            levelUpPanel.SetActive(false);
        }
        isUIVisible = false;
        Time.timeScale = 1f; // Asegurar que el juego no esté pausado
        
        // Asegurar que el cursor del sistema esté siempre oculto
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.None;
        
        // Asegurar que el canvas de level up tenga un sorting order menor que el crosshair
        Canvas levelUpCanvas = GetComponentInParent<Canvas>();
        if (levelUpCanvas != null && levelUpCanvas.sortingOrder >= 32767)
        {
            levelUpCanvas.sortingOrder = 50; // Debajo del crosshair
        }
        
        // Asegurar que existe un EventSystem para los botones
        if (UnityEngine.EventSystems.EventSystem.current == null)
        {
            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }
    }

    public void Initialize(AbilityManager manager)
    {
        abilityManager = manager;
    }

    public void ShowLevelUpUI(List<AbilityOption> options)
    {
        // Evitar mostrar la UI si ya está visible
        if (isUIVisible)
        {
            Debug.LogWarning("LevelUpUI: UI is already visible, ignoring request to show again.");
            return;
        }

        if (levelUpPanel == null || abilityOptionsContainer == null)
        {
            Debug.LogError("LevelUpUI: Missing UI references!");
            return;
        }

        if (options == null || options.Count == 0)
        {
            Debug.LogError("LevelUpUI: No options provided!");
            return;
        }

        // Mantener el cursor del sistema siempre oculto
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.None;

        isUIVisible = true;
        currentOptions = options;
        levelUpPanel.SetActive(true);
        Time.timeScale = 0f; // Pausar el juego

        // Asegurar que existe un EventSystem para los botones
        if (UnityEngine.EventSystems.EventSystem.current == null)
        {
            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }

        // Limpiar opciones anteriores
        foreach (Transform child in abilityOptionsContainer)
        {
            Destroy(child.gameObject);
        }

        // Crear UI para cada opción
        for (int i = 0; i < options.Count; i++)
        {
            if (options[i] != null && options[i].ability != null)
            {
                CreateAbilityOptionUI(options[i], i);
            }
            else
            {
                Debug.LogError($"LevelUpUI: Invalid option at index {i} - ability is null!");
            }
        }
        
        // Seleccionar el primer botón automáticamente para navegación con teclado
        StartCoroutine(SelectFirstButton());
    }

    void CreateAbilityOptionUI(AbilityOption option, int index)
    {
        if (abilityOptionPrefab == null)
        {
            // Si no hay prefab, crear UI básica
            CreateBasicAbilityOption(option, index);
            return;
        }

        GameObject optionUI = Instantiate(abilityOptionPrefab, abilityOptionsContainer);
        SetupAbilityOptionUI(optionUI, option, index);
    }

    void CreateBasicAbilityOption(AbilityOption option, int index)
    {
        if (option.ability == null)
        {
            Debug.LogError($"LevelUpUI: Ability is null for option {index}!");
            return;
        }

        GameObject optionContainer = new GameObject($"AbilityOption_{index}");
        optionContainer.transform.SetParent(abilityOptionsContainer, false);

        RectTransform rectTransform = optionContainer.AddComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(380, 400);

        // Fondo - color base consistente
        Image background = optionContainer.AddComponent<Image>();
        Color baseColor = new Color(0.2f, 0.2f, 0.25f, 0.95f); // Color base para todas las opciones
        background.color = baseColor;
        
        // Agregar outline al fondo para crear un borde visible
        var bgOutline = optionContainer.AddComponent<UnityEngine.UI.Outline>();
        bgOutline.effectColor = new Color(0.3f, 0.5f, 0.8f, 1f); // Azul brillante para el borde
        bgOutline.effectDistance = new Vector2(4, 4);
        bgOutline.useGraphicAlpha = false;

        // Botón
        Button button = optionContainer.AddComponent<Button>();
        int optionIndex = index; // Capturar para el lambda
        
        // Configurar colores del botón - todas empiezan con el mismo color, solo se oscurecen en hover
        ColorBlock colors = button.colors;
        Color normalColor = new Color(0.2f, 0.2f, 0.25f, 0.95f); // Color base consistente para todas
        colors.normalColor = normalColor; // Color normal - todas iguales
        colors.highlightedColor = new Color(normalColor.r * 0.5f, normalColor.g * 0.5f, normalColor.b * 0.5f, normalColor.a); // Solo se oscurece en hover
        colors.pressedColor = new Color(normalColor.r * 0.4f, normalColor.g * 0.4f, normalColor.b * 0.4f, normalColor.a); // Más oscuro al presionar
        colors.selectedColor = normalColor; // Mismo color que normal (no oscurecido)
        colors.disabledColor = new Color(0.1f, 0.1f, 0.1f, 0.5f);
        colors.colorMultiplier = 1f;
        colors.fadeDuration = 0.15f; // Transición suave
        button.colors = colors;
        
        // Configurar transición del botón
        button.transition = Selectable.Transition.ColorTint;
        button.targetGraphic = background;
        
        // Agregar listener del botón
        button.onClick.AddListener(() => SelectAbility(optionIndex));
        
        // Asegurar que el botón sea interactuable
        button.interactable = true;

        // Contenedor vertical para el contenido
        GameObject contentContainer = new GameObject("Content");
        contentContainer.transform.SetParent(optionContainer.transform, false);
        RectTransform contentRect = contentContainer.AddComponent<RectTransform>();
        contentRect.anchorMin = Vector2.zero;
        contentRect.anchorMax = Vector2.one;
        contentRect.sizeDelta = Vector2.zero;
        contentRect.anchoredPosition = Vector2.zero;

        VerticalLayoutGroup layout = contentContainer.AddComponent<VerticalLayoutGroup>();
        layout.spacing = 12;
        layout.padding = new RectOffset(20, 20, 25, 25);
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.childControlWidth = true;
        layout.childControlHeight = false;
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = false;

        // Icono (siempre crear el contenedor, incluso si no hay icono)
        GameObject iconObj = new GameObject("Icon");
        iconObj.transform.SetParent(contentContainer.transform, false);
        RectTransform iconRect = iconObj.AddComponent<RectTransform>();
        iconRect.sizeDelta = new Vector2(100, 100);
        
        if (option.ability.abilityIcon != null)
        {
            Image icon = iconObj.AddComponent<Image>();
            icon.sprite = option.ability.abilityIcon;
            icon.preserveAspect = true;
        }
        else
        {
            // Si no hay icono, mostrar un placeholder más visible
            Image icon = iconObj.AddComponent<Image>();
            icon.color = new Color(0.4f, 0.4f, 0.5f, 0.6f);
        }

        // Nombre de la habilidad (siempre mostrar, usar nombre por defecto si está vacío)
        GameObject nameObj = new GameObject("AbilityName");
        nameObj.transform.SetParent(contentContainer.transform, false);
        TextMeshProUGUI nameText = nameObj.AddComponent<TextMeshProUGUI>();
        string abilityName = string.IsNullOrEmpty(option.ability.abilityName) 
            ? option.ability.name.Replace("Ability", "").Trim() 
            : option.ability.abilityName;
        nameText.text = abilityName;
        nameText.fontSize = 28;
        nameText.alignment = TextAlignmentOptions.Center;
        nameText.color = new Color(1f, 1f, 1f, 1f); // Blanco brillante
        nameText.fontStyle = FontStyles.Bold;
        nameText.textWrappingMode = TextWrappingModes.Normal;
        
        // Agregar outline para mejor legibilidad
        var outline = nameObj.AddComponent<UnityEngine.UI.Outline>();
        outline.effectColor = new Color(0f, 0f, 0f, 0.8f); // Negro semi-transparente
        outline.effectDistance = new Vector2(2, 2);

        // Descripción deshabilitada - no mostrar descripciones
        // if (!string.IsNullOrEmpty(option.ability.abilityDescription))
        // {
        //     GameObject descObj = new GameObject("Description");
        //     descObj.transform.SetParent(contentContainer.transform, false);
        //     TextMeshProUGUI descText = descObj.AddComponent<TextMeshProUGUI>();
        //     descText.text = option.ability.abilityDescription;
        //     descText.fontSize = 16;
        //     descText.alignment = TextAlignmentOptions.Center;
        //     descText.color = new Color(0.9f, 0.9f, 0.9f, 1f);
        //     descText.enableWordWrapping = true;
        //     RectTransform descRect = descObj.GetComponent<RectTransform>();
        //     descRect.sizeDelta = new Vector2(0, 70);
        //     var descOutline = descObj.AddComponent<UnityEngine.UI.Outline>();
        //     descOutline.effectColor = new Color(0f, 0f, 0f, 0.6f);
        //     descOutline.effectDistance = new Vector2(1, 1);
        // }

        // Keybind (siempre mostrar)
        GameObject keybindObj = new GameObject("Keybind");
        keybindObj.transform.SetParent(contentContainer.transform, false);
        TextMeshProUGUI keybindText = keybindObj.AddComponent<TextMeshProUGUI>();
        keybindText.text = $"Tecla: {FormatKeyCode(option.keybind)}";
        keybindText.fontSize = 22;
        keybindText.alignment = TextAlignmentOptions.Center;
        keybindText.color = new Color(1f, 0.85f, 0.2f); // Amarillo/dorado más brillante
        keybindText.fontStyle = FontStyles.Bold;
        
        // Agregar outline para mejor legibilidad
        var keybindOutline = keybindObj.AddComponent<UnityEngine.UI.Outline>();
        keybindOutline.effectColor = new Color(0f, 0f, 0f, 0.9f);
        keybindOutline.effectDistance = new Vector2(2, 2);

        // Cooldown (siempre mostrar, incluso si es 0)
        GameObject cooldownObj = new GameObject("Cooldown");
        cooldownObj.transform.SetParent(contentContainer.transform, false);
        TextMeshProUGUI cooldownText = cooldownObj.AddComponent<TextMeshProUGUI>();
        if (option.ability.abilityCooldown > 0)
        {
            cooldownText.text = $"Cooldown: {option.ability.abilityCooldown}s";
        }
        else
        {
            cooldownText.text = "Sin cooldown";
        }
        cooldownText.fontSize = 20;
        cooldownText.alignment = TextAlignmentOptions.Center;
        cooldownText.color = new Color(0.8f, 0.8f, 0.8f, 1f); // Gris más claro y visible
        cooldownText.fontStyle = FontStyles.Bold;
        
        // Agregar outline sutil
        var cooldownOutline = cooldownObj.AddComponent<UnityEngine.UI.Outline>();
        cooldownOutline.effectColor = new Color(0f, 0f, 0f, 0.7f);
        cooldownOutline.effectDistance = new Vector2(1, 1);
    }

    void SetupAbilityOptionUI(GameObject optionUI, AbilityOption option, int index)
    {
        // Buscar componentes en el prefab
        Button button = optionUI.GetComponent<Button>();
        if (button == null)
        {
            button = optionUI.GetComponentInChildren<Button>();
        }

        if (button != null)
        {
            int optionIndex = index;
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => SelectAbility(optionIndex));
        }

        // Buscar y actualizar textos
        TextMeshProUGUI[] texts = optionUI.GetComponentsInChildren<TextMeshProUGUI>();
        foreach (var text in texts)
        {
            if (text.name.Contains("Name") || text.name.Contains("name"))
            {
                text.text = option.ability.abilityName;
            }
            else if (text.name.Contains("Desc") || text.name.Contains("desc"))
            {
                text.text = option.ability.abilityDescription;
            }
            else if (text.name.Contains("Key") || text.name.Contains("key"))
            {
                text.text = $"Tecla: {FormatKeyCode(option.keybind)}";
            }
        }

        // Buscar y actualizar icono
        Image[] images = optionUI.GetComponentsInChildren<Image>();
        foreach (var img in images)
        {
            if (img.name.Contains("Icon") || img.name.Contains("icon"))
            {
                if (option.ability.abilityIcon != null)
                {
                    img.sprite = option.ability.abilityIcon;
                }
            }
        }
    }

    public void SelectAbility(int index)
    {
        if (index < 0 || index >= currentOptions.Count)
        {
            Debug.LogError("LevelUpUI: Invalid ability index!");
            return;
        }

        AbilityOption selected = currentOptions[index];
        
        if (abilityManager != null)
        {
            abilityManager.AddSelectedAbility(selected.ability, selected.keybind);
        }

        HideLevelUpUI();
    }

    public void HideLevelUpUI()
    {
        if (!isUIVisible)
        {
            return; // Ya está oculta
        }

        isUIVisible = false;
        
        // Deseleccionar cualquier botón seleccionado
        if (UnityEngine.EventSystems.EventSystem.current != null)
        {
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
        }
        
        if (levelUpPanel != null)
        {
            levelUpPanel.SetActive(false);
        }
        
        // Mantener el cursor del sistema oculto
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.None;
        
        Time.timeScale = 1f; // Reanudar el juego
        currentOptions.Clear();
    }

    void Update()
    {
        // Asegurar que el cursor del sistema esté siempre oculto
        if (Cursor.visible)
        {
            Cursor.visible = false;
        }
        
        // Permitir selección con teclado cuando la UI está visible
        if (isUIVisible && currentOptions.Count > 0)
        {
            // Seleccionar con números 1, 2, 3
            if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
            {
                if (currentOptions.Count >= 1)
                {
                    SelectAbility(0);
                }
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
            {
                if (currentOptions.Count >= 2)
                {
                    SelectAbility(1);
                }
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
            {
                if (currentOptions.Count >= 3)
                {
                    SelectAbility(2);
                }
            }
        }
    }

    IEnumerator SelectFirstButton()
    {
        // Esperar un frame para que los botones se creen completamente
        yield return null;
        
        // NO seleccionar automáticamente el primer botón para evitar que se oscurezca
        // Si quieres navegación con teclado, puedes habilitar esto pero asegúrate de que
        // el color selected sea igual al normal
        // if (abilityOptionsContainer != null && abilityOptionsContainer.childCount > 0)
        // {
        //     Button firstButton = abilityOptionsContainer.GetChild(0).GetComponent<Button>();
        //     if (firstButton != null && UnityEngine.EventSystems.EventSystem.current != null)
        //     {
        //         UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(firstButton.gameObject);
        //     }
        // }
    }

    string FormatKeyCode(KeyCode key)
    {
        // Formatear KeyCode para mostrar de manera más amigable
        string keyString = key.ToString();
        
        // Reemplazar algunos códigos comunes
        if (keyString.StartsWith("Alpha"))
        {
            return keyString.Replace("Alpha", "");
        }
        if (keyString.StartsWith("Left") || keyString.StartsWith("Right"))
        {
            return keyString;
        }
        
        return keyString;
    }
}
