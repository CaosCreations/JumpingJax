using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TabButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Button button;
    public Text buttonText;
    public Image image;

    public Sprite activeSwoosh;
    public Sprite inactiveSwoosh;

    private Color activeColor = new Color(.58f, .93f, .76f);
    private Color hoverColor = new Color(1, 1, 1);
    private Color inactiveColor = new Color(0.5f, 0.5f, 0.5f);

    private bool isSelected;

    public void Init(string buttonText, UnityAction action)
    {
        isSelected = false;
        this.buttonText.text = buttonText;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(action);
    }

    public void SelectTab()
    {
        isSelected = true;
        image.sprite = activeSwoosh;
        buttonText.color = activeColor;
    }

    public void UnselectTab()
    {
        isSelected = false;
        image.sprite = inactiveSwoosh;
        buttonText.color = inactiveColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isSelected)
        {
            buttonText.color = hoverColor;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isSelected)
        {
            buttonText.color = inactiveColor;
        }
    }
}
