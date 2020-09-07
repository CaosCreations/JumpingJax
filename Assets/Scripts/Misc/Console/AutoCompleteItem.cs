using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AutoCompleteItem : MonoBehaviour
{
    [SerializeField] private Text commandText;
    [SerializeField] private Text commandDescriptionText;

    private ConsoleCommand consoleCommand;
    public ConsoleCommand ConsoleCommand {
        get
        {
            return consoleCommand;
        }
        set
        {
            consoleCommand = value;
            commandText.text = value.Command;
            commandDescriptionText.text = value.Description;
        }
    }

    public void Highlight()
    {
        commandText.color = ConsoleConstants.highlightColor;
        commandDescriptionText.color = ConsoleConstants.highlightColor;
    }

    public void Unhighlight()
    {
        commandText.color = ConsoleConstants.autocompleteColor;
        commandDescriptionText.color = ConsoleConstants.autocompleteColor;
    }
}
