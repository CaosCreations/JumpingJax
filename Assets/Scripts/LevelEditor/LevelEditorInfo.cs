using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class LevelEditorInfo : MonoBehaviour
{
    public Sprite noImageSprite;

    public InputField nameInput;
    public InputField descriptionInput;
    public Image previewImage;

    public Level level;

    public static event Action<string> onLevelNameUpdated;

    public void Init(Level level)
    {
        if(level == null)
        {
            this.level = null;

            nameInput.text = string.Empty;
            nameInput.onEndEdit.RemoveAllListeners();
            nameInput.interactable = false;

            descriptionInput.text = string.Empty;
            descriptionInput.onEndEdit.RemoveAllListeners();
            descriptionInput.interactable = false;

            previewImage.sprite = noImageSprite;
            return;
        }

        this.level = level;

        nameInput.text = level.levelName;
        nameInput.onValueChanged.RemoveAllListeners();
        nameInput.onValueChanged.AddListener((value) => UpdateLevelName(value));
        // Update button label
        nameInput.onValueChanged.AddListener((value) => onLevelNameUpdated?.Invoke(value));
        nameInput.interactable = true;

        descriptionInput.text = level.description;
        descriptionInput.onValueChanged.RemoveAllListeners();
        descriptionInput.onValueChanged.AddListener((value) => UpdateLevelDescription(value));
        descriptionInput.interactable = true;

        if (level.previewSprite == null)
        {
            previewImage.sprite = noImageSprite;
        }
        else
        {
            previewImage.sprite = level.previewSprite;
        }
    }

    private void UpdateLevelName(string text)
    {
        if(level != null)
        {
            level.levelName = text;
            SaveAsset();
        }
    }

    private void UpdateLevelDescription(string text)
    {
        level.description = text;
        SaveAsset();
    }

    private void SaveAsset()
    {
        File.WriteAllText(level.levelEditorScriptableObjectPath, JsonUtility.ToJson(level));
    }
}
