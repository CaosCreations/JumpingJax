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

    private GameObject loadScreenContainer;
    private const float MIN_TIME_TO_SHOW = 2f;
    // The reference to the current loading operation running in the background:
    private AsyncOperation currentLoadingOperation;
    private bool isLoading;

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
        if (isLoading)
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

        isLoading = true;

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

    public void Hide()
    {
        loadScreenContainer.SetActive(false);

        currentLoadingOperation = null;

        isLoading = false;

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
