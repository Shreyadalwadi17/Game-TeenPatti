using System;
using System.Collections;
using System.Collections.Generic;
using BaseFramework;
using Game.BaseFramework;
using TMPro;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    public GameObject btnHolder;
    [Header("Player Buttons")] 
    public GameObject callBtn;
    public GameObject blindBtn;
    public GameObject blindRaiseBtn;
    public GameObject raiseBtn;
    public GameObject foldBtn;
    public GameObject showBtn;
   
    private void Start()
    {
       
        Events.WebRequestCompleted += Events_WebRequestCompleted;
        HideButtonsToPlayer();
    }

    private void OnDestroy()
    {
        Events.WebRequestCompleted -= Events_WebRequestCompleted;
    }
    
    
    private void Events_WebRequestCompleted(API_TYPE type, string response)
    {
        switch (type)
        {
            case API_TYPE.None:
                Debug.Log(response);
                break;
        }
    }
    
    public void OnCallBtnClicked()
    {
        Debug.Log("buttonClicked");
        TableManager.instance.CallByCurrentPlayer();
    }
    public void OnRaiseBtnClicked()
    {
        Debug.Log("buttonClicked");
        TableManager.instance.RaiseByCurrentPlayer();
    }
    public void OnShowBtnClicked()
    {
        Debug.Log("buttonClicked");
        TableManager.instance.ShowByCurrentPlayer();
    }
    public void OnFoldBtnClicked()
    {
        Debug.Log("buttonClicked");
        TableManager.instance.PackByCurrentPlayer();
    }
    public void OnBlindBtnClicked()
    {
        Debug.Log("buttonClicked");
        TableManager.instance.BlindByCurrentPlayer();
    }
    public void OnBlindRaiseBtnClicked()
    {
        Debug.Log("buttonClicked");
        TableManager.instance.BlindRaiseByCurrentPlayer();
    }
    

    public void ShowButtonsToPlayer()
    {
        Debug.LogError("the button holder parent name "+btnHolder.transform.gameObject.name);   
        btnHolder.gameObject.SetActive(true);
        if(btnHolder.activeInHierarchy==true)
        {
            Debug.LogError("in active in hierrechy");
        }
    }
    public void HideButtonsToPlayer()
    {
        btnHolder.SetActive(false);
    }
}
