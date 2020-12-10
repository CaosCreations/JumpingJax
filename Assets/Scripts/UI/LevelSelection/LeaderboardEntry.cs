using System;
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
    public PrimaryButton replayButton;

    public async void Init(Steamworks.Data.LeaderboardEntry entry, Action replaySet)
    {
        place.text = entry.GlobalRank + ".";
        TimeSpan timeSpan = TimeSpan.FromMilliseconds(entry.Score);
        time.text = timeSpan.ToString(PlayerConstants.levelCompletionTimeFormat);
        playerName.text = entry.User.Name;

        Texture2D avatarTexture = await SteamCacheManager.GetUserAvatar(entry.User.Id);
        Sprite sprite = Sprite.Create(avatarTexture, new Rect(0, 0, avatarTexture.width, avatarTexture.height), new Vector2(0.5f, 0.5f));
        avatarImage.sprite = sprite;

        replayButton.Init(replaySet);
        replayButton.button.onClick.AddListener(SetButtonActive);
    }

    public void SetButtonActive()
    {
        replayButton.SetActive();
    }
}
