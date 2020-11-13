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
        buttonText.color = PlayerConstants.activeColor;
    }

    public void UnselectTab()
    {
        isSelected = false;
        image.sprite = inactiveSwoosh;
        buttonText.color = PlayerConstants.inactiveColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isSelected)
        {
            buttonText.color = PlayerConstants.hoverColor;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isSelected)
        {
            buttonText.color = PlayerConstants.inactiveColor;
        }
    }
}
