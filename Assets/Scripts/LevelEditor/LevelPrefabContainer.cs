using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelPrefabContainer", menuName = "ScriptableObjects/levelPrefabContainer")]
public class LevelPrefabContainer : ScriptableObject
{
    [SerializeField]
    public LevelPrefab[] levelPrefabs;
}
