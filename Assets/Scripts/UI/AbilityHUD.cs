using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// Gestor del HUD de habilidades.
/// Crea y actualiza los slots de habilidades dinámicamente cuando el jugador adquiere o pierde habilidades.
/// </summary>
public class AbilityHUD : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerAbilityController abilityController;
    [SerializeField] private GameObject abilityHUDSlotPrefab;
    [SerializeField] private Transform slotsContainer;
    
    [Header("Prefab References")]
    [SerializeField] private Canvas abilityHUDCanvas;
    
    private List<AbilityHUDSlot> activeSlots = new List<AbilityHUDSlot>();
    
    private void Start()
    {
        // Buscar el controlador de habilidades si no está asignado
        if (abilityController == null)
        {
            abilityController = FindFirstObjectByType<PlayerAbilityController>();
        }
        
        if (abilityController == null)
        {
            Debug.LogError("AbilityHUD: No PlayerAbilityController found! HUD cannot function.");
            enabled = false;
            return;
        }
        
        // Conectar los eventos del controlador de habilidades
        abilityController.OnAbilityAdded += OnAbilityAdded;
        abilityController.OnAbilityRemoved += OnAbilityRemoved;
        
        // Buscar o crear el contenedor de slots
        if (slotsContainer == null)
        {
            // Si estamos en un Canvas, usar el Transform de este objeto
            slotsContainer = transform;
        }
        
        // Asegurar que el contenedor tenga un VerticalLayoutGroup
        VerticalLayoutGroup layoutGroup = slotsContainer.GetComponent<VerticalLayoutGroup>();
        if (layoutGroup == null)
        {
            layoutGroup = slotsContainer.gameObject.AddComponent<VerticalLayoutGroup>();
            layoutGroup.childForceExpandWidth = true;
            layoutGroup.childForceExpandHeight = false;
            layoutGroup.spacing = 10;
            layoutGroup.padding = new RectOffset(10, 10, 10, 10);
            
            Debug.Log("AbilityHUD: Added VerticalLayoutGroup to container");
        }
        
        // Asegurar que tenga ContentSizeFitter para ajusarse automáticamente
        ContentSizeFitter fitter = slotsContainer.GetComponent<ContentSizeFitter>();
        if (fitter == null)
        {
            fitter = slotsContainer.gameObject.AddComponent<ContentSizeFitter>();
            fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            
            Debug.Log("AbilityHUD: Added ContentSizeFitter to container");
        }
        
        // Crear el prefab dinámicamente si no existe
        if (abilityHUDSlotPrefab == null)
        {
            abilityHUDSlotPrefab = CreateAbilitySlotPrefabDynamic();
        }
        
        if (abilityHUDSlotPrefab == null)
        {
            Debug.LogError("AbilityHUD: Could not create abilityHUDSlotPrefab!");
            enabled = false;
            return;
        }
        
        Debug.Log("AbilityHUD: Fully initialized and ready!");
        
        // Inicializar slots existentes (en caso de que el juego cargue con habilidades ya adquiridas)
        RefreshHUD();
    }
    
    private void Update()
    {
        // Actualizar la visualización de todos los slots (especialmente cooldown)
        foreach (var slot in activeSlots)
        {
            if (slot != null)
                slot.UpdateDisplay();
        }
    }
    
    /// <summary>
    /// Crea dinámicamente un prefab de slot de habilidad
    /// </summary>
    private GameObject CreateAbilitySlotPrefabDynamic()
    {
        // Crear GameObject raíz del slot
        GameObject slotGO = new GameObject("AbilitySlot_Prefab");
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
        abilityImage.color = new Color(0.85f, 0.85f, 0.9f, 1f); // Gris muy claro y opaco
        
        // Agregar outline al fondo para mejor visibilidad
        Outline outline = bgGO.AddComponent<Outline>();
        outline.effectColor = new Color(0, 0, 0, 0.3f);
        outline.effectDistance = new Vector2(1, -1);
        
        LayoutElement bgLayoutElement = bgGO.AddComponent<LayoutElement>();
        bgLayoutElement.preferredWidth = 100;
        bgLayoutElement.preferredHeight = 100;
        
        // Crear Cooldown Fill Image
        GameObject cooldownGO = new GameObject("CooldownFill");
        cooldownGO.transform.SetParent(bgGO.transform, false);
        
        RectTransform cooldownRect = cooldownGO.AddComponent<RectTransform>();
        cooldownRect.anchorMin = Vector2.zero;
        cooldownRect.anchorMax = Vector2.one;
        cooldownRect.offsetMin = Vector2.zero;
        cooldownRect.offsetMax = Vector2.zero;
        
        Image cooldownFill = cooldownGO.AddComponent<Image>();
        cooldownFill.color = new Color(0, 0, 0, 0.3f); // Más transparente
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
        
        // Necesario para TMPro
        TMPro.TextMeshProUGUI keyText = keyTextGO.AddComponent<TMPro.TextMeshProUGUI>();
        keyText.text = "Q";
        keyText.alignment = TMPro.TextAlignmentOptions.BottomRight;
        keyText.fontSize = 36;
        keyText.color = Color.white;
        
        // Crear Indicador de Habilidad Robada (Cruz Roja X)
        GameObject stolenIndicatorGO = new GameObject("StolenIndicator");
        stolenIndicatorGO.transform.SetParent(slotGO.transform, false);
        
        RectTransform stolenRect = stolenIndicatorGO.AddComponent<RectTransform>();
        stolenRect.anchorMin = Vector2.zero;
        stolenRect.anchorMax = Vector2.one;
        stolenRect.offsetMin = Vector2.zero;
        stolenRect.offsetMax = Vector2.zero;
        
        // Agregar Image de fondo semitransparente para hacer la cruz más visible
        Image stolenBg = stolenIndicatorGO.AddComponent<Image>();
        stolenBg.color = new Color(0, 0, 0, 0.2f); // Fondo muy transparente
        
        TMPro.TextMeshProUGUI stolenText = stolenIndicatorGO.AddComponent<TMPro.TextMeshProUGUI>();
        stolenText.text = "✕"; // Cruz o X
        stolenText.alignment = TMPro.TextAlignmentOptions.Center;
        stolenText.fontSize = 70;
        stolenText.color = Color.red;
        stolenText.enabled = false; // Oculto por defecto
        
        // Agregar CanvasGroup al indicador para controlar su visibilidad completamente
        CanvasGroup stolenCanvasGroup = stolenIndicatorGO.AddComponent<CanvasGroup>();
        stolenCanvasGroup.alpha = 0f; // Comenzar invisible
        
        // Usar reflection para asignar los componentes privados
        var iconField = typeof(AbilityHUDSlot).GetField("abilityIconImage", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        iconField?.SetValue(hudSlot, abilityImage);
        
        var keyTextField = typeof(AbilityHUDSlot).GetField("keyBindText", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        keyTextField?.SetValue(hudSlot, keyText);
        
        var cooldownField = typeof(AbilityHUDSlot).GetField("cooldownFillImage", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        cooldownField?.SetValue(hudSlot, cooldownFill);
        
        var stolenField = typeof(AbilityHUDSlot).GetField("stolenIndicator", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        stolenField?.SetValue(hudSlot, stolenText);
        
        var stolenGOField = typeof(AbilityHUDSlot).GetField("stolenIndicatorGO", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        stolenGOField?.SetValue(hudSlot, stolenIndicatorGO);
        
        Debug.Log("AbilityHUD: Ability slot prefab created dynamically!");
        return slotGO;
    }
    
    /// <summary>
    /// Reconstruye el HUD basado en los slots actuales del controlador
    /// </summary>
    public void RefreshHUD()
    {
        // Limpiar todos los slots existentes
        foreach (var slot in activeSlots)
        {
            if (slot != null)
                Destroy(slot.gameObject);
        }
        activeSlots.Clear();
        
        // Crear un nuevo slot para cada habilidad del jugador
        if (abilityController != null)
        {
            foreach (var abilitySlot in abilityController.slots)
            {
                CreateSlotUI(abilitySlot);
            }
        }
    }
    
    /// <summary>
    /// Crea un nuevo slot de UI para una habilidad
    /// </summary>
    private void CreateSlotUI(PlayerAbilityController.AbilitySlot abilitySlot)
    {
        if (abilityHUDSlotPrefab == null)
        {
            Debug.LogError("AbilityHUD: Cannot create slot, prefab is null!");
            return;
        }
        
        // Instanciar el prefab
        GameObject slotGO = Instantiate(abilityHUDSlotPrefab, slotsContainer);
        slotGO.name = $"AbilitySlot_{abilitySlot.ability.abilityName}";
        slotGO.SetActive(true);
        
        // Obtener el componente AbilityHUDSlot
        AbilityHUDSlot hudSlot = slotGO.GetComponent<AbilityHUDSlot>();
        
        if (hudSlot == null)
        {
            Debug.LogError("AbilityHUD: Instantiated slot doesn't have an AbilityHUDSlot component!");
            Destroy(slotGO);
            return;
        }
        
        // Configurar el slot
        hudSlot.SetAbility(abilitySlot);
        activeSlots.Add(hudSlot);
        
        Debug.Log($"AbilityHUD: Created UI slot for ability: {abilitySlot.ability.abilityName}");
    }
    
    /// <summary>
    /// Se llama cuando se añade una nueva habilidad
    /// Nota: Esta función se llama desde PlayerAbilityController mediante un evento
    /// </summary>
    public void OnAbilityAdded(PlayerAbilityController.AbilitySlot newSlot)
    {
        Debug.Log($"AbilityHUD: OnAbilityAdded called for {newSlot.ability.abilityName}");
        
        // Crear un nuevo slot UI
        CreateSlotUI(newSlot);
    }
    
    /// <summary>
    /// Se llama cuando se remueve una habilidad
    /// Nota: Esta función se llama desde PlayerAbilityController mediante un evento
    /// </summary>
    public void OnAbilityRemoved(Ability removedAbility)
    {
        Debug.Log($"AbilityHUD: OnAbilityRemoved called for {removedAbility.abilityName}");
        
        // Encontrar y eliminar el slot correspondiente
        for (int i = activeSlots.Count - 1; i >= 0; i--)
        {
            if (activeSlots[i] == null)
            {
                activeSlots.RemoveAt(i);
                continue;
            }
            
            // Buscar el slot con la habilidad removida
            if (activeSlots[i].GetAbility() == removedAbility)
            {
                Destroy(activeSlots[i].gameObject);
                activeSlots.RemoveAt(i);
                return;
            }
        }
    }
}
