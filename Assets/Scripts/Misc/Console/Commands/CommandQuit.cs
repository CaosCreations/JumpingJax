using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Oneleif.debugconsole
{
    [CreateAssetMenu(fileName = "New Quit Command", menuName = "Developer Console/Commands/Quit Command")]
    public class CommandQuit : ConsoleCommand
    {
        public override void Process(string[] args)
        {
            if (Application.isEditor)
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#endif
            }
            else
            {
                Application.Quit();
            }
        }
    }
}