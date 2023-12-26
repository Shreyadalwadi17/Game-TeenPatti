using System.Collections;
using System.Collections.Generic;
using Game.BaseFramework;
using UnityEngine;
using System.Threading.Tasks;
using SimpleJSON;
using UnityEngine.SceneManagement;


public class GamePlayMenuUI : MonoBehaviour
{
    public GameObject menuPanel;
    public GameObject logoutPanel;
    public GameObject gameInfoPanel;


    public void OnMenubuttonClick()
    {
        StopGamePlay();
        menuPanel.SetActive(true);
    }
    public void OnMenuCloseButtonClick()
    {
        menuPanel.SetActive(false);
        StartGamePlay();
    }
    public void OnLogoutbuttonClick()
    {
        StopGamePlay();
        menuPanel.SetActive(false);
        logoutPanel.SetActive(true);
    }
    public void OnLogoutCloseButtonClick()
    {
        logoutPanel.SetActive(false);
        StartGamePlay();
    }
    public void OnLogoutNoButtonClick()
    {
        logoutPanel.SetActive(false);
        StartGamePlay();
    }
    public void OnLogoutYesButtonClick()
    {
        StopGamePlay();
        Debug.Log("Game Quit");
        logoutPanel.SetActive(false);
        JSONNode node = new JSONObject();
        node.Add("sEventName", SocketServerEventType.reqLeave.ToString());

        TableManager.instance.Players.Clear();
        foreach(Transform t in TableManager.instance.PlayersHolders)
        {
            for (int i = t.childCount - 1; i >= 0; i--)
            {
                Destroy(t.GetChild(i).gameObject);
            }
        }

        StartCoroutine(LoadScene());
    }

    public void OnGameInfobuttonClick()
    {
        StopGamePlay();
        menuPanel.SetActive(false);
        gameInfoPanel.SetActive(true);
    }
    public void OnGameInfoCloseButtonClick()
    {
        gameInfoPanel.SetActive(false);
        StartGamePlay();
    }


    public void StopGamePlay()
    {
        Time.timeScale = 1;
    }
    public void StartGamePlay()
    {
        Time.timeScale = 0;
    }
    IEnumerator LoadScene()
    {
        yield return new WaitForSeconds(2f);
        string currentSceneName = SceneManager.GetActiveScene().name;
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(1);
        asyncOperation.allowSceneActivation = false;

        while (!asyncOperation.isDone)
        {
            if (asyncOperation.progress >= 0.9f) 
            {
                SceneManager.UnloadSceneAsync(currentSceneName);
                asyncOperation.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}




