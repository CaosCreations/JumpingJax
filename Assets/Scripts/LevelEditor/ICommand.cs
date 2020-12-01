using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICommand
{
    void Execute();
    void Undo();
    CommandNames CommandName();
    Vector3 Position();
    Vector3 PrevPosition();
    Quaternion Rotation();
    Quaternion PrevRotation();
    Vector3 Scale();
    Vector3 PrevScale();
    GameObject GetGameObject();
}
