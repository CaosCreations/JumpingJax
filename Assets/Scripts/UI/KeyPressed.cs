﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeyPressed : MonoBehaviour
{
    public Image forwardPressed;
    public Image leftPressed;
    public Image rightPressed;
    public Image backPressed;
    public Image crouchPressed;
    public Image jumpPressed;
    public Image mousePressed;

    public Sprite directionDefaultSprite;
    public Sprite directionPressedSprite;

    public Sprite crouchDefaultSprite;
    public Sprite crouchPressedSprite;

    public Sprite jumpDefaultSprite;
    public Sprite jumpPressedSprite;

    public Sprite defaultMousePressedSprite;
    public Sprite leftMousePressedSprite;
    public Sprite rightMousePressedSprite;
    public Sprite bothMousePressedSprite;

    private Level currentLevel;

    private bool isForwardPressed;
    private bool isLeftPressed;
    private bool isRightPressed;
    private bool isBackPressed;
    private bool isJumpPressed;
    private bool isCrouchPressed;
    private bool isMouseLeftPressed;
    private bool isMouseRightPressed;

    private void Start()
    {
        currentLevel = GameManager.GetCurrentLevel();
    }

    void Update()
    {
        if (Time.timeScale == 0)
        {
            return;
        }

        GetPressed();
        CheckKeys();
        CheckMouse();
    }

    public void SetPressed(KeysPressed keysPressed)
    {
        isForwardPressed = keysPressed.isForwardPressed;
        isLeftPressed = keysPressed.isLeftPressed;
        isRightPressed = keysPressed.isRightPressed;
        isBackPressed = keysPressed.isBackPressed;
        isJumpPressed = keysPressed.isJumpPressed;
        isCrouchPressed = keysPressed.isCrouchPressed;
        isMouseLeftPressed = keysPressed.isMouseLeftPressed;
        isMouseRightPressed = keysPressed.isMouseRightPressed;
    }

    private void GetPressed()
    {
        // if the level isn't completed, just show the currently pressed buttons
        if (!currentLevel.isCompleted)
        {
            isForwardPressed = InputManager.GetKey(PlayerConstants.Forward);
            isLeftPressed = InputManager.GetKey(PlayerConstants.Left);
            isRightPressed = InputManager.GetKey(PlayerConstants.Right);
            isBackPressed = InputManager.GetKey(PlayerConstants.Back);
            isJumpPressed = InputManager.GetKey(PlayerConstants.Jump);
            isCrouchPressed = InputManager.GetKey(PlayerConstants.Crouch);
            isMouseLeftPressed = InputManager.GetKey(PlayerConstants.Portal1);
            isMouseRightPressed = InputManager.GetKey(PlayerConstants.Portal2);
        }
    }

    private void CheckKeys()
    {
        forwardPressed.sprite = isForwardPressed ? directionPressedSprite : directionDefaultSprite;
        leftPressed.sprite = isLeftPressed ? directionPressedSprite : directionDefaultSprite;
        rightPressed.sprite = isRightPressed ? directionPressedSprite : directionDefaultSprite;
        backPressed.sprite = isBackPressed ? directionPressedSprite : directionDefaultSprite;
        jumpPressed.sprite = isJumpPressed ? jumpPressedSprite : jumpDefaultSprite;
        crouchPressed.sprite = isCrouchPressed ? crouchPressedSprite : crouchDefaultSprite;
    }

    private void CheckMouse()
    {
        if (isMouseLeftPressed && !isMouseRightPressed)
        {
            mousePressed.sprite = leftMousePressedSprite;
        }
        else if (!isMouseLeftPressed && isMouseRightPressed)
        {
            mousePressed.sprite = rightMousePressedSprite;
        }
        else if (isMouseLeftPressed && isMouseRightPressed)
        {
            mousePressed.sprite = bothMousePressedSprite;
        }
        else
        {
            mousePressed.sprite = defaultMousePressedSprite;
        }
    }
}
