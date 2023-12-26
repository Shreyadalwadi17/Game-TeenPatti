using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scaler : MonoBehaviour
{
    public float xmultiplier;
    void Start()
    {
        float width = ScreenSize.GetScreenToWorldWidth;
        float height = ScreenSize.GetScreenToWorldHeight; 
        
        transform.localScale = Vector3.one * (width/height)/xmultiplier;
    }
}
