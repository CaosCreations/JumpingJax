using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

    public enum SelectionDirection
    {
        up, down, none
    }

    public enum FocusSelection 
    {
        Cache, AutoComplete
    }

public class DeveloperConsole : MonoBehaviour
{
    public static DeveloperConsole Instance { get; private set; }
    private static ReaderWriterLockSlim _readWriteLock = new ReaderWriterLockSlim();

    [SerializeField] public ConsoleCommand[] commands;

    [Header("UI Components")]
    [SerializeField] private GameObject consoleContainer;
    [SerializeField] private Text consoleText;
    [SerializeField] private InputField consoleInput;
    [SerializeField] private ScrollRect logScrollView;
    private PauseMenu pauseMenu;

    [Header("Debugging values")]
    public bool consoleIsActive = false;

    // Set at runtime
    private AutoComplete autoComplete;

    private List<string> cachedCommands;
    private int currentCacheIndex;

    private FocusSelection focusSelection;
    [SerializeField]
    private bool isEnabledInOptions;

    // Outputs to: C:\Users\<your-user>\AppData\LocalLow\DefaultCompany\UnityDebugConsole\log.txt
    private string logFilePath;

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
        Init();
    }

    private void Init()
    {
        if (ConsoleConstants.shouldOutputDebugLogs)
        {
            Application.logMessageReceived += HandleLog;
        }
        logFilePath = Path.Combine(Application.persistentDataPath, ConsoleConstants.fileLoggerFileName);

        Debug.Log("Initializing Dev Console");
        consoleContainer.SetActive(false);
        autoComplete = GetComponentInChildren<AutoComplete>();
        cachedCommands = new List<string>();
        focusSelection = FocusSelection.Cache;
        pauseMenu = GameObject.FindObjectOfType<PauseMenu>();
        isEnabledInOptions = OptionsPreferencesManager.GetConsoleToggle();

        MiscOptions.onConsoleToggle += ToggleConsoleEnabled;


        // Delete large log files
        //if (File.Exists(logFilePath))
        //{
        //    FileInfo fileInfo = new FileInfo(logFilePath);
        //    if (fileInfo.Length > ConsoleConstants.MaxLogSizeInBytes)
        //    {
        //        try
        //        {
        //            File.Delete(logFilePath);
        //        }catch(Exception e)
        //        {
        //            Debug.LogError($"DeveloperConsole couldn't delete file: {e.Message}\n{e.StackTrace}");
        //        }
        //    }
        //}
    }

    public void ToggleConsoleEnabled(bool isEnabled)
    {
        Debug.Log("enabled console " + isEnabled);
        OptionsPreferencesManager.SetConsoleToggle(isEnabled);
        isEnabledInOptions = isEnabled;
    }

    private void HandleLog(string logMessage, string stackTrace, LogType type)
    {
        string color = "white";
        switch (type)
        {
            case LogType.Error:
                color = "#f75757";
                break;

            case LogType.Warning:
                color = "#f0e881";
                break;

            case LogType.Log:
                color = "#a8e3b4";
                break;
            
        }

        LogMessage("<color=" + color + ">[" + type.ToString() + "]" + logMessage + "</color> ");
        ClearOldLogs();
        MoveScrollViewToBottom();
    }

    private void Update()
    {
        if (GameManager.Instance.didWinCurrentLevel || pauseMenu.isPaused || !isEnabledInOptions)
        {
            return;
        }

        if (Input.GetKeyDown(ConsoleConstants.toggleKey))
        {
            ToggleConsole();
        }

        if (!consoleIsActive)
        {
            return;
        }

        HandleFocusInteraction();
    }

    private void HandleFocusInteraction()
    {
        // Change focus selection
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (focusSelection == FocusSelection.AutoComplete)
            {
                autoComplete.UnhighlightSelection();
                focusSelection = FocusSelection.Cache;
            }
            else
            {
                // Don't change focus if the menu isn't open
                if (autoComplete.HasItems())
                {
                    autoComplete.HighlightSelection();
                    focusSelection = FocusSelection.AutoComplete;
                }
            }
        }

        SelectionDirection inputSelectionDirection = GetInputSelectionDirection();

        // Perform interaction on focused element
        if (inputSelectionDirection != SelectionDirection.none)
        {
            if (focusSelection == FocusSelection.AutoComplete)
            {
                if (autoComplete.HasItems())
                {
                    consoleInput.onValueChanged.RemoveAllListeners();

                    autoComplete.SelectResult(inputSelectionDirection);
                    consoleInput.text = autoComplete.GetAutoCompleteCommand();
                    consoleInput.MoveTextEnd(false);

                    consoleInput.onValueChanged.AddListener(delegate
                    { ShowCommandAutoComplete(consoleInput); });
                }
            }
            else
            {
                MoveCache(inputSelectionDirection);
            }
        }
    }

    private void ClearOldLogs()
    {
        // If there is more than x lines in the text, delete the oldest
        string logs = consoleText.text;
        int lines = Regex.Matches(logs, "\n").Count;
        if (lines > ConsoleConstants.MaxConsoleLines)
        {
            consoleText.text = logs.Remove(0, logs.IndexOf('\n') + 1);
        }
    }

    private SelectionDirection GetInputSelectionDirection()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            return SelectionDirection.up;
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            return SelectionDirection.down;
        }

        return SelectionDirection.none;
    }

    private void ToggleConsole()
    {
        consoleIsActive = !consoleIsActive;
        consoleContainer.SetActive(consoleIsActive);
        if (consoleIsActive)
        {
            currentCacheIndex = cachedCommands.Count;
            SetupInputField();
            MoveScrollViewToBottom();
        }
    }

    private void LogMessage(string message)
    {
        if(consoleText != null)
        {
            consoleText.text += message + "\n";
        }

        DateTime now = DateTime.Now;
        message = string.Format("[{0:H:mm:ss}] {1}\n", now, message);

        _readWriteLock.EnterWriteLock();
        try
        {
            // Append text to the file
            using (StreamWriter sw = File.AppendText(logFilePath))
            {
                sw.WriteLine(message);
                sw.Close();
            }
        }
        finally
        {
            // Release lock
            _readWriteLock.ExitWriteLock();
        }
    }

    private void SetupInputField()
    {
        consoleInput.onEndEdit.RemoveAllListeners();
        consoleInput.onValueChanged.RemoveAllListeners();

        ClearInputField(consoleInput);

        consoleInput.onEndEdit.AddListener(delegate
        { ProcessCommand(consoleInput); });

        consoleInput.onValueChanged.AddListener(delegate
        { ShowCommandAutoComplete(consoleInput); });
    }

    private void ClearInputField(InputField consoleInput)
    {
        consoleInput.text = string.Empty;
        consoleInput.Select();
        consoleInput.ActivateInputField();
    }

    public void ShowCommandAutoComplete(InputField consoleInput)
    {
        if (consoleInput.text.StartsWith("`"))
        {
            ClearInputField(consoleInput);
        }
        autoComplete.FillResults(consoleInput);
    }

    public void ProcessCommand(InputField consoleInput)
    {
        if(consoleInput.text == "`" || string.IsNullOrEmpty(consoleInput.text))
        {
            return;
        }

        (ConsoleCommand command, string[] args) = GetCommandFromInput(consoleInput.text);
        LogMessage(ConsoleConstants.commandPrefix + consoleInput.text);
        if (command != null)
        {
            command.Process(args);
        }
        else
        {
            Debug.LogWarning("Command not recognized");
        }

        // Add command to cache and reset the field
        cachedCommands.Add(consoleInput.text);
        currentCacheIndex = cachedCommands.Count;
        ClearInputField(consoleInput);
        focusSelection = FocusSelection.Cache;
    }

    private (ConsoleCommand, string[]) GetCommandFromInput(string input)
    {
        // Split command from arguments
        string[] inputSplit = input.Split(' ');

        string commandInput = inputSplit[0];
        string[] commandArguments = inputSplit.Skip(1).ToArray();

        ConsoleCommand command = GetValidCommand(commandInput);
        return (command, commandArguments);
    }

    public ConsoleCommand GetValidCommand(string inputCommand)
    {
        foreach (var command in Instance.commands)
        {
            if(command.Command == inputCommand)
            {
                return command;
            }
        }

        return null;
    }

    private void MoveCache(SelectionDirection direction)
    {
        if(cachedCommands.Count > 0)
        {
            if (direction == SelectionDirection.up)
            {
                if (currentCacheIndex > 0)
                {
                    currentCacheIndex--;
                    consoleInput.text = cachedCommands[currentCacheIndex];
                }
                else if(currentCacheIndex == 0)
                {
                    consoleInput.text = cachedCommands[currentCacheIndex];
                }
            }
            else if (direction == SelectionDirection.down)
            {
                if (currentCacheIndex < cachedCommands.Count - 1)
                {
                    currentCacheIndex++;
                    consoleInput.text = cachedCommands[currentCacheIndex];
                }
                else
                {
                    consoleInput.text = string.Empty;
                }
            }
        }

        consoleInput.ActivateInputField();
        consoleInput.Select();
        consoleInput.MoveTextEnd(false);
    }

    private void MoveScrollViewToBottom()
    {
        if (consoleIsActive)
        {
            logScrollView.verticalNormalizedPosition = 0;
        }
    }
}
