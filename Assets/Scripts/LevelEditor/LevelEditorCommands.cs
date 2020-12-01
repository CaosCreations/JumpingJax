using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CommandNames
{
    create, delete, position, rotation, scale
}
public class LevelEditorCommands : ICommand
{
    GameObject gameObject;
    Vector3 position;
    Vector3 prevPos;
    Quaternion rotation;
    Quaternion prevRotation;
    Vector3 scale;
    Vector3 prevScale;
    CommandNames commandName; 

    public LevelEditorCommands(GameObject gameObject, Vector3 position, Vector3 prevPos, Quaternion rotation, Quaternion prevRotation, Vector3 scale, Vector3 prevScale, CommandNames commandName)
    {
        this.gameObject = gameObject;
        this.position = position;
        this.prevPos = prevPos;
        this.rotation = rotation;
        this.prevRotation = prevRotation;
        this.scale = scale;
        this.prevScale = prevScale;
        this.commandName = commandName;
    }

    public CommandNames CommandName()
    {
        return commandName;
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    public Vector3 Position()
    {
        return position;
    }

    public Vector3 PrevPosition()
    {
        return prevPos;
    }

    public Quaternion Rotation()
    {
        return rotation;
    }

    public Quaternion PrevRotation()
    {
        return prevRotation;
    }

    public Vector3 Scale()
    {
        return scale;
    }

    public Vector3 PrevScale()
    {
        return prevScale;
    }


    public void Execute()
    {
        LevelEditorHUD.Create(gameObject, gameObject.transform.position);
    }

    public void Undo()
    {
        if(gameObject.transform.position == position)
        {
            Inspector.FlipActive(this.gameObject);
        }
        
    }

    
}
