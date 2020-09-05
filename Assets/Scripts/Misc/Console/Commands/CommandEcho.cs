using UnityEngine;

namespace Oneleif.debugconsole
{
    [CreateAssetMenu(fileName = "New Echo Command", menuName = "Developer Console/Commands/Echo Command")]
    public class CommandEcho : ConsoleCommand
    {
        public override void Process(string[] args)
        {
            Debug.Log("Echoing: " + string.Join(" ", args));
        }
    }
}
