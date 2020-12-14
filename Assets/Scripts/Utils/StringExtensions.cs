using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public static class StringExtensions 
{ 
    public static string InsertNewLines(this string self)
    {
        return self.Replace("<br>", "\n");
    }

    public static string InsertCustomHotKeys(this string self)
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
                self = self.Remove(match.Index, match.Length).Insert(match.Index, currentKeys[key].ToString());
            }
        }
        return self; 
    }

    public static string InsertSpecificHotKey(this string self, KeyCode oldKeyCode, KeyCode newKeyCode)
    {
        string pattern = PlayerConstants.HotKeyPattern.Replace("*", oldKeyCode.ToString());
        return Regex.Replace(self, pattern, newKeyCode.ToString());
    }
}
