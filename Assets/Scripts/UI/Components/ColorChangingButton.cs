using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ColorChangingButton : MonoBehaviour,  IPointerDownHandler, IPointerUpHandler
{
    public Button button;
    public Text text;

    public Sprite buttonDefaultSprite;
    public Sprite buttonHoverSprite;
    public Sprite buttonActiveSprite;
    public Sprite buttonDisabledSprite;

    public Color defaultTextColor;
    public Color selectedTextColor;

    private bool isActive;

    private void Awake()
    {
        button = GetComponent<Button>();
        text = GetComponentInChildren<Text>();
    }

    public void Init(Action func)
    {
        SpriteState spriteState = new SpriteState();
        spriteState.highlightedSprite = buttonHoverSprite;
        spriteState.pressedSprite = buttonActiveSprite;
        spriteState.disabledSprite = buttonDisabledSprite;

        button.transition = Selectable.Transition.SpriteSwap;
        button.spriteState = spriteState;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => func());
    }

    public void Init()
    {
        SpriteState spriteState = new SpriteState();
        spriteState.highlightedSprite = buttonHoverSprite;
        spriteState.pressedSprite = buttonActiveSprite;
        spriteState.disabledSprite = buttonDisabledSprite;

        button.transition = Selectable.Transition.SpriteSwap;
        button.spriteState = spriteState;
    }

    public void SetActive()
    {
        button.image.sprite = buttonActiveSprite;
        text.color = selectedTextColor;
    }

    public void ClearActive()
    {
        button.image.sprite = buttonDefaultSprite;
        text.color = defaultTextColor;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        text.color = selectedTextColor;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        text.color = defaultTextColor;
    }
}
