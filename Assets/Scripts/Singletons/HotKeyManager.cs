﻿using System;
using System.Collections.Generic;
using UnityEngine;

public class HotKeyManager : MonoBehaviour {
    [SerializeField]
    private Dictionary<string, KeyCode> keys = new Dictionary<string, KeyCode>();
    private Dictionary<string, KeyCode> defaults = new Dictionary<string, KeyCode>();
    private Dictionary<string, string> tooltips = new Dictionary<string, string>();

    public static HotKeyManager Instance;

    public event Action<KeyCode, KeyCode> onHotKeySet;   

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        InitDefaults();
        LoadSavedHotkeys();
        LoadTooltips();
    }

    public Dictionary<String, KeyCode> GetHotKeys()
    {
        return keys;
    }
	
	public Dictionary<String, KeyCode> GetDefaultHotKeys() 
	{
		return defaults;
	}

    public Dictionary<string, string> GetTooltips()
    {
        return tooltips;
    }

    public void SetButtonForKey(string key, KeyCode keyCode)
    {

        KeyCode oldKeyCode = keys[key];
        keys[key] = keyCode;
        onHotKeySet?.Invoke(oldKeyCode, keys[key]);
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
        LoadSavedKey(PlayerConstants.Undo, PlayerConstants.UndoDefault);
        LoadSavedKey(PlayerConstants.Redo, PlayerConstants.RedoDefault);
        LoadSavedKey(PlayerConstants.ResetLevel, PlayerConstants.ResetLevelDefault);
        LoadSavedKey(PlayerConstants.Respawn, PlayerConstants.RespawnDefault);
        LoadSavedKey(PlayerConstants.Portal1, PlayerConstants.Portal1Default);
        LoadSavedKey(PlayerConstants.Portal2, PlayerConstants.Portal2Default);
        LoadSavedKey(PlayerConstants.FirstPersonGhost, PlayerConstants.FirstPersonGhostDefault);
        LoadSavedKey(PlayerConstants.ToggleUI, PlayerConstants.ToggleUIDefault);
        LoadSavedKey(PlayerConstants.LevelEditorSpeedIncrease, PlayerConstants.LevelEditorSpeedIncreaseDefault);
        LoadSavedKey(PlayerConstants.LevelEditorSpeedDecrease, PlayerConstants.LevelEditorSpeedDecreaseDefault);
        LoadSavedKey(PlayerConstants.LevelEditorXAxisSelect, PlayerConstants.LevelEditorXAxisSelectDefault);
        LoadSavedKey(PlayerConstants.LevelEditorYAxisSelect, PlayerConstants.LevelEditorYAxisSelectDefault);
        LoadSavedKey(PlayerConstants.LevelEditorZAxisSelect, PlayerConstants.LevelEditorZAxisSelectDefault);
    }

    public void LoadSavedKey(string keyName, string defaultValue)
    {
        string key = PlayerPrefs.GetString(keyName, defaultValue);

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
            Debug.Log("SetDefaults KVP: " + entry.Key + ": " + entry.Value); 
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
        AddDefaultKey(PlayerConstants.Undo, PlayerConstants.UndoDefault);
        AddDefaultKey(PlayerConstants.Redo, PlayerConstants.RedoDefault);
        AddDefaultKey(PlayerConstants.ResetLevel, PlayerConstants.ResetLevelDefault);
        AddDefaultKey(PlayerConstants.Respawn, PlayerConstants.RespawnDefault);
        AddDefaultKey(PlayerConstants.Portal1, PlayerConstants.Portal1Default);
        AddDefaultKey(PlayerConstants.Portal2, PlayerConstants.Portal2Default);
        AddDefaultKey(PlayerConstants.FirstPersonGhost, PlayerConstants.FirstPersonGhostDefault);
        AddDefaultKey(PlayerConstants.ToggleUI, PlayerConstants.ToggleUIDefault);
        AddDefaultKey(PlayerConstants.LevelEditorSpeedIncrease, PlayerConstants.LevelEditorSpeedIncreaseDefault);
        AddDefaultKey(PlayerConstants.LevelEditorSpeedDecrease, PlayerConstants.LevelEditorSpeedDecreaseDefault);
        AddDefaultKey(PlayerConstants.LevelEditorXAxisSelect, PlayerConstants.LevelEditorXAxisSelectDefault);
        AddDefaultKey(PlayerConstants.LevelEditorYAxisSelect, PlayerConstants.LevelEditorYAxisSelectDefault);
        AddDefaultKey(PlayerConstants.LevelEditorZAxisSelect, PlayerConstants.LevelEditorZAxisSelectDefault);
    }

    public void AddDefaultKey(string keyName, string defaultValue)
    {
        KeyCode keyCode;
        if (Enum.TryParse(defaultValue, out keyCode))
        {
            defaults.Add(keyName, keyCode);
        }
        else
        {
            Debug.Log("Could not parse default key code: " + keyName);
        }
    }

    public void LoadTooltips()
    {
        tooltips.Add(PlayerConstants.Forward, PlayerConstants.ForwardTooltip);
        tooltips.Add(PlayerConstants.Back, PlayerConstants.BackTooltip);
        tooltips.Add(PlayerConstants.Left, PlayerConstants.LeftTooltip);
        tooltips.Add(PlayerConstants.Right, PlayerConstants.RightTooltip);
        tooltips.Add(PlayerConstants.Jump, PlayerConstants.JumpTooltip);
        tooltips.Add(PlayerConstants.Crouch, PlayerConstants.CrouchTooltip);
        tooltips.Add(PlayerConstants.Undo, PlayerConstants.UndoTooltip);
        tooltips.Add(PlayerConstants.Redo, PlayerConstants.RedoTooltip);
        tooltips.Add(PlayerConstants.ResetLevel, PlayerConstants.ResetTooltip);
        tooltips.Add(PlayerConstants.Respawn, PlayerConstants.RespawnTooltip);
        tooltips.Add(PlayerConstants.Portal1, PlayerConstants.Portal1Tooltip);
        tooltips.Add(PlayerConstants.Portal2, PlayerConstants.Portal2Tooltip);
        tooltips.Add(PlayerConstants.ToggleUI, PlayerConstants.ToggleUITooltip);
        tooltips.Add(PlayerConstants.LevelEditorSpeedIncrease, PlayerConstants.LevelEditorSpeedIncreaseTooltip);
        tooltips.Add(PlayerConstants.LevelEditorSpeedDecrease, PlayerConstants.LevelEditorSpeedDecreaseTooltip);
        tooltips.Add(PlayerConstants.FirstPersonGhost, PlayerConstants.FirstPersonGhostTooltip);
        tooltips.Add(PlayerConstants.LevelEditorXAxisSelect, PlayerConstants.LevelEditorXAxisSelectTooltip);
        tooltips.Add(PlayerConstants.LevelEditorYAxisSelect, PlayerConstants.LevelEditorYAxisSelectTooltip);
        tooltips.Add(PlayerConstants.LevelEditorZAxisSelect, PlayerConstants.LevelEditorZAxisSelectTooltip);
    }
}