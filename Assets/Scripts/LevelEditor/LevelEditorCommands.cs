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
public class CreateCommand : LevelEditorCommands
{
    public CreateCommand(GameObject gameObject)
    {
        this.gameObject = gameObject;
        this.commandName = CommandNames.create;
    }
    override public void Undo()
    {
        Inspector.FlipActive(this.gameObject);
    }
    override public void Redo()
    {
        Inspector.FlipActive(this.gameObject);
    }
}
public class DeleteCommand : LevelEditorCommands
{
    public DeleteCommand(GameObject gameObject)
    {
        this.gameObject = gameObject;
        this.commandName = CommandNames.delete;
    }
    override public void Undo()
    {
        Inspector.FlipActive(gameObject);
    }
    override public void Redo()
    {
        Inspector.FlipActive(this.gameObject);
    }
}
public class PositionCommand : LevelEditorCommands
{
    Vector3 position;
    Vector3 prevPos;
    public PositionCommand(GameObject gameObject, Vector3 position, Vector3 prevPos)
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
public class RotateCommand : LevelEditorCommands
{
    Quaternion rotation;
    Quaternion prevRotation;
    public RotateCommand(GameObject gameObject, Quaternion rotation, Quaternion prevRotation)
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
    override public void Redo()
    {
        gameObject.transform.rotation = rotation;
    }
}
public class ScaleCommand : LevelEditorCommands
{
    Vector3 scale;
    Vector3 prevScale;
    public ScaleCommand(GameObject gameObject, Vector3 scale, Vector3 prevScale)
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
    override public void Redo()
    {
        gameObject.transform.localScale = scale;
    }
}