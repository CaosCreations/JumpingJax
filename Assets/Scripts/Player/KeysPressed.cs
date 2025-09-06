using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class KeysPressed
{
    [SerializeField]
    public bool isForwardPressed;

    [SerializeField]
    public bool isLeftPressed;

    [SerializeField]
    public bool isRightPressed;

    [SerializeField]
    public bool isBackPressed;

    [SerializeField]
    public bool isJumpPressed;

    [SerializeField]
    public bool isCrouchPressed;

    [SerializeField]
    public bool isMouseLeftPressed;

    [SerializeField]
    public bool isMouseRightPressed;

    public int GetFlags()
    {
        int flags = 0;
        if (isForwardPressed) flags += (int)KeyPressFlags.Forward;
        if (isLeftPressed) flags += (int)KeyPressFlags.Left;
        if (isRightPressed) flags += (int)KeyPressFlags.Right;
        if (isBackPressed) flags += (int)KeyPressFlags.Back;
        if (isJumpPressed) flags += (int)KeyPressFlags.Jump;
        if (isCrouchPressed) flags += (int)KeyPressFlags.Crouch;
        if (isMouseLeftPressed) flags += (int)KeyPressFlags.MouseLeft;
        if (isMouseRightPressed) flags += (int)KeyPressFlags.MouseRight;
        return flags;
    }


    public void SetFlags(int flags)
    {
        isForwardPressed = (flags & (int)KeyPressFlags.Forward) != 0;
        isLeftPressed = (flags & (int)KeyPressFlags.Left) != 0;
        isRightPressed = (flags & (int)KeyPressFlags.Right) != 0;
        isBackPressed = (flags & (int)KeyPressFlags.Back) != 0;
        isJumpPressed = (flags & (int)KeyPressFlags.Jump) != 0;
        isCrouchPressed = (flags & (int)KeyPressFlags.Crouch) != 0;
        isMouseLeftPressed = (flags & (int)KeyPressFlags.MouseLeft) != 0;
        isMouseRightPressed = (flags & (int)KeyPressFlags.MouseRight) != 0;
    }
}

[Flags]
public enum KeyPressFlags
{
    Forward = 1,
    Left = 2,
    Right = 4,
    Back = 8,
    Jump = 16,
    Crouch = 32,
    MouseLeft = 64,
    MouseRight = 128
}