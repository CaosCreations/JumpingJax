using System.Collections;
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

    void Update()
    {
        CheckKeys();
        CheckMouse();
    }

    private void CheckKeys()
    {
        forwardPressed.sprite = InputManager.GetKey(PlayerConstants.Forward) ? directionPressedSprite : directionDefaultSprite;
        leftPressed.sprite = InputManager.GetKey(PlayerConstants.Left) ? directionPressedSprite : directionDefaultSprite;
        rightPressed.sprite = InputManager.GetKey(PlayerConstants.Right) ? directionPressedSprite : directionDefaultSprite;
        backPressed.sprite = InputManager.GetKey(PlayerConstants.Back) ? directionPressedSprite : directionDefaultSprite;
        jumpPressed.sprite = InputManager.GetKey(PlayerConstants.Jump) ? jumpPressedSprite : jumpDefaultSprite;
        crouchPressed.sprite = InputManager.GetKey(PlayerConstants.Crouch) ? crouchPressedSprite : crouchDefaultSprite;
    }

    private void CheckMouse()
    {
        bool mouse1Pressed = InputManager.GetKey(PlayerConstants.Portal1);
        bool mouse2Pressed = InputManager.GetKey(PlayerConstants.Portal2);
        if (mouse1Pressed && !mouse2Pressed)
        {
            mousePressed.sprite = leftMousePressedSprite;
        }
        else if (!mouse1Pressed && mouse2Pressed)
        {
            mousePressed.sprite = rightMousePressedSprite;
        }
        else if (mouse1Pressed && mouse2Pressed)
        {
            mousePressed.sprite = bothMousePressedSprite;
        }
        else
        {
            mousePressed.sprite = defaultMousePressedSprite;
        }
    }
}
