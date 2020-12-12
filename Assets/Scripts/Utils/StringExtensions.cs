using System.Collections.Generic;

public static class StringExtensions 
{ 
    public static string InsertNewLines(this string str)
    {
        return str.Replace("<br>", "\n");
    }

    public static string InsertCustomHotKeys(this string str)
    {
        foreach (KeyValuePair<string, string> kvp in HotKeyUtils.GetHotKeyMismatches())
        {
            str = str.Replace($"[{kvp.Key}]", $"[{kvp.Value}]");
        }
        return str; 
    }
}
