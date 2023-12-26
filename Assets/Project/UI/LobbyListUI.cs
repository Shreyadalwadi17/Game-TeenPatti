using System;
using System.Collections;
using System.Collections.Generic;
using BaseFramework;
using Game.BaseFramework;
using Newtonsoft.Json;
using SimpleJSON;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyListUI : UIScreenView
{
    
    public List<TableInfo> tableInfos;
    public TableData tableData;
    string str = "{\"2\":[{\"eBoardType\":\"cash\",\"nBoardFee\":100,\"aWinningAmount\":[90],\"iProtoId\":\"642c0ad040113789f545f971\"}]}";
    [SerializeField] private TableListElementUI ElementPrefab;
    [SerializeField] private Transform parentOfElement;
   
    public override void OnScreenShowCalled()
    {
        base.OnScreenShowCalled();
        CallProtoList();
        Events.WebRequestCompleted += Events_WebRequestCompleted;
    }

    
    public override void OnScreenHideCalled()
    {
        base.OnScreenHideCalled();
        Events.WebRequestCompleted -= Events_WebRequestCompleted;
    }
    public void CallProtoList()
    {
        ServicesHandler.instance.CallGetProtoList();
    }

    private void Events_WebRequestCompleted(API_TYPE type, string response)
    {
        switch (type)
        {
            
            case  API_TYPE.API_getProtoList:
                Debug.Log(response);
                JSONNode node = JSONNode.Parse(response);
                
                tableData = JsonConvert.DeserializeObject<TableData>(response);
                foreach (Transform element in parentOfElement)
                {
                    Destroy(element.gameObject);
                }
                foreach (KeyValuePair<int, List<TableInfo>> entry in tableData.Data)
                {
                    
                    int tableId = entry.Key;
                    List<TableInfo> tableList = entry.Value;

                    foreach (TableInfo table in tableList)
                    {
                        TableListElementUI lobbyElement = Instantiate(ElementPrefab, parentOfElement);
                        lobbyElement.MaxPlayerLbl.text = tableId.ToString();
                        lobbyElement.tableTypeLbl.text = table.eBoardType.ToString();
                        lobbyElement.JoiningFeeLbl.text = table.nBoardFee.ToString()+" Rs";
                        lobbyElement.WiningAmountLbl.text = table.aWinningAmount[0].ToString()+" Rs";
                        lobbyElement.protoId = table.iProtoId;
                    }
                }
                break;
        }
    }
}

[Serializable]
public class TableInfo
{
    public string eBoardType;
    public int nBoardFee;
    public int[] aWinningAmount;
    public string iProtoId;
}

[Serializable]
public class TableData
{
    //public TableInfo[] tableInfoList;
    public Dictionary<int, List<TableInfo>> Data;
}

