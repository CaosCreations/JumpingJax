using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectionCard : MonoBehaviour
{
    public Button selectButton;
    public LevelSelectionTab tab;

    public Image previewOutlineImage;
    public Image previewImage;

    public Text levelNameText;
    public Text bestTimeText;

    public Image boneImage1;
    public Image boneImage2;
    public Image boneImage3;

    public Sprite boneEmptySprite;
    public Sprite boneEmptyInactiveSprite;
    public Sprite boneFilledSprite;

    public Sprite outlineActiveSprite;
    public Sprite outlineInactiveSprite;
    public Sprite noImageSprite;

    public void Init(Level level)
    {
        levelNameText.text = level.levelName;
        levelNameText.color = level.levelSaveData.isCompleted ? PlayerConstants.activeColor : PlayerConstants.inactiveColor;
        SetBestTime(level);
        SetPreviewImage(level);
        SetBoneImages(level);
        SetTab(level);
    }

    private void SetBestTime(Level level)
    {
        if (level.levelSaveData.isCompleted)
        {
            TimeSpan time = TimeSpan.FromSeconds(level.levelSaveData.completionTime);
            string timeString = time.ToString(PlayerConstants.levelCompletionTimeFormat);
            bestTimeText.text = "Best Time: " + timeString;
            bestTimeText.color = PlayerConstants.activeColor;
        }
        else
        {
            bestTimeText.text = "Best Time: N/A";
            bestTimeText.color = PlayerConstants.inactiveColor;
        }
    }

    private void SetPreviewImage(Level level)
    {
        if (level.previewSprite == null)
        {
            previewImage.sprite = noImageSprite;
        }
        else
        {
            previewImage.sprite = level.previewSprite;
        }


        if (level.levelSaveData.isCompleted)
        {
            previewOutlineImage.sprite = outlineActiveSprite;
        }
        else
        {
            previewOutlineImage.sprite = outlineInactiveSprite;
        }
    }

    private void SetBoneImages(Level level)
    {
        if (!level.levelSaveData.isCompleted)
        {
            boneImage1.sprite = boneEmptyInactiveSprite;
            boneImage2.sprite = boneEmptyInactiveSprite;
            boneImage3.sprite = boneEmptyInactiveSprite;
            return;
        }

        int numberOfBones = level.GetNumberOfTimeBones();

        boneImage1.sprite = boneEmptySprite;
        boneImage2.sprite = boneEmptySprite;
        boneImage3.sprite = boneEmptySprite;

        if (numberOfBones >= 1)
        {
            boneImage1.sprite = boneFilledSprite;
        }

        if (numberOfBones >= 2)
        {
            boneImage2.sprite = boneFilledSprite;
        }

        if (numberOfBones == 3)
        {
            boneImage3.sprite = boneFilledSprite;
        }

    }

    private void SetTab(Level level)
    {
        if (level.workshopFilePath != string.Empty)
        {
            tab = LevelSelectionTab.Workshop;
        }
        else if (level.levelName.ToLower().Contains("portal"))
        {
            tab = LevelSelectionTab.Portal;
        }
        else
        {
            tab = LevelSelectionTab.Hop;
        }
    }
}
