using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Oneleif.debugconsole
{
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
        [SerializeField] public ConsoleCommand[] commands;

        [Header("UI Components")]
        [SerializeField] private GameObject consoleContainer;
        [SerializeField] private Text consoleText;
        [SerializeField] private InputField consoleInput;

        private bool consoleIsActive = false;

        private AutoComplete autoComplete;
        private FileLogger fileLogger;

        private List<string> cachedCommands;
        private int currentCacheIndex;

        private FocusSelection focusSelection;


        #region Singleton

        public static DeveloperConsole Instance { get; private set; }
        private void Awake()
        {
            if (FindObjectsOfType(GetType()).Length > 1)
            {
                Destroy(gameObject);
            }

            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance == this)
            {
                Destroy(Instance.gameObject);
                Instance = this;
            }
            DontDestroyOnLoad(this.gameObject);
        }

        #endregion Singleton

        private void Start()
        {
            consoleContainer.SetActive(false);
            autoComplete = GetComponentInChildren<AutoComplete>();
            fileLogger = GetComponent<FileLogger>();
            cachedCommands = new List<string>();
            focusSelection = FocusSelection.Cache;
        }

        private void OnEnable()
        {
            if (ConsoleConstants.shouldOutputDebugLogs)
            {
                Application.logMessageReceived += HandleLog;
            }
        }

        private void OnDisable()
        {
            if (ConsoleConstants.shouldOutputDebugLogs)
            {
                Application.logMessageReceived -= HandleLog;
            }
        }

        private void HandleLog(string logMessage, string stackTrace, LogType type)
        {
            string color = "";
            switch (type)
            {
                case LogType.Error:
                    color = "red";
                    break;

                case LogType.Warning:
                    color = "yellow";
                    break;

                case LogType.Log:
                    color = "blue";
                    break;
            }

            LogMessage("<color=" + color + ">[" + type.ToString() + "]" + logMessage + "</color> ");
        }

        private void Update()
        {
            if (GameManager.Instance.didWinCurrentLevel)
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

                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.Confined;
                Time.timeScale = 0;
            }
            else
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                Time.timeScale = 1;
            }
        }

        private void LogMessage(string message)
        {
            consoleText.text += message + "\n";
            if(fileLogger != null)
            {
                fileLogger.LogToFile(message);
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

            // Move scroll view to the bottom

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
            foreach (var command in commands)
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
    }
}