using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Game Info Command", menuName = "Developer Console/Commands/GameInfoCommand")]
public class CommandGameInfo : ConsoleCommand
{
    public override void Process(string[] args)
    {
        string steamUser = "not connected to steam";
        if (Steamworks.SteamClient.IsValid)
        {
            steamUser = Steamworks.SteamClient.Name;
        }
        Debug.Log($"Game Info: \n Version: {Application.version} \n Unity version: {Application.unityVersion} \n Steam user: {steamUser}");
    }
}
