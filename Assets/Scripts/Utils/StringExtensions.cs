using System.Linq;
using System.Text.RegularExpressions;

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
            if (currentKeys[key].ToString() != match.Value)
            {
                self = self.Remove(match.Index, match.Length).Insert(match.Index, currentKeys[key].ToString());
            }
        }
        return self; 
    }
}
