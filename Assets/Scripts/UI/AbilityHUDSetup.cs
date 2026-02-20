using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Helper script para configurar automáticamente el HUD de habilidades en la escena.
/// Crea toda la estructura Canvas + HUD + Prefab slots.
/// 
/// USAGE:
/// 1. Crea un GameObject vacío en la escena llamado "AbilityHUD"
/// 2. Agrega este script al GameObject
/// 3. En el inspector, asigna la referencia a PlayerAbilityController
/// 4. Dale click al botón "Setup Ability HUD" en el inspector
/// 5. Elimina este script (ya ha hecho su trabajo)
/// </summary>
#if UNITY_EDITOR
public class AbilityHUDSetup : MonoBehaviour
{
    [SerializeField] private PlayerAbilityController playerAbilityController;
    
    /// <summary>
    /// Configura toda la estructura del HUD automáticamente
    /// </summary>
    [ContextMenu("Setup Ability HUD")]
    public void SetupAbilityHUD()
    {
        if (playerAbilityController == null)
        {
            Debug.LogError("AbilityHUDSetup: PlayerAbilityController not assigned!");
            return;
        }
        
        GameObject hudRoot = gameObject;
        
        // 1. Crear Canvas si no existe
        Canvas canvas = hudRoot.GetComponent<Canvas>();
        if (canvas == null)
        {
            canvas = hudRoot.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        }
        
        CanvasScaler scaler = hudRoot.GetComponent<CanvasScaler>();
        if (scaler == null)
        {
            scaler = hudRoot.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
        }
        
        GraphicRaycaster raycaster = hudRoot.GetComponent<GraphicRaycaster>();
        if (raycaster == null)
        {
            hudRoot.AddComponent<GraphicRaycaster>();
        }
        
        // 2. Configurar el RectTransform del Canvas
        RectTransform hudRectTransform = hudRoot.GetComponent<RectTransform>();
        hudRectTransform.anchorMin = Vector2.zero;
        hudRectTransform.anchorMax = Vector2.one;
        hudRectTransform.offsetMin = Vector2.zero;
        hudRectTransform.offsetMax = Vector2.zero;
        
        Debug.Log("AbilityHUDSetup: Canvas configured successfully!");
        
        // 3. Crear contenedor vertical (SlotsContainer)
        GameObject slotsContainerGO = new GameObject("SlotsContainer");
        slotsContainerGO.transform.SetParent(hudRoot.transform, false);
        
        RectTransform containerRect = slotsContainerGO.AddComponent<RectTransform>();
        // Posicionar en arriba-derecha de la pantalla
        containerRect.anchorMin = new Vector2(1, 1);
        containerRect.anchorMax = new Vector2(1, 1);
        containerRect.pivot = new Vector2(1, 1);
        containerRect.anchoredPosition = new Vector2(-20, -20);
        containerRect.sizeDelta = new Vector2(120, 400);
        
        VerticalLayoutGroup layoutGroup = slotsContainerGO.AddComponent<VerticalLayoutGroup>();
        layoutGroup.childForceExpandWidth = true;
        layoutGroup.childForceExpandHeight = false;
        layoutGroup.spacing = 10;
        layoutGroup.padding = new RectOffset(10, 10, 10, 10);
        
        ContentSizeFitter fitter = slotsContainerGO.AddComponent<ContentSizeFitter>();
        fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        
        Debug.Log("AbilityHUDSetup: SlotsContainer created!");
        
        // 4. Crear el prefab de slot
        GameObject slotPrefabGO = CreateAbilitySlotPrefab();
        
        // 5. Crear el componente AbilityHUD
        AbilityHUD abilityHUD = hudRoot.AddComponent<AbilityHUD>();
        
        // Usar reflection para asignar los valores del script
        var controllerField = typeof(AbilityHUD).GetField("abilityController", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        controllerField?.SetValue(abilityHUD, playerAbilityController);
        
        var slotPrefabField = typeof(AbilityHUD).GetField("abilityHUDSlotPrefab", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        slotPrefabField?.SetValue(abilityHUD, slotPrefabGO);
        
        var containerField = typeof(AbilityHUD).GetField("slotsContainer", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        containerField?.SetValue(abilityHUD, slotsContainerGO.transform);
        
        var canvasField = typeof(AbilityHUD).GetField("abilityHUDCanvas", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        canvasField?.SetValue(abilityHUD, canvas);
        
        Debug.Log("AbilityHUDSetup: AbilityHUD component configured!");
        
        // 6. Conectar eventos
        playerAbilityController.OnAbilityAdded += abilityHUD.OnAbilityAdded;
        playerAbilityController.OnAbilityRemoved += abilityHUD.OnAbilityRemoved;
        
        Debug.Log("AbilityHUDSetup: Events connected! Setup complete.");
        Debug.Log("You can now delete this AbilityHUDSetup script as it has completed its purpose.");
    }
    
    /// <summary>
    /// Crea el prefab de slot de habilidad
    /// </summary>
    private GameObject CreateAbilitySlotPrefab()
    {
        // Crear GameObject raíz del slot
        GameObject slotGO = new GameObject("AbilitySlot");
        slotGO.SetActive(false); // Desactivar para que se use como prefab
        
        RectTransform slotRect = slotGO.AddComponent<RectTransform>();
        slotRect.sizeDelta = new Vector2(100, 100);
        
        // Agregar componente de slot
        AbilityHUDSlot hudSlot = slotGO.AddComponent<AbilityHUDSlot>();
        
        // Crear Background (Image de la habilidad)
        GameObject bgGO = new GameObject("Background");
        bgGO.transform.SetParent(slotGO.transform, false);
        
        RectTransform bgRect = bgGO.AddComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;
        
        Image abilityImage = bgGO.AddComponent<Image>();
        abilityImage.color = new Color(0.2f, 0.2f, 0.2f, 1f); // Gris oscuro por defecto
        
        // Crear Cooldown Fill Image
        GameObject cooldownGO = new GameObject("CooldownFill");
        cooldownGO.transform.SetParent(bgGO.transform, false);
        
        RectTransform cooldownRect = cooldownGO.AddComponent<RectTransform>();
        cooldownRect.anchorMin = Vector2.zero;
        cooldownRect.anchorMax = Vector2.one;
        cooldownRect.offsetMin = Vector2.zero;
        cooldownRect.offsetMax = Vector2.zero;
        
        Image cooldownFill = cooldownGO.AddComponent<Image>();
        cooldownFill.color = new Color(0, 0, 0, 0.5f); // Semi-transparente
        cooldownFill.type = Image.Type.Filled;
        cooldownFill.fillMethod = Image.FillMethod.Vertical;
        cooldownFill.fillOrigin = (int)Image.OriginVertical.Bottom;
        cooldownFill.fillAmount = 0;
        
        // Crear KeyBind Text
        GameObject keyTextGO = new GameObject("KeyBindText");
        keyTextGO.transform.SetParent(slotGO.transform, false);
        
        RectTransform keyTextRect = keyTextGO.AddComponent<RectTransform>();
        keyTextRect.anchorMin = Vector2.one; // Esquina inferior-derecha
        keyTextRect.anchorMax = Vector2.one;
        keyTextRect.pivot = Vector2.one;
        keyTextRect.anchoredPosition = new Vector2(-5, 5);
        keyTextRect.sizeDelta = new Vector2(60, 30);
        
        TextMeshProUGUI keyText = keyTextGO.AddComponent<TextMeshProUGUI>();
        keyText.text = "Q";
        keyText.alignment = TextAlignmentOptions.BottomRight;
        keyText.fontSize = 32;
        keyText.color = Color.white;
        
        // Usar reflection para asignar los componentes de imagen
        var iconField = typeof(AbilityHUDSlot).GetField("abilityIconImage", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        iconField?.SetValue(hudSlot, abilityImage);
        
        var keyTextField = typeof(AbilityHUDSlot).GetField("keyBindText", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        keyTextField?.SetValue(hudSlot, keyText);
        
        var cooldownField = typeof(AbilityHUDSlot).GetField("cooldownFillImage", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        cooldownField?.SetValue(hudSlot, cooldownFill);
        
        Debug.Log("AbilityHUDSetup: Ability slot prefab created!");
        return slotGO;
    }
}
#endif
