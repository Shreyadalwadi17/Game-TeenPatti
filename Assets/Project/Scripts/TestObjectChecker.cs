using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestObjectChecker : MonoBehaviour
{
    private void OnEnable()
    {
        Debug.Log("This Object is Active");
    }

    private void OnDisable()
    {
        Debug.Log("This Object is Disabled");
    }
}
