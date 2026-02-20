using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Componente que representa un slot de habilidad en el HUD.
/// Muestra la imagen de la habilidad, la tecla asignada y la barra de cooldown.
/// </summary>
public class AbilityHUDSlot : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image abilityIconImage;
    [SerializeField] private TextMeshProUGUI keyBindText;
    [SerializeField] private Image cooldownFillImage;
    [SerializeField] private TextMeshProUGUI stolenIndicator; // Indicador de habilidad robada
    [SerializeField] private GameObject stolenIndicatorGO; // GameObject para usar CanvasGroup
    
    private PlayerAbilityController.AbilitySlot abilitySlot;
    private CanvasGroup canvasGroup;
    
    private void Awake()
    {
        // Obtener referencias auto si no están asignadas
        if (abilityIconImage == null)
            abilityIconImage = GetComponentInChildren<Image>();
        
        // Buscar todos los TextMeshProUGUI en los hijos
        TextMeshProUGUI[] textComponents = GetComponentsInChildren<TextMeshProUGUI>();
        
        if (keyBindText == null && textComponents.Length > 0)
            keyBindText = textComponents[0];
        
        if (stolenIndicator == null && textComponents.Length > 1)
            stolenIndicator = textComponents[1];
        
        // Buscar el GameObject del indicador robado
        if (stolenIndicatorGO == null && stolenIndicator != null)
            stolenIndicatorGO = stolenIndicator.gameObject;
        
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }
    
    /// <summary>
    /// Asigna una habilidad a este slot y configura su visualización
    /// </summary>
    public void SetAbility(PlayerAbilityController.AbilitySlot slot)
    {
        abilitySlot = slot;
        
        if (slot.ability == null)
        {
            Debug.LogWarning("AbilityHUDSlot: Trying to set a null ability!");
            return;
        }
        
        // Mostrar el icono de la habilidad
        if (abilityIconImage != null && slot.ability.abilityIcon != null)
        {
            abilityIconImage.sprite = slot.ability.abilityIcon;
        }
        
        // Mostrar la tecla asignada
        if (keyBindText != null)
        {
            keyBindText.text = FormatKeyCode(slot.key);
        }
        
        // Inicilizar la barra de cooldown
        if (cooldownFillImage != null)
        {
            cooldownFillImage.fillAmount = 0f;
        }
    }
    
    /// <summary>
    /// Actualiza la visualización del slot (cooldown, estado, etc.)
    /// Debe llamarse cada frame desde AbilityHUD
    /// </summary>
    public void UpdateDisplay()
    {
        if (abilitySlot == null)
            return;
        
        // Actualizar indicador de habilidad robada (incluso si ability == null)
        if (stolenIndicatorGO != null)
        {
            CanvasGroup stolenCanvasGroup = stolenIndicatorGO.GetComponent<CanvasGroup>();
            if (stolenCanvasGroup != null)
            {
                stolenCanvasGroup.alpha = abilitySlot.isStolen ? 1f : 0f;
            }
        }
        
        // Si no hay habilidad y no está robada, retornar
        if (abilitySlot.ability == null && !abilitySlot.isStolen)
            return;
        
        UpdateCooldownDisplay();
    }
    
    private void UpdateCooldownDisplay()
    {
        if (cooldownFillImage == null)
            return;
        
        // Si la habilidad está robada, mostrar fill rojo
        if (abilitySlot.isStolen)
        {
            cooldownFillImage.fillAmount = 1f;
            cooldownFillImage.color = new Color(1f, 0.2f, 0.2f, 0.6f); // Rojo semitransparente
            
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1f;
            }
            return;
        }
        
        // Si no hay habilidad, no actualizar cooldown
        if (abilitySlot.ability == null)
            return;
        
        // Solo mostrar cooldown si está en estado Cooldown
        if (abilitySlot.state == AbilityState.Cooldown)
        {
            float timeRemaining = abilitySlot.nextReadyTime - Time.time;
            float totalCooldown = abilitySlot.ability.abilityCooldown;
            
            // Calcular el fill amount (0 = cooldown completo, 1 = listo)
            float fillAmount = 1f - (timeRemaining / totalCooldown);
            fillAmount = Mathf.Clamp01(fillAmount);
            
            cooldownFillImage.fillAmount = fillAmount;
            cooldownFillImage.color = new Color(0f, 0f, 0f, 0.3f); // Negro muy transparente
            
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1f;
            }
        }
        else
        {
            // Habilidad lista - sin fill
            cooldownFillImage.fillAmount = 0f;
            cooldownFillImage.color = new Color(0f, 0f, 0f, 0.3f);
            
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1f;
            }
        }
    }
    
    /// <summary>
    /// Obtiene la habilidad de este slot
    /// </summary>
    public Ability GetAbility()
    {
        return abilitySlot?.ability;
    }
    
    /// <summary>
    /// Convierte un KeyCode a un string legible
    /// </summary>
    private string FormatKeyCode(KeyCode key)
    {
        // Casos especiales para teclas que necesitan nombres personalizados
        return key switch
        {
            KeyCode.LeftShift => "LSHIFT",
            KeyCode.RightShift => "RSHIFT",
            KeyCode.LeftAlt => "LALT",
            KeyCode.RightAlt => "RALT",
            KeyCode.LeftControl => "LCTRL",
            KeyCode.RightControl => "RCTRL",
            KeyCode.Alpha0 => "0",
            KeyCode.Alpha1 => "1",
            KeyCode.Alpha2 => "2",
            KeyCode.Alpha3 => "3",
            KeyCode.Alpha4 => "4",
            KeyCode.Alpha5 => "5",
            KeyCode.Alpha6 => "6",
            KeyCode.Alpha7 => "7",
            KeyCode.Alpha8 => "8",
            KeyCode.Alpha9 => "9",
            // Para las demás teclas, usar el nombre del KeyCode
            _ => key.ToString()
        };
    }
}
