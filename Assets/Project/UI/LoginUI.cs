using BaseFramework;
using Game.BaseFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LoginUI : UIScreenView
{
    public TMP_InputField emailField;
    public TMP_InputField passwordField;

    public override void OnScreenShowCalled()
    {
        base.OnScreenShowCalled();
        Events.WebRequestCompleted += Events_WebRequestCompleted;
    }

    private void Events_WebRequestCompleted(API_TYPE type, string response)
    {
        switch (type)
        {
            case API_TYPE.API_login:
                Debug.Log(response);
                UIController.instance.ShowNextScreen(ScreenType.Mainmenu);
                break;
        }
        
       
    }

    public override void OnScreenHideCalled()
    {
        base.OnScreenHideCalled();
        Events.WebRequestCompleted -= Events_WebRequestCompleted;
    }

    public override void OnBack()
    {
        base.OnBack();
        OnSignUpClicked();
    }

    public void OnSignUpClicked()
    {
        UIController.instance.ShowNextScreen(ScreenType.Register);
    }

    public void onSignInClicked()
    {
        ServicesHandler.instance.CallLoginService(emailField.text.Trim(),passwordField.text.Trim());
    }

    [ContextMenu("TestID")]
    public void TestID()
    {
        emailField.text = "praharsh.k@yudiz.in";
    }
}
