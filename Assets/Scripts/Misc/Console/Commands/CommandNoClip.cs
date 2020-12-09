using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New NoClip Command", menuName = "Developer Console/Commands/NoClipCommand")]
public class CommandNoClip : ConsoleCommand
{
    public override void Process(string[] args)
    {
        PlayerMovement playerMovement = FindObjectOfType<PlayerMovement>(); 
        if(playerMovement != null)
        {
            playerMovement.NoClip();
        }
    }
}
