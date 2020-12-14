using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public static class StringExtensions 
{ 
    public static string InsertNewLines(this string self)
    {
        return self.Replace("<br>", "\n");
    }

    public static string InsertHotKeys(this string self, bool defaulting)
    {
        MatchCollection matches = new Regex(PlayerConstants.HotKeyPattern).Matches(self);
        if (matches.Count <= 0)
        {
            return self; 
        }
        var currentKeys = HotKeyManager.Instance.GetHotKeys(); 
        var defaultKeys = HotKeyManager.Instance.GetDefaultHotKeys();

        foreach (Match match in matches.Cast<Match>().Reverse())
        {
            string key = defaultKeys.FirstOrDefault(x => x.Value.ToString() == match.Value).Key;
            if (key != null && !currentKeys[key].ToString().Equals(match.Value, System.StringComparison.InvariantCultureIgnoreCase))
            {
                KeyCode replacementKey = defaulting ? defaultKeys[key] : currentKeys[key];
                self = self.Remove(match.Index, match.Length).Insert(match.Index, replacementKey.ToString());
            }
        }
        return self; 
    }

    public static string InsertSpecificHotKey(this string self, KeyCode oldKeyCode, KeyCode newKeyCode)
    {
        string pattern = PlayerConstants.HotKeyPattern.Replace("*", $"[{oldKeyCode}]");
        self = Regex.Replace(self, pattern, newKeyCode.ToString());
        Debug.Log("New pattern" + pattern);
        return self;
    }
}
