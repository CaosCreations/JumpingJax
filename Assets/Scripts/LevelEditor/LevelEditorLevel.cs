using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LevelEditorLevel
{
    [SerializeField]
    public List<ObjectData> levelObjects;

    public LevelEditorLevel()
    {
        levelObjects = new List<ObjectData>();
    }
}
