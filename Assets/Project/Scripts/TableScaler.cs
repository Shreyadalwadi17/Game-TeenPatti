using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableScaler : MonoBehaviour
{
    public float xmultiplier;
    public float refxmultiplier;
    public float lessmutliplier;
    void Start()
    {
          float width = ScreenSize.GetScreenToWorldWidth;
        float height = ScreenSize.GetScreenToWorldHeight;
        //less than 2160 , than keep xmultipler to be 2  
        if(Screen.height <2160)
        {
            xmultiplier=lessmutliplier;
        }
        else
        {
            xmultiplier=refxmultiplier;
        }
        transform.localScale = Vector3.one * (width/height)/xmultiplier;
    }
}
