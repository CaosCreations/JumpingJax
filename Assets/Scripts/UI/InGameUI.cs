using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameUI : MonoBehaviour
{
    public Text completionTimeText;
    //public Text fpsText;
    public SpeedSlider speed;
    public Text tutorialText;
    public Text tutorialNextText;
    public GameObject tutorialPane;
    public PlayerMovement playerMovement;

    private string[] tutorialTexts;
    private int tutorialTextIndex = 0;

    private void Start()
    {
        playerMovement = GetComponentInParent<PlayerMovement>();
        speed = GetComponentInChildren<SpeedSlider>();

        tutorialTexts = GameManager.GetCurrentLevel().tutorialTexts;
        LoadNextTutorial();
    }

    void Update()
    {
        if(GameManager.Instance != null)
        {
            TimeSpan time = TimeSpan.FromSeconds(GameManager.Instance.currentCompletionTime);
            completionTimeText.text = time.ToString("hh':'mm':'ss");
        }

        //fpsText.text = "FPS: " + Mathf.Round(1 / Time.deltaTime);
        Vector2 directionalSpeed = new Vector2(playerMovement.newVelocity.x, playerMovement.newVelocity.z);
        speed.SetSpeed(directionalSpeed.magnitude);

        if (Input.GetKeyDown(PlayerConstants.NextTutorial))
        {
            LoadNextTutorial();
        }

    }

    private void LoadNextTutorial()
    {
        if(tutorialTextIndex < tutorialTexts.Length)
        {
            tutorialPane.SetActive(true);
            tutorialText.text = tutorialTexts[tutorialTextIndex].Replace("<br>", "\n");
            Invoke("UpdateParentLayoutGroup", 0.1f);
            tutorialTextIndex++;
        }
        else
        {
            tutorialPane.SetActive(false);
        }
    }

    void UpdateParentLayoutGroup()
    {
        tutorialText.gameObject.SetActive(false);
        tutorialText.gameObject.SetActive(true);

        tutorialNextText.gameObject.SetActive(false);
        tutorialNextText.gameObject.SetActive(true);
    }
}
