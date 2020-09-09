using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelPreview : MonoBehaviour
{
    public GameObject leaderboardItemPrefab;

    public Button playButton;
    public Image previewImage;
    public Transform leaderboard;
    public Transform scrollViewContent;


    private Level levelToPreview;

    private void Start()
    {
        playButton.onClick.RemoveAllListeners();
        playButton.onClick.AddListener(Play);
        playButton.gameObject.SetActive(false);
        previewImage.gameObject.SetActive(false);
        leaderboard.gameObject.SetActive(false);
    }

    void Play()
    {
        GameManager.Instance.currentLevelBuildIndex = levelToPreview.levelBuildIndex;
        SceneManager.LoadScene(levelToPreview.levelBuildIndex);
    }

    public void Init(Level level)
    {
        playButton.gameObject.SetActive(true);
        previewImage.gameObject.SetActive(true);
        leaderboard.gameObject.SetActive(true);
        levelToPreview = level;
        previewImage.sprite = level.previewSprite;

        // make HTTP request from steam for leaderboard data
    }

}
