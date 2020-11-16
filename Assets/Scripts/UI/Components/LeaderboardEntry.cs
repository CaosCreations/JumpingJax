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

    public async void Init(Steamworks.Data.LeaderboardEntry entry)
    {
        place.text = entry.GlobalRank + ".";
        TimeSpan timeSpan = TimeSpan.FromTicks(entry.Score);
        time.text = timeSpan.ToString(PlayerConstants.levelCompletionTimeFormat);
        playerName.text = entry.User.Name;

        Steamworks.Data.Image? avatar = await SteamUtil.GetSteamFriendAvatar(entry.User.Id);

        if (avatar.HasValue)
        {
            Steamworks.Data.Image unboxxedAvatar = avatar.Value;
            int imageWidth = Convert.ToInt32(unboxxedAvatar.Width);
            int imageHeight = Convert.ToInt32(unboxxedAvatar.Height);
            
            Texture2D avatarTexture = new Texture2D(imageWidth, imageHeight);
            avatarTexture.LoadImage(unboxxedAvatar.Data);

            Sprite sprite = Sprite.Create(avatarTexture, new Rect(0, 0, imageWidth, imageHeight), new Vector2(0.5f, 0.5f));
            avatarImage.sprite = sprite;
        }
        else
        {
            // TODO: idfk yet
        }
    }
}
