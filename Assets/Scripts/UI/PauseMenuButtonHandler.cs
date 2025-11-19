using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PauseMenuButtonHandler : MonoBehaviour, IPointerClickHandler
{
    private Button button;

    void Awake()
    {
        button = GetComponent<Button>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (button != null && button.interactable) button.onClick.Invoke();
    }
}

