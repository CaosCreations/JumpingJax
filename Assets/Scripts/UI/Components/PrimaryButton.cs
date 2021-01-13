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

    private bool shouldIgnorePointer;

    private void Awake()
    {
        button = GetComponent<Button>();
        text = GetComponentInChildren<Text>();
        shouldIgnorePointer = false;
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

    #region tab
    // Use for acting as a tab
    public void SetActive()
    {
        button.image.sprite = buttonActiveSprite;
        text.color = activeTextColor;
        shouldIgnorePointer = true;
    }

    public void ClearActive()
    {
        button.image.sprite = buttonDefaultSprite;
        text.color = defaultTextColor;
        shouldIgnorePointer = false;
    }
    #endregion 

    #region disable
    // Use for acting as a disable-able button
    public void SetDisabled()
    {
        button.image.sprite = buttonDisabledSprite;
        button.interactable = false;
        text.color = disabledTextColor;
        shouldIgnorePointer = true;
    }
    
    public void ClearDisabled()
    {
        button.image.sprite = buttonDefaultSprite;
        button.interactable = true;
        text.color = activeTextColor;
        shouldIgnorePointer = false;
    }
    #endregion

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!shouldIgnorePointer)
        {
            button.image.sprite = buttonActiveSprite;
            text.color = activeTextColor;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!shouldIgnorePointer)
        {
            button.image.sprite = buttonDefaultSprite;
            text.color = defaultTextColor;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!shouldIgnorePointer)
        {
            button.image.sprite = buttonHoverSprite;
            text.color = defaultTextColor;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!shouldIgnorePointer)
        {
            button.image.sprite = buttonDefaultSprite;
            text.color = defaultTextColor;
        }
    }
}
