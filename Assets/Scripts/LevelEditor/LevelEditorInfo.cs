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

            descriptionInput.text = string.Empty;
            descriptionInput.onEndEdit.RemoveAllListeners();

            previewImage.sprite = noImageSprite;
            return;
        }

        this.level = level;

        nameInput.text = level.levelName;
        nameInput.onEndEdit.RemoveAllListeners();
        nameInput.onEndEdit.AddListener((value) => UpdateLevelName(value));
        nameInput.onEndEdit.AddListener((value) => onLevelNameUpdated?.Invoke(value));

        descriptionInput.text = level.description;
        descriptionInput.onEndEdit.RemoveAllListeners();
        descriptionInput.onEndEdit.AddListener((value) => UpdateLevelDescription(value));

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
        level.levelName = text;
        SaveAsset();
    }

    private void UpdateLevelDescription(string text)
    {
        level.description = text;
        SaveAsset();
    }

    private void SaveAsset()
    {
        File.WriteAllText(level.levelEditorFilePath, JsonUtility.ToJson(level));
    }
}
