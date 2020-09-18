using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeUtils
{
    public static string GetTimeString(float completionTime)
    {
        TimeSpan time = TimeSpan.FromSeconds(completionTime);
        return time.ToString(PlayerConstants.levelCompletionTimeFormat);
    }
}
