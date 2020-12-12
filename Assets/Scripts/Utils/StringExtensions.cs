using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            str = str.Replace(kvp.Key, kvp.Value);
        }
        Debug.Log("New string: " + str); 
        return str; 
    }
}
