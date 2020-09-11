﻿using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Level X", menuName = "ScriptableObjects/level")]
public class Level : ScriptableObject
{
    [Header("Set in Editor")]
    [SerializeField]
    public int levelBuildIndex;

    [SerializeField]
    public string levelName;

    [SerializeField]
    public float gravityMultiplier;

    [SerializeField]
    public string[] tutorialTexts;

    [SerializeField]
    public int numberOfCheckpoints;

    [SerializeField]
    public string filePath;

    [Header("Set in Game")]
    [SerializeField]
    public Collectible[] collectibles;

    [SerializeField]
    public bool isCompleted;

    [SerializeField]
    public float completionTime;

    [SerializeField]
    public Vector3[] ghostRunPositions;

    [SerializeField]
    public KeysPressed[] ghostRunKeys;

    [SerializeField]
    public Sprite previewSprite;
}