using BaseFramework;
using Game.BaseFramework;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RegisterUI : UIScreenView
{
    public TMP_InputField emailField;
    public TMP_InputField passwordField;
    public TMP_InputField usernameField;


    public override void OnScreenShowCalled()
    {
        base.OnScreenShowCalled();
        Events.WebRequestCompleted += Events_WebRequestCompleted;
    }

    public override void OnScreenHideCalled()
    {
        base.OnScreenHideCalled();
        Events.WebRequestCompleted -= Events_WebRequestCompleted;

    }
    private void Events_WebRequestCompleted(API_TYPE type, string response)
    {
        switch (type)
        {
            case API_TYPE.API_register:
                Debug.Log(response);
                OnSignInClicked();
                break;
        }
    }


    public override void OnBack()
    {
        base.OnBack();
        OnSignInClicked();
    }

    public void OnSignUpClicked()
    {
        ServicesHandler.instance.CallRegisterService(usernameField.text.Trim(), passwordField.text.Trim(),emailField.text.Trim());
    }

    public void OnSignInClicked()
    {
        UIController.instance.ShowNextScreen(ScreenType.Login);
    }
}
