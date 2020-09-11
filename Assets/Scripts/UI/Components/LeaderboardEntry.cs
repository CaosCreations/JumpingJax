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
    public Button replayButton;

    public void Init(Steamworks.Data.LeaderboardEntry entry)
    {
        place.text = entry.GlobalRank + ".";
        TimeSpan timeSpan = TimeSpan.FromTicks(entry.Score);
        time.text = timeSpan.ToString(PlayerConstants.levelCompletionTimeFormat);
        playerName.text = entry.User.Name;
    }
}
