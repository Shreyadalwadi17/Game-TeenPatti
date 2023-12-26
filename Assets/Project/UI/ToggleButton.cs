using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleButton : MonoBehaviour
{
    public bool isOn;
    public GameObject on, off;

    public bool IsOn
    {
        get { return isOn; }
        set
        {
            isOn = value;
            UpdateButtons();
        }
    }

    public  void OnButtonClick()
    {
        //base.OnButtonClick();
        IsOn = !IsOn;
    }

    public void UpdateButtons()
    {
        on.SetActive(isOn);
        off.SetActive(!isOn);
    }
}
