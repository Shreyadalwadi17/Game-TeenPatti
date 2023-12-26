using System.Collections;
using System.Collections.Generic;
using BaseFramework;
using Game.BaseFramework;
using SimpleJSON;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

public class MainMenuUI : UIScreenView
{
    [SerializeField] private Button playBtn;
    public override void OnScreenShowCalled()
    {
        base.OnScreenShowCalled();
        Events.WebRequestCompleted += Events_WebRequestCompleted;
        SocketHandler.Instance.CreateSocket();
        
        ServicesHandler.instance.CallViewProfileService();
        playBtn.onClick.AddListener(StartGame);
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
            case API_TYPE.None:
                Debug.Log(response);
                break;
            
        }
    }

    private void StartGame()
    {
        UIController.instance.ShowNextScreen(ScreenType.Tableselection);
    }
    
}
