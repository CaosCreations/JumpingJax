using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelEditorButton : MonoBehaviour
{
    public Level level;

    public Button button;
    public Text text;

    private void Awake()
    {
        button = GetComponent<Button>();
        text = GetComponentInChildren<Text>();
    }

    public void Init(Level level)
    {
        text.text = level.levelName;
        this.level = level;

    }
}
