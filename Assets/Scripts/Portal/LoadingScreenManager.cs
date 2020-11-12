using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreenManager : MonoBehaviour
{
    public static LoadingScreenManager Instance;

    [SerializeField]
    public Sprite[] animatedImages;
    public Slider loadingBar;
    public Image mainImage;
    public Text levelNameText;

    private GameObject loadScreenContainer;
    private const float MIN_TIME_TO_SHOW = 3f;
    // The reference to the current loading operation running in the background:
    private AsyncOperation currentLoadingOperation;
    private bool isLoading;

    private float timeElapsed;

    private float timeSinceSpriteChange = 0;
    private float spriteChangeInterval = 0.05f;
    private int currentSpriteIndex = 0;

    private void Awake()
    {
        if (FindObjectsOfType(GetType()).Length > 1)
        {
            Destroy(gameObject);
        }

        if (LoadingScreenManager.Instance == null)
        {
            LoadingScreenManager.Instance = this;
        }
        else if (LoadingScreenManager.Instance == this)
        {
            Destroy(LoadingScreenManager.Instance.gameObject);
            LoadingScreenManager.Instance = this;
        }
        DontDestroyOnLoad(this.gameObject);

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

                if (timeElapsed >= MIN_TIME_TO_SHOW)
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
    }

    public void Hide()
    {
        loadScreenContainer.SetActive(false);

        currentLoadingOperation = null;

        isLoading = false;
    }

    private void AnimateLoadScreen()
    {
        float percentComplete = timeElapsed / MIN_TIME_TO_SHOW;

        loadingBar.value = percentComplete;

        string levelName = GameManager.GetCurrentLevel().name;

        if (string.IsNullOrEmpty(levelName))
        {
            levelNameText.text = "Main Menu";

        }

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
