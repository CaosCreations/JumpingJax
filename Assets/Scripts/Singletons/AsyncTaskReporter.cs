using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsyncTaskReporter : MonoBehaviour
{
    public static AsyncTaskReporter Instance { get; private set; }

    public bool ghostDownloadRunning;

    void Awake()
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
    }

    public static bool TasksAreRunning()
    {
        return Instance.ghostDownloadRunning;
    }
}
