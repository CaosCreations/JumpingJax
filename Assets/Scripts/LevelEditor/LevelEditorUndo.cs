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
            //Destroy(commandHistory[counter].GetGameObject()); //problem here with it deleting objects when removing a move command
            commandHistory.RemoveAt(counter);
        }

        commandHistory.Add(command);
        counter++;
        Debug.Log("Length of Command History: " + commandHistory.Count);
    }


    // Update is called once per frame
    void Update()
    {
        //(Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.LeftControl)) && Input.GetKeyDown(KeyCode.Z) if you want ctrl + z
        if (Input.GetKeyDown(KeyCode.U))
        {
            if(counter > 0)
            {

                counter--;
                switch (commandHistory[counter].CommandName())
                {
                    case CommandNames.create:
                        commandHistory[counter].Undo();
                        Debug.Log("Undo create");
                        break;
                    case CommandNames.delete:
                        commandHistory[counter].Undo(); 
                        Debug.Log("Undo delete");
                        break;
                    case CommandNames.position:
                        commandHistory[counter].GetGameObject().transform.position = commandHistory[counter].PrevPosition();
                        Debug.Log("Undo movement");
                        break;
                    case CommandNames.rotation:
                        commandHistory[counter].GetGameObject().transform.rotation = commandHistory[counter].PrevRotation();
                        Debug.Log("Undo rotation");
                        break;
                    case CommandNames.scale:
                        commandHistory[counter].GetGameObject().transform.localScale = commandHistory[counter].PrevScale();
                        Debug.Log("Undo scale");
                        break;
                    default:
                        Debug.Log("Default case");
                        break;
                }
                Debug.Log("Length of Command History: " + commandHistory.Count);
                Debug.Log("Counter is at " + counter);
            }
        }
        else if (Input.GetKeyDown(KeyCode.P))
        {
            if (counter < commandHistory.Count)
            {
                switch (commandHistory[counter].CommandName())
                {
                    case CommandNames.create:
                        commandHistory[counter].Undo(); 
                        Debug.Log("Redo create");
                        break;
                    case CommandNames.delete:
                        commandHistory[counter].Undo();
                        Debug.Log("Redo delete");
                        break;
                    case CommandNames.position:
                        commandHistory[counter].GetGameObject().transform.position = commandHistory[counter].Position();
                        Debug.Log("Redo movement");
                        break;
                    case CommandNames.rotation:
                        commandHistory[counter].GetGameObject().transform.rotation = commandHistory[counter].Rotation();
                        Debug.Log("Redo rotation");
                        break;
                    case CommandNames.scale:
                        commandHistory[counter].GetGameObject().transform.localScale = commandHistory[counter].Scale();
                        Debug.Log("Redo scale");
                        break;
                    default:
                        Debug.Log("Default case");
                        break;
                }
                counter++;
                Debug.Log("Length of Command History: " + commandHistory.Count);
                Debug.Log("Counter is at " + counter);
            }
        }
        
    }
}
