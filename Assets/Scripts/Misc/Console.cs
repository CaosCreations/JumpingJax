using System;
using System.Collections.Generic;
using UnityEngine;

public class Console : MonoBehaviour
{
    public static Console Instance { get; private set; }

    public KeyCode toggleKey = KeyCode.BackQuote;

    public bool openOnStart = false;

    private void Awake()
    {
        if (FindObjectsOfType(GetType()).Length > 1)
        {
            Destroy(gameObject);
        }

        if (Console.Instance == null)
        {
            Console.Instance = this;
        }
        else if (Console.Instance == this)
        {
            Destroy(Console.Instance.gameObject);
            Console.Instance = this;
        }
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
    }

    private void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
        }
    }
}
