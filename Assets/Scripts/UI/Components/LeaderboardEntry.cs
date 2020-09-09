using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardEntry : MonoBehaviour
{
    public Text place;
    public Text time;
    public Text playerName;

    public void Init(Steamworks.Data.LeaderboardEntry entry)
    {
        place.text = entry.GlobalRank + ".";
        time.text = entry.Score.ToString();
        playerName.text = entry.User.Name;
    }
}
