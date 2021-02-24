using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelPreview : MonoBehaviour
{
    public Button playButton;
    public Button backButton;

    public Image previewImage;
    public Sprite noImageSprite;

    public Text levelNameText;
    public Text bestTimeText;
    public Text rankText;

    public Image singleBoneImage;
    public Image doubleBoneImage;
    public Image tripleBoneImage;

    public Sprite boneEmptySprite;
    public Sprite boneFilledSprite;

    public Text singleBoneText;
    public Text doubleBoneText;
    public Text tripleBoneText;

    private Level levelToPreview;
    
    public LeaderboardManager leaderboardManager;

    private void Awake()
    {
        leaderboardManager = GetComponentInChildren<LeaderboardManager>();
    }

    private void Start()
    {
        playButton.onClick.RemoveAllListeners();
        playButton.onClick.AddListener(Play);
        backButton.onClick.RemoveAllListeners();
        backButton.onClick.AddListener(Back);
    }

    void Play()
    {
        // If this is a workshop map
        if(levelToPreview.workshopFilePath != string.Empty) 
        {
            GameManager.LoadScene(levelToPreview);
        }
        else
        {
            GameManager.LoadScene(levelToPreview.levelBuildIndex);
        }

        leaderboardManager.SetReplayLocation();

        if (leaderboardManager.replayFileId != 0)
        {
            OptionsPreferencesManager.SetLeaderboardGhostTooltip(false);
        }
    }

    void Back()
    {
        gameObject.SetActive(false);
    }

    public async void Init(Level level)
    {
        levelToPreview = level;

        UpdateDetailPane();

        await leaderboardManager.InitAsync(levelToPreview.levelName);
        rankText.text = leaderboardManager.currentRank;
    }

    private void UpdateDetailPane()
    {
        levelNameText.text = levelToPreview.levelName;

        // We need this check in case the workshop dev forgot to upload a screen shot
        if (levelToPreview.previewSprite == null)
        {
            previewImage.sprite = noImageSprite;
        }
        else
        {
            previewImage.sprite = levelToPreview.previewSprite;
        }
        previewImage.preserveAspect = true;


        if (levelToPreview.levelSaveData.isCompleted)
        {
            TimeSpan time = TimeSpan.FromSeconds(levelToPreview.levelSaveData.completionTime);
            string timeString = time.ToString(PlayerConstants.levelCompletionTimeFormat);
            bestTimeText.text = timeString;
        }
        else
        {
            bestTimeText.text = "N/A";
        }

        SetBoneImages();
        singleBoneText.text = TimeSpan.FromSeconds(levelToPreview.bone1Time).ToString(PlayerConstants.levelCompletionTimeFormat);
        doubleBoneText.text = TimeSpan.FromSeconds(levelToPreview.bone2Time).ToString(PlayerConstants.levelCompletionTimeFormat);
        tripleBoneText.text = TimeSpan.FromSeconds(levelToPreview.bone3Time).ToString(PlayerConstants.levelCompletionTimeFormat);
    }

    void SetBoneImages()
    {
        int numberOfBones = levelToPreview.GetNumberOfTimeBones();

        singleBoneImage.sprite = boneEmptySprite;
        doubleBoneImage.sprite = boneEmptySprite;
        tripleBoneImage.sprite = boneEmptySprite;

        if (numberOfBones >= 1)
        {
            singleBoneImage.sprite = boneFilledSprite;
        }

        if (numberOfBones >= 2)
        {
            doubleBoneImage.sprite = boneFilledSprite;
        }

        if (numberOfBones == 3)
        {
            tripleBoneImage.sprite = boneFilledSprite;
        }
    }
}
