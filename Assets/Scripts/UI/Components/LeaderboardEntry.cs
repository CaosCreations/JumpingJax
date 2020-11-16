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

            Texture2D avatarTexture = LoadTextureFromImage(avatar.Value);

            Sprite sprite = Sprite.Create(avatarTexture, new Rect(0, 0, imageWidth, imageHeight), new Vector2(0.5f, 0.5f));
            avatarImage.sprite = sprite;
        }
        else
        {
            // TODO: idfk yet
        }
    }

    public static Texture2D LoadTextureFromImage(Steamworks.Data.Image img)
    {
        var texture = new Texture2D((int)img.Width, (int)img.Height);

        for (int x = 0; x < img.Width; x++)
            for (int y = 0; y < img.Height; y++)
            {
                var p = img.GetPixel(x, y);

                texture.SetPixel(x, (int)img.Height - y, new UnityEngine.Color32(p.r, p.g, p.b, p.a));
            }

        texture.Apply();

        return texture;
    }
}
