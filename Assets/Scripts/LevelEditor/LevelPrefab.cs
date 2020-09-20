using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "X Prefab", menuName = "ScriptableObjects/levelPrefabs")]
public class LevelPrefab : ScriptableObject
{
    [SerializeField]
    public string objectName;

    [SerializeField]
    public GameObject prefab;

    [SerializeField]
    public Sprite previewImage;

    [SerializeField]
    public ObjectType objectType;
}
