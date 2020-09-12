using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingScreenManager : MonoBehaviour
{
    public static LoadingScreenManager Instance;

    private const float MIN_TIME_TO_SHOW = 1f;
    // The reference to the current loading operation running in the background:
    private AsyncOperation currentLoadingOperation;
    private bool isLoading;

    private float timeElapsed;

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

        Hide();
    }
    void Start()
    {
        
    }

    void Update()
    {
        if (isLoading)
        {
            if (!currentLoadingOperation.isDone)
            {
                timeElapsed += Time.deltaTime;

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
        gameObject.SetActive(true);

        // Store the reference:
        currentLoadingOperation = loadingOperation;

        // Stop the loading operation from finishing, even if it technically did:
        currentLoadingOperation.allowSceneActivation = false;

        // Reset the time elapsed:
        timeElapsed = 0f;

        isLoading = true;
    }

    public void Hide()
    {
        gameObject.SetActive(false);

        currentLoadingOperation = null;

        isLoading = false;
    }

}
