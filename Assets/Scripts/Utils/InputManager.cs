using UnityEngine;

public class InputManager
{
    public static bool GetKeyDown(string keyName)
    {
        return Input.GetKeyDown(HotKeyManager.Instance.GetKeyFor(keyName));
    }

    public static bool GetKeyUp(string keyName)
    {
        return Input.GetKeyUp(HotKeyManager.Instance.GetKeyFor(keyName));
    }

    public static bool GetKey(string keyName)
    {
        return Input.GetKey(HotKeyManager.Instance.GetKeyFor(keyName));
    }

    public static bool GetModifiedKeyDown(string keyName)
    {
        return Input.GetKey(HotKeyManager.Instance.GetKeyFor(PlayerConstants.ModifierKey)) 
            && Input.GetKeyDown(HotKeyManager.Instance.GetKeyFor(keyName));
    }
}
