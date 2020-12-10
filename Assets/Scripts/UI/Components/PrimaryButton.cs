using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PrimaryButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Button button;
    public Text text;

    public Sprite buttonDefaultSprite;
    public Sprite buttonHoverSprite;
    public Sprite buttonActiveSprite;
    public Sprite buttonDisabledSprite;

    public Color defaultTextColor;
    public Color disabledTextColor;
    public Color activeTextColor;

    private bool isActive;

    private void Awake()
    {
        button = GetComponent<Button>();
        text = GetComponentInChildren<Text>();
        isActive = false;
    }

    public void Init(Action func)
    {
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => func());
    }

    public void Init(string text, Action func)
    {
        this.text.text = text;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => func());
    }

    public void SetActive()
    {
        button.image.sprite = buttonActiveSprite;
        text.color = activeTextColor;
        isActive = true;
    }

    public void ClearActive()
    {
        button.image.sprite = buttonDefaultSprite;
        text.color = defaultTextColor;
        isActive = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!isActive)
        {
            button.image.sprite = buttonActiveSprite;
            text.color = activeTextColor;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!isActive)
        {
            button.image.sprite = buttonDefaultSprite;
            text.color = defaultTextColor;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isActive)
        {
            button.image.sprite = buttonHoverSprite;
            text.color = defaultTextColor;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isActive)
        {
            button.image.sprite = buttonDefaultSprite;
            text.color = defaultTextColor;
        }
    }
}
