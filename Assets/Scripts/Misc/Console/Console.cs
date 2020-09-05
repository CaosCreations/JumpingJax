using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Console : MonoBehaviour
{
    public static Console Instance { get; private set; }

    public KeyCode toggleKey = KeyCode.BackQuote;

    public GameObject container;
    public Transform scrollViewContent;
    public InputField inputField;

    private void Awake()
    {
        //if (FindObjectsOfType(GetType()).Length > 1)
        //{
        //    Destroy(gameObject);
        //}

        //if (Console.Instance == null)
        //{
        //    Console.Instance = this;
        //}
        //else if (Console.Instance == this)
        //{
        //    Destroy(Console.Instance.gameObject);
        //    Console.Instance = this;
        //}
        //DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        container.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            ToggleDevConsole();
        }
    }

    private void ToggleDevConsole()
    {
        container.SetActive(!container.activeSelf);
        if (container.activeSelf)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
            Time.timeScale = 0;
            inputField.Select();
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            Time.timeScale = 1;
        }
    }
}
