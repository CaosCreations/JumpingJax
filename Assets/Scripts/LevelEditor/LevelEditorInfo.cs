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

    public LevelEditor levelEditor;

    private void Start()
    {
        levelEditor = GetComponentInParent<LevelEditor>();
    }

    public void Init(Level level)
    {
        nameInput.onValueChanged.RemoveAllListeners();
        descriptionInput.onValueChanged.RemoveAllListeners();

        if (level == null)
        {
            this.level = null;

            nameInput.text = string.Empty;
            nameInput.interactable = false;

            descriptionInput.text = string.Empty;
            descriptionInput.interactable = false;

            previewImage.sprite = noImageSprite;
            return;
        }

        this.level = level;

        nameInput.text = level.levelName;
        nameInput.onValueChanged.AddListener((value) => UpdateLevelName(value));
        nameInput.interactable = true;

        descriptionInput.text = level.description;
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

        // Update button label
        levelEditor.UpdateLevelNames(text);
        // Refresh validation
        levelEditor.CanPublish();
    }

    private void UpdateLevelDescription(string text)
    {
        level.description = text;
        SaveAsset();

        // Refresh validation
        levelEditor.CanPublish();
    }

    private void SaveAsset()
    {
        File.WriteAllText(level.levelEditorScriptableObjectPath, JsonUtility.ToJson(level));
    }
}
