using System.Collections.Generic;
using UnityEngine;

public class HotKeyUtils : MonoBehaviour
{
    public static Dictionary<string, string> GetHotKeyMismatches()
    {
        Dictionary<string, string> mismatches = new Dictionary<string, string>();
        Dictionary<string, KeyCode> defaults = HotKeyManager.Instance.GetDefaultHotKeys();

        foreach (KeyValuePair<string, KeyCode> kvp in HotKeyManager.Instance.GetHotKeys())
        {
            if (kvp.Value != defaults[kvp.Key])
            {
                mismatches.Add(defaults[kvp.Key].ToString(), kvp.Value.ToString());
            }
        }
        return mismatches;
    }
}
