using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MainMenuButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler
{
    public Text menuButtonText;

    public void OnPointerEnter(PointerEventData eventData)
    {
        menuButtonText.color = PlayerConstants.activeColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        menuButtonText.color = PlayerConstants.hoverColor;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        menuButtonText.color = PlayerConstants.hoverColor;
    }
}
