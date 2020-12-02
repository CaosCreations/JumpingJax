using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public interface ICommand
{
    void Undo();
    void Redo();
    CommandNames CommandName();
    GameObject GetGameObject();
}