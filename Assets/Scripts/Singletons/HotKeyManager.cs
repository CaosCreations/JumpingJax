using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotKeyManager : MonoBehaviour {
    [SerializeField]
    private Dictionary<string, KeyCode> keys = new Dictionary<string, KeyCode>();
    private Dictionary<string, KeyCode> defaults = new Dictionary<string, KeyCode>();
    //private Dictionary<string, KeyCode> modifiedKeys = new Dictionary<string, KeyCode>(); 

    public static HotKeyManager Instance;
    

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        InitDefaults();
        LoadSavedHotkeys();
    }

    public Dictionary<String, KeyCode> GetHotKeys()
    {
        return keys;
    }

    public void SetButtonForKey(string key, KeyCode keyCode)
    {
        keys[key] = keyCode;
        PlayerPrefs.SetString(key, keyCode.ToString());
    }

    public KeyCode GetKeyFor(string action)
    {
        return keys[action];
    }

    public void LoadSavedHotkeys()
    {
        LoadSavedKey(PlayerConstants.Forward, PlayerConstants.ForwardDefault);
        LoadSavedKey(PlayerConstants.Back, PlayerConstants.BackDefault);
        LoadSavedKey(PlayerConstants.Left, PlayerConstants.LeftDefault);
        LoadSavedKey(PlayerConstants.Right, PlayerConstants.RightDefault);
        LoadSavedKey(PlayerConstants.Jump, PlayerConstants.JumpDefault);
        LoadSavedKey(PlayerConstants.Crouch, PlayerConstants.CrouchDefault);
        LoadSavedKey(PlayerConstants.ResetLevel, PlayerConstants.ResetLevelDefault);
        LoadSavedKey(PlayerConstants.Portal1, PlayerConstants.Portal1Default);
        LoadSavedKey(PlayerConstants.Portal2, PlayerConstants.Portal2Default);
        LoadSavedKey(PlayerConstants.Modifier, PlayerConstants.ModifierDefault);
        LoadSavedKey(PlayerConstants.ToggleUI, PlayerConstants.ToggleUIDefault); 
    }

    public void LoadSavedKey(string keyName, string defaultValue)
    {
        string key = PlayerPrefs.GetString(keyName, defaultValue);
        //Debug.Log("Loading saved key: " + key); 

        KeyCode keyCode;
        if (Enum.TryParse(key, out keyCode))
        {
            keys.Add(keyName, keyCode);
        }
        else
        {
            Debug.Log("Could not parse key code: " + keyName);
        }
    }

    public void SetDefaults()
    {
        keys = new Dictionary<string, KeyCode>(defaults);

        foreach(KeyValuePair<String, KeyCode> entry in keys)
        {
            Debug.Log("SefDefaults KVP: " + entry.Key + ": " + entry.Value); 
            PlayerPrefs.SetString(entry.Key, entry.Value.ToString());
        }
    }

    public void InitDefaults()
    {
        AddDefaultKey(PlayerConstants.Forward, PlayerConstants.ForwardDefault);
        AddDefaultKey(PlayerConstants.Back, PlayerConstants.BackDefault);
        AddDefaultKey(PlayerConstants.Left, PlayerConstants.LeftDefault);
        AddDefaultKey(PlayerConstants.Right, PlayerConstants.RightDefault);
        AddDefaultKey(PlayerConstants.Jump, PlayerConstants.JumpDefault);
        AddDefaultKey(PlayerConstants.Crouch, PlayerConstants.CrouchDefault);
        AddDefaultKey(PlayerConstants.ResetLevel, PlayerConstants.ResetLevelDefault);
        AddDefaultKey(PlayerConstants.Portal1, PlayerConstants.Portal1Default);
        AddDefaultKey(PlayerConstants.Portal2, PlayerConstants.Portal2Default);
        AddDefaultKey(PlayerConstants.Modifier, PlayerConstants.ModifierDefault);
        AddDefaultKey(PlayerConstants.ToggleUI, PlayerConstants.ToggleUIDefault); 
    }

    public void AddDefaultKey(string keyName, string defaultValue)
    {
        KeyCode keyCode;
        if (Enum.TryParse(defaultValue, out keyCode))
        {
            Debug.Log($"AddDefaultKey defValue: {defaultValue}");
            defaults.Add(keyName, keyCode);
        }
        else
        {
            Debug.Log("Could not parse default key code: " + keyName);
        }
    }
}