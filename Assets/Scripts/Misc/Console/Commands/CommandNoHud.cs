using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New NoHud Command", menuName = "Developer Console/Commands/NoHud Command")]
public class CommandNoHud : ConsoleCommand
{
    public override void Process(string[] args)
    {
        try
        {
            FindObjectOfType<InGameUI>().ToggleUI();
        }
        catch
        {
            Debug.Log("No InGameUI found");
        }
    }
}
