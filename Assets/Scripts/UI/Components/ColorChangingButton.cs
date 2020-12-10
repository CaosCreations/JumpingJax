using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ColorChangingButton : MonoBehaviour,  IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Button button;
    public Text text;

    public Sprite buttonDefaultSprite;
    public Sprite buttonHoverSprite;
    public Sprite buttonActiveSprite;
    public Sprite buttonDisabledSprite;

    public Color defaultTextColor;
    public Color activeTextColor;

    private bool isActive;

    private void Awake()
    {
        button = GetComponent<Button>();
        text = GetComponentInChildren<Text>();
    }

    public void Init(Action func)
    {
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => func());
    }

    public void Init()
    {
    }

    public void SetActive()
    {
        button.image.sprite = buttonActiveSprite;
        text.color = activeTextColor;
    }

    public void ClearActive()
    {
        button.image.sprite = buttonDefaultSprite;
        text.color = defaultTextColor;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        button.image.sprite = buttonActiveSprite;
        text.color = activeTextColor;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        button.image.sprite = buttonDefaultSprite;
        text.color = defaultTextColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        button.image.sprite = buttonHoverSprite;
        text.color = defaultTextColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        button.image.sprite = buttonDefaultSprite;
        text.color = defaultTextColor;
    }
}
