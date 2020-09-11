using Steamworks;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class WipeSteamStats : MonoBehaviour
{
    [MenuItem("Tools/CaosCreations/Wipe stats")]
    private static void WipeStats()
    {
        SteamUtil.WipeAllStats();
    }

}
