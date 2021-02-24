﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardEntry : MonoBehaviour
{
    public Text place;
    public Text time;
    public Text playerName;
    public Image avatarImage;

    public GameObject replayCheck;
    public Button replayButton;
    public LeaderboardTab leaderboardTab;

    public async void Init(Steamworks.Data.LeaderboardEntry entry, Action replaySet, LeaderboardTab tab, bool hasAttachedReplay)
    {
        this.leaderboardTab = tab;
        place.text = entry.GlobalRank + ".";
        TimeSpan timeSpan = TimeSpan.FromMilliseconds(entry.Score);
        time.text = timeSpan.ToString(PlayerConstants.levelCompletionTimeFormat);
        playerName.text = entry.User.Name;

        Texture2D avatarTexture = await SteamCacheManager.GetUserAvatar(entry.User.Id);
        Sprite sprite = Sprite.Create(avatarTexture, new Rect(0, 0, avatarTexture.width, avatarTexture.height), new Vector2(0.5f, 0.5f));
        avatarImage.sprite = sprite;

        if (hasAttachedReplay)
        {
            replayButton.onClick.AddListener(() => replaySet());
            replayButton.onClick.AddListener(SetButtonActive);
        }
        else
        {
            replayButton.gameObject.SetActive(false);
        }
        
    }

    public void SetButtonActive()
    {
        replayCheck.SetActive(true);
        //replayCheck.SetActive(!replayCheck.activeSelf);
    }

    public void ClearActive()
    {
        replayCheck.SetActive(false);
    }
}
