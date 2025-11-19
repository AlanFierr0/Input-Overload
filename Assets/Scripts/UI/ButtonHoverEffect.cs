using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Material normalMaterial;
    public Material glowMaterial;
    
    private Image buttonImage;

    void Start()
    {
        buttonImage = GetComponent<Image>();
        buttonImage.raycastTarget = true;
        
        Button button = GetComponent<Button>();
        if (button != null) button.transition = Selectable.Transition.None;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        buttonImage.material = glowMaterial;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        buttonImage.material = normalMaterial;
    }
}

