using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingScreenManager : MonoBehaviour
{
    public static LoadingScreenManager Instance;

    [SerializeField]
    public Sprite[] animatedImages;
    public Slider loadingBar;
    public Image mainImage;
    public Text levelNameText;
    public Text loadingInfoText;

    private GameObject loadScreenContainer;
    private const float MIN_TIME_TO_SHOW = 1f;
    // The reference to the current loading operation running in the background:
    private AsyncOperation currentLoadingOperation;
    private bool isLoadingForSceneChange;
    private bool isLoadingForDownload;

    private float timeElapsed;

    private float timeSinceSpriteChange = 0;
    private float spriteChangeInterval = 0.05f;
    private int currentSpriteIndex = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        loadScreenContainer = transform.GetChild(0).gameObject;
        Hide();
    }

    void Update()
    {
        if (isLoadingForDownload)
        {
            if (AsyncTaskReporter.TasksAreRunning() && timeElapsed <= MIN_TIME_TO_SHOW)
            {
                timeElapsed += Time.deltaTime;
                timeSinceSpriteChange += Time.deltaTime;

                AnimateLoadScreen();
            }
            else
            {
                Hide();
            }
        }

        if (isLoadingForSceneChange)
        {
            if (!currentLoadingOperation.isDone)
            {
                timeElapsed += Time.deltaTime;
                timeSinceSpriteChange += Time.deltaTime;

                AnimateLoadScreen();

                if (timeElapsed >= MIN_TIME_TO_SHOW && !AsyncTaskReporter.TasksAreRunning())
                {
                    // The loading screen has been showing for the minimum time required.
                    // Allow the loading operation to formally finish:
                    currentLoadingOperation.allowSceneActivation = true;
                    loadingInfoText.text = "Loading Map Data...";
                }
                else if (timeElapsed >= MIN_TIME_TO_SHOW && AsyncTaskReporter.TasksAreRunning())
                {
                    loadingInfoText.text = "Downloading Ghost Run Data...";
                }
            }
            else
            {
                Hide();
            }
        }
    }

    public void Show(AsyncOperation loadingOperation)
    {
        // Enable the loading screen:
        loadScreenContainer.SetActive(true);

        // Store the reference:
        currentLoadingOperation = loadingOperation;

        // Stop the loading operation from finishing, even if it technically did:
        currentLoadingOperation.allowSceneActivation = false;

        // Reset the time elapsed:
        timeElapsed = 0f;

        isLoadingForSceneChange = true;

        mainImage.sprite = animatedImages[0];

        string levelName = GameManager.GetCurrentLevel().levelName;


        if (string.IsNullOrEmpty(levelName))
        {
            levelNameText.text = "Main Menu";
        }
        else
        {
            levelNameText.text = levelName;
        }
    }

    public void ShowWhileDownloading()
    {
        // Enable the loading screen:
        loadScreenContainer.SetActive(true);

        // Reset the time elapsed:
        timeElapsed = 0f;

        isLoadingForDownload = true;

        mainImage.sprite = animatedImages[0];

        string levelName = GameManager.GetCurrentLevel().levelName;


        if (string.IsNullOrEmpty(levelName))
        {
            levelNameText.text = "Main Menu";
        }
        else
        {
            levelNameText.text = levelName;
        }

        loadingInfoText.text = "Downloading Ghost Run Data...";
    }

    public void Hide()
    {
        loadScreenContainer.SetActive(false);

        currentLoadingOperation = null;

        isLoadingForDownload = false;
        isLoadingForSceneChange = false;

        // In Awake(), this script is made first so the Instance doesn't exist on startup
        if (GameManager.Instance != null) {
            GameManager.Instance.isLoadingScene = false;
        }
    }

    private void AnimateLoadScreen()
    {
        float percentComplete = timeElapsed / MIN_TIME_TO_SHOW;

        loadingBar.value = percentComplete;

        if(timeSinceSpriteChange > spriteChangeInterval)
        {
            timeSinceSpriteChange = 0;

            if (currentSpriteIndex < animatedImages.Length - 1)
            {
                currentSpriteIndex++;
            }
            else
            {
                currentSpriteIndex = 0;
            }
            mainImage.sprite = animatedImages[currentSpriteIndex];
        }
        
    }
}
