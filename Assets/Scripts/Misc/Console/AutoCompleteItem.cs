using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Oneleif.debugconsole
{
    public class AutoCompleteItem : MonoBehaviour
    {
        [SerializeField] private Text commandText;
        [SerializeField] private Text commandDescriptionText;

        private ConsoleCommand consoleCommand;
        public ConsoleCommand ConsoleCommand {
            get
            {
                return consoleCommand;
            }
            set
            {
                consoleCommand = value;
                commandText.text = value.Command;
                commandDescriptionText.text = value.Description;
            }
        }

        public void Highlight()
        {
            commandText.color = new Color(0, 0, 1, 1);
            commandDescriptionText.color = new Color(0, 0, 1, 1);
        }

        public void Unhighlight()
        {
            commandText.color = new Color(0, 0, 0, 1);
            commandDescriptionText.color = new Color(0, 0, 0, 1);
        }
    }
}
