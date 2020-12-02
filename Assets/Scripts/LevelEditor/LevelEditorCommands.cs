using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum CommandNames
{
    create, delete, position, rotation, scale
}
public class LevelEditorCommands : ICommand
{
    public GameObject gameObject;
    public CommandNames commandName; 
    public CommandNames CommandName()
    {
        return commandName;
    }
    public GameObject GetGameObject()
    {
        return gameObject;
    }
    public virtual void Undo() { }
    public virtual void Redo() { }
}
public class CreateObjectCommand : LevelEditorCommands
{
    public CreateObjectCommand(GameObject gameObject)
    {
        this.gameObject = gameObject;
        this.commandName = CommandNames.create;
    }
    override public void Undo()
    {
        Inspector.FlipActive(this.gameObject);
    }
}
public class DeleteObjectCommand : LevelEditorCommands
{
    public DeleteObjectCommand(GameObject gameObject)
    {
        this.gameObject = gameObject;
        this.commandName = CommandNames.delete;
    }
    override public void Undo()
    {
        Inspector.FlipActive(gameObject);
    }
}
public class MoveObjectCommand : LevelEditorCommands
{
    Vector3 position;
    Vector3 prevPos;
    public MoveObjectCommand(GameObject gameObject, Vector3 position, Vector3 prevPos)
    {
        this.gameObject = gameObject;
        this.commandName = CommandNames.position;
        this.position = position;
        this.prevPos = prevPos;
    }
    override public void Undo()
    {
        gameObject.transform.position = prevPos;
    }
    override public void Redo()
    {
        gameObject.transform.position = position;
    }
}
public class RotateObjectCommand : LevelEditorCommands
{
    Quaternion rotation;
    Quaternion prevRotation;
    public RotateObjectCommand(GameObject gameObject, Quaternion rotation, Quaternion prevRotation)
    {
        this.gameObject = gameObject;
        this.commandName = CommandNames.rotation;
        this.rotation = rotation;
        this.prevRotation = prevRotation;
    }
    override public void Undo()
    {
        gameObject.transform.rotation = prevRotation;
    }
}
public class ScaleObjectCommand : LevelEditorCommands
{
    Vector3 scale;
    Vector3 prevScale;
    public ScaleObjectCommand(GameObject gameObject, Vector3 scale, Vector3 prevScale)
    {
        this.gameObject = gameObject;
        this.commandName = CommandNames.scale;
        this.scale = scale;
        this.prevScale = prevScale;
    }
    override public void Undo()
    {
        gameObject.transform.localScale = prevScale;
    }
}