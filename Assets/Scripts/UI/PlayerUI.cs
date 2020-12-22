using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class PlayerUI : MonoBehaviour {

    [SerializeField]
    GameObject inGameUI = null;

    [SerializeField]
    GameObject winMenu = null;

    void Start () {
        inGameUI.SetActive(true);
        winMenu.gameObject.SetActive(false);
    }
	
    public void ShowWinScreen()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        inGameUI.SetActive(false);
        winMenu.gameObject.SetActive(true);
    }

    public void ToggleOffWinScreen()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
        inGameUI.SetActive(true);
        winMenu.gameObject.SetActive(false);
    }

    
}
