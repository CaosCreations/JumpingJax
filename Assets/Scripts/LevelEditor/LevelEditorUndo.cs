using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEditorUndo : MonoBehaviour
{
    public static Vector3 prevPos;
    public static Quaternion prevRotation;
    public static Vector3 prevScale;

    private static bool shouldDebug = false;
    private bool shiftPressed;
    private bool ctrlPressed;
    private bool zPressed;
    private bool undo;
    private bool redo;

    public static List<ICommand> commandHistory;
    static int counter;
    void Awake()
    {
        commandHistory = new List<ICommand>();
    }
    public static void AddCommand(ICommand command)
    {
        if (commandHistory.Count > counter)
        {
            commandHistory.RemoveRange(counter, commandHistory.Count - counter);
        }
        commandHistory.Add(command);
        counter++;

        if (shouldDebug == true){Debug.Log("Length of Command History: " + commandHistory.Count); }
    }
    void Update()
    {
        CheckUndo();
        if (InputManager.GetKeyDown(PlayerConstants.Undo) || undo) 
        {
            if (counter > 0)
            {
                counter--;
                commandHistory[counter].Undo();
                zPressed = false;

                if (shouldDebug == true)
                {
                    Debug.Log($"Undo {commandHistory[counter].CommandName()}");
                    Debug.Log("Length of Command History: " + commandHistory.Count);
                    Debug.Log("Counter is at " + counter);
                }
            }
        }
        else if (InputManager.GetKeyDown(PlayerConstants.Redo) || redo)
        {
            if (counter < commandHistory.Count)
            {
                commandHistory[counter].Redo();
                if (shouldDebug == true) { Debug.Log($"Redo {commandHistory[counter].CommandName()}"); } //this debug log has to happen before the counter increments
                counter++;
                zPressed = false;

                if (shouldDebug == true)
                {
                    Debug.Log("Length of Command History: " + commandHistory.Count);
                    Debug.Log("Counter is at " + counter);
                }
            }
        }
    }

    public void CheckUndo()
    {
        if (Input.GetKeyDown(KeyCode.RightControl) || Input.GetKeyDown(KeyCode.LeftControl))
        {
            ctrlPressed = true;
        }

        if (Input.GetKeyUp(KeyCode.RightControl) || Input.GetKeyUp(KeyCode.LeftControl))
        {
            ctrlPressed = false;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            shiftPressed = true;
        }

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            shiftPressed = false;
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            zPressed = true;
        }

        if (Input.GetKeyUp(KeyCode.Z))
        {
            zPressed = false;
        }

        if (ctrlPressed && zPressed && shiftPressed) { redo = true; }
        else { redo = false; }

        if (ctrlPressed && zPressed && !redo) { undo = true; }
        else { undo = false; }
    }
}
