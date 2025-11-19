using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Componente auxiliar que maneja los eventos de los botones manualmente cuando el tiempo está pausado
/// </summary>
public class PauseMenuButtonHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private Button button;
    private Image buttonImage;
    private Color normalColor;
    private Color highlightedColor;

    void Awake()
    {
        button = GetComponent<Button>();
        buttonImage = GetComponent<Image>();
        
        if (button != null && buttonImage != null)
        {
            normalColor = button.colors.normalColor;
            highlightedColor = button.colors.highlightedColor;
        }
    }

    // El Update no se ejecuta cuando Time.timeScale = 0, así que el hover se maneja en PauseMenu

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (button != null && button.interactable && buttonImage != null)
        {
            buttonImage.color = highlightedColor;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (button != null && button.interactable && buttonImage != null)
        {
            buttonImage.color = normalColor;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (button != null && button.interactable)
        {
            button.onClick.Invoke();
        }
    }
}

