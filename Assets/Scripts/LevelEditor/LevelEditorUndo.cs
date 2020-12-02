using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEditorUndo : MonoBehaviour
{
    public static List<ICommand> commandHistory;
    static int counter;
    void Awake()
    {
        commandHistory = new List<ICommand>();
    }
    public static void AddCommand(ICommand command)
    {
        while (commandHistory.Count > counter)
        {
            commandHistory.RemoveAt(counter);
        }
        commandHistory.Add(command);
        counter++;
        Debug.Log("Length of Command History: " + commandHistory.Count);
    }
    void Update()
    {
        //(Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.LeftControl)) && Input.GetKeyDown(KeyCode.Z) if you want ctrl + z
        if (InputManager.GetKeyDown(PlayerConstants.Undo))
        {
            if(counter > 0)
            {
                counter--;
                commandHistory[counter].Undo();

                Debug.Log($"Undo {commandHistory[counter].CommandName()}");
                Debug.Log("Length of Command History: " + commandHistory.Count);
                Debug.Log("Counter is at " + counter);
            }
        }
        else if (InputManager.GetKeyDown(PlayerConstants.Redo))
        {
            if (counter < commandHistory.Count)
            {
                commandHistory[counter].Redo();

                Debug.Log($"Redo {commandHistory[counter].CommandName()}"); //this debug log has to happen before the counter increments
                
                counter++;
                Debug.Log("Length of Command History: " + commandHistory.Count);
                Debug.Log("Counter is at " + counter);
            }
        }
    }
}
