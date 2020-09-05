using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Oneleif.debugconsole
{
    [Serializable]
    public abstract class ConsoleCommand : ScriptableObject, IConsoleCommand
    {
        [SerializeField] private string command = string.Empty;
        [SerializeField] private string description = string.Empty;

        [SerializeField] public string Command => command;
        [SerializeField] public string Description => description;

        public abstract void Process(string[] args);
    }
}