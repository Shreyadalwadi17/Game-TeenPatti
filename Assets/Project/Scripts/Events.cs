using BaseFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Events
{
    public static event Action<API_TYPE, string> WebRequestCompleted = delegate { };

    public static void OnWebRequestComplete(API_TYPE Type, string Data)
    {
        WebRequestCompleted?.Invoke(Type, Data);
    }
}
