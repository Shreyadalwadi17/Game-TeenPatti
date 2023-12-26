using System;
using System.Collections;
using System.Collections.Generic;
using BaseFramework;
using Newtonsoft.Json;
using SimpleJSON;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : IndestructibleSingleton<GameManager>
{
    public Data rootData;

    public string TableId;
    public int TotalPlayer;
    public string TableState;
    public string myPlayerId;
    

    private void Start()
    {
        Events.WebRequestCompleted += Events_WebRequestCompleted;
    }

    private void OnDestroy()
    {
        Events.WebRequestCompleted -= Events_WebRequestCompleted;
    }
    
    private void Events_WebRequestCompleted(API_TYPE type, string response)
    {
        switch (type)
        {
            case API_TYPE.API_viewProfile:
                Debug.Log("Profile" + response);
                JSONNode node1 = JSONNode.Parse(response);
                myPlayerId = node1["data"]["_id"];
                break;
            
            case API_TYPE.API_boardJoin:
                Debug.Log("GameManager" + response);

                JSONNode node = JSONNode.Parse(response);
                rootData = JsonUtility.FromJson<Data>(node["data"].ToString());
                TableId = rootData.iBoardId;
                TableState = rootData.eState;
                TotalPlayer = rootData.nTotalParticipant;
                StartCoroutine(LoadScene(2));
                break;
        }
    }
    
    IEnumerator LoadScene(int sceneNum)
    {
        yield return new WaitForSeconds(2f);
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneNum);
        asyncOperation.allowSceneActivation = false;
        while (!asyncOperation.isDone)
        {
            //Output the current progress
            //ProgressBar.value = asyncOperation.progress;

            if (asyncOperation.progress >= 0.9f)
            {
                yield return new WaitForSeconds(1f);
                // Hide();
                yield return new WaitForSeconds(1f);
                asyncOperation.allowSceneActivation = true;
            }

            yield return null;
        }
        AskToJoinTable();
    }
    #region Requests To Server and Callbacks

    public void AskToJoinTable()
    {
        if (SocketHandler.Instance.root.IsOpen)
        {
            print("<color=green> " + SocketServerEventType.reqJoinBoard.ToString() + "</color> ");

            SocketHandler.Instance.root.ExpectAcknowledgement<object>(OnAskJoinRoomCallback).
                Emit(SocketServerEventType.reqJoinBoard.ToString(),
                    new { iBoardId = TableId, isReconnect = SocketHandler.Instance.isReconnected });
            
        }
        else
        {
            Debug.Log("Trying to Connect");
            SocketHandler.Instance.root.Manager.Open();
        }
    }

    private void OnAskJoinRoomCallback(object data)
    {
        print("<color=purple> OnAskJoinRoomCallBack </color>");

        var jsonString = JsonConvert.SerializeObject(data);
        JSONNode json = JSONNode.Parse(jsonString);
        //Debug.Log(json);
        print("<color=purple> OnAskJoinRoomCallBack </color>" + json.ToString());
        string error = json["error"];
        string tabledata = json["oData"].ToString();
     
        if (String.IsNullOrEmpty(error))
        { 
            SocketHandler.Instance.AddTableListener(TableId);
        }
        Debug.LogError("here in ask room callback"+json["oData"].ToString());
        Debug.Log("Join Callback Data = " +json["oData"].ToString());
        TableManager.instance.tableDetails = JsonUtility.FromJson<TableDetails>(tabledata);
        Debug.LogError("the table manager player no "+   TableManager.instance.tableDetails.aParticipant.Length);
        
    }

    #endregion
}
[Serializable]
public class Data
{
    public string iBoardId;
    public string eState;
    public int nChips;
    public int nTotalParticipant;
}

[Serializable]
public class Table
{
    public Data data;
}