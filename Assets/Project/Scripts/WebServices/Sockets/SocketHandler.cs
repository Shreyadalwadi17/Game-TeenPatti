using BaseFramework;
using PlatformSupport.Collections.ObjectModel;
using System;
using System.Threading.Tasks;
using BestHTTP.SocketIO3;
using BestHTTP.SocketIO3.Transports;
using Newtonsoft.Json;
using SimpleJSON;
using UnityEngine;

public class SocketHandler : Singleton<SocketHandler>
{
    /// <summary>
    /// The Socket.IO manager instance.
    /// </summary>
    private SocketManager Manager;
    public Socket root;
    public bool isReconnected = false;
    bool isQuiting;
    void OnDestroy()
    {
        if (this.Manager != null)
        {
            // Leaving this sample, close the socket
            this.Manager.Close();
            this.Manager = null;
        }
    }

    public void CreateSocket()
    {
        CloseAllConnections();
        SocketOptions options = new SocketOptions();
        options.AutoConnect = false;
        options.ConnectWith = TransportTypes.WebSocket;
        options.Timeout = TimeSpan.FromMilliseconds(60000);
        options.Reconnection = false;
        options.AdditionalQueryParams = new ObservableDictionary<string, string>();
        options.AdditionalQueryParams.Add("authorization",APIData.APIAuthToken);
        Manager = new SocketManager(new Uri(APIData.finalBaseSocketURL), options);
        root = Manager.Socket;
        SetRootListeners();

        //Socket Logger
        //HTTPManager.Logger.Level = Loglevels.All;
        
        Manager.Open();

    }
    public void CloseAllConnections()
    {
        if (Manager != null)
        {
            Debug.Log("Cleaning Root first");
            Manager.Close();
            Manager = null;
        }
    }
    
    private void SetRootListeners()
    {
        root.On(SocketIOEventTypes.Connect, OnServerConnected);
        root.On(SocketIOEventTypes.Disconnect, OnServerDisconnected);
        root.On<Error>(SocketIOEventTypes.Error, (err) =>
        {
            print("<color=red>There is Some error occured from server : </color>" + err.message);
        });
        
    }

    public void AddTableListener(string iTableId)
    {
        Debug.Log("AddTableListener: " + iTableId);
       
        root.On<object>(iTableId, (data) =>
        {
            var jsonString = JsonConvert.SerializeObject(data);
            JSONNode node = JSONNode.Parse(jsonString);
            string en = node["sEventName"];
           #region Server events

                try
                {
                    GamePlayServerEvents(node); // You will receive all event name
                }
                catch (Exception e)
                {
                    Debug.Log("<color=red>There is some issue from server, the data is/might not available or Field of object has changed! : </color>" 
                          + e.Message);
                }

            #endregion
        });
    }
    public void RemoveTableListener(string iTableId)
    {
        if (root.IsOpen)
        {
            print("RemoveTableListener: " + iTableId);
            root.Off(iTableId);
        }
    }

    private void GamePlayServerEvents(JSONNode node)
    {
        //Handle responses from server and apply it into our Game. 
        SocketServerEventType reqType= (SocketServerEventType)Enum.Parse(typeof(SocketServerEventType), node["sEventName"]);
        string data = node["oData"].ToString();
        
         if (root.IsOpen)
            Debug.Log("<color=aqua> SEvents : " + reqType + " </color>");
        else
            Debug.Log("Socket is Not Connected!");
        
        switch (reqType)
        {
            
            case SocketServerEventType.resGameInitializeTimer:
                Debug.LogError("resGameInitializeTimer");
                Debug.Log("<color=yellow>resGameInitializeTimer Event Received!</color>" + data);
                TableManager.instance.SetGameStartingTimer(data);
                break;
            
            case SocketServerEventType.resPack:
                Debug.Log("<color=yellow>resPack Event Received!</color>" + data);
                Debug.LogError("resPack");
                if (TableManager.instance.currentPlayer !=null)
                {
                    TickTimer.OnUpdateTick -= TableManager.instance.OnUpdateTick;
                    TableManager.instance.currentPlayer.ResetTimer();
                }
                break;
            
            case SocketServerEventType.resResult:
                Debug.Log("<color=yellow>resResult Event Received!</color>" + data);
                Debug.LogError("resResult");
                TableManager.instance.ShowResult(data);
                break;
            
            case SocketServerEventType.resBoardState:
                Debug.Log("<color=yellow>resBoardState Event Received!</color>" + data);
                Debug.LogError("resBoardState");
                //This Event will have full data of Table ,This will also be used for Reconnecting
                TableManager.instance.SetTableDetails(data);
                Dealer.instance.Initialize();
                break;
            
            case SocketServerEventType.resCardSeen:
                Debug.Log("<color=yellow>resCardSeen Event Received!</color>" + data);
                Debug.LogError("resCardSeen");
                TableManager.instance.SetCardSeenLable(node);
                
                
                break;
            
            case SocketServerEventType.resKickOut:
                Debug.LogError("resKickOut");
                Debug.Log("<color=yellow>resKickOut Event Received!</color>" + data);
                break;
            
            case SocketServerEventType.resPlaceBet:
                Debug.Log("<color=yellow>resPlaceBet Event Received!</color>" + data);
                Debug.LogError("resPlaceBet");
                Debug.LogError("the data recieved "+data);
                TableManager.instance.currentPlayer.PayChips();
                TableManager.instance.SetPotValue(data);
                if (TableManager.instance.currentPlayer !=null)
                {
                    TickTimer.OnUpdateTick -= TableManager.instance.OnUpdateTick;
                    TableManager.instance.currentPlayer.ResetTimer();
                    
                }
                break;
            
            case SocketServerEventType.resPlayerLeft:
                Debug.Log("<color=yellow>resPlayerLeft Event Received!</color>" + data);
                Debug.LogError("resPlayerLeft");
                TableManager.instance.PlayerLeft(data);
                break;
            
            case SocketServerEventType.resPlayersState:
                Debug.LogError("resPlayersState");
                Debug.Log("<color=yellow>resPlayersState Event Received!</color>" + data);
                TableManager.instance.ChangePlayerStates(data);
                break;
            
            case SocketServerEventType.resPlayerTurn:
                Debug.LogError("resPlayerTurn");
                Debug.Log("<color=yellow>resPlayerTurn Event Received!</color>" + data);
                TableManager.instance.SetPlayerTurn(data);
                break;
            
            case SocketServerEventType.resShowHand:
                Debug.Log("<color=yellow>resShowHand Event Received!</color>" + data);
                Debug.LogError("resShowHand");
                //TableManager.instance.currentPlayer.PayChips();
                TableManager.instance.ShowHandForEveryOne(node["oData"]["aParticipant"].ToString());
                break;
            
            case SocketServerEventType.resTurnMissed:
                Debug.LogError("resTurnMissed");
                Debug.Log("<color=yellow>resTurnMissed Event Received!</color>" + data);
                TableManager.instance.PlayerTurnMissed(data);
                break;
            
            case SocketServerEventType.resUserJoined:
                Debug.LogError("resUserJoined");
                Debug.Log("<color=yellow>resUserJoined Event Received!</color>" + data);
                //  Debug.LogError("here in player data detailds"+data);
                TableManager.instance.GeneratePlayer(data);
                break;
            
            case SocketServerEventType.resHand:
                Debug.LogError("resHand");
                Debug.Log("<color=yellow>resHand Event Received!</color>" + data);
                //Will get Player Hands 
                TableManager.instance.SetUserHand(data);
                break;
            
            case SocketServerEventType.resetTable:
                Debug.LogError("resetTable");
                Debug.Log("<color=yellow>resetTable Event Received!</color>" + data);
                TableManager.instance.ResetTableForNewRound(data);
                break;
            
            case SocketServerEventType.resGameState:
                Debug.LogError("resGameState");
                Debug.Log("<color=yellow>resGameState Event Received!</color>" + data);
                TableManager.instance.RejoinGame(data);
                break;
            
        }
    }
    private void OnApplicationPause(bool pauseStatus)
    {
        OnPause(pauseStatus);
    }

    public void OnPause(bool pauseStatus)
    {
        try
        {
            if (!pauseStatus)
            {
                if (!root.IsOpen)
                {
                    Debug.Log("You are back! Check Socket Connection :" + root.IsOpen);
                    CheckAndReconnectRoot(3);
                }
            }
            else
            {
                root.Disconnect();
            }
        }
        catch (Exception e)
        {
            Debug.Log("Application Pause:" + e.Message);
        }
    }
    public async void CheckAndReconnectRoot(int attempts)
    {
        try
        {
            //OnRootDisconnected();   //TODO : later subscribe unsubscribe on disconnect
            Debug.Log("server rechable ");
   
            SB:
            bool val = await CheckNetwork.instance.IsServerReachable();
            Debug.Log("server rechable " + val);
            if (val)
            {
                //App is quitting - we don't required to reconnect the root.
                if (isQuiting) return;
                isReconnected = true;
                CreateSocket();
                //if (root.IsOpen)
                //{
                //}
                //else
                //{
                //root.Manager.Open();
                //}
                //Preloader.Instance.Hide();
            }
            else
            {
                Debug.Log("Creating root socket attempt minus");
                attempts--;
                if (attempts > 0)
                {
                    await Task.Delay(2 * 1000);
                    goto SB;
                }
                else
                {
                    //Preloader.Instance.Hide();
                    //Alert.Instance.Show(LocalizationManager.Instance.data.labels.connection_error, LocalizationManager.Instance.data.messages.lostServerConnectionAlert, -1f, false, (res) =>
                    //{
                    //    if (res)
                    //    {
                    CheckAndReconnectRoot(3);
                    //        Preloader.Instance.Show();
                    //    }
                    //}, LocalizationManager.Instance.data.labels.reconnect);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Reconnect :" + e.Message);
        }
    }
    
    private void OnServerConnected()
    {
        Debug.Log("Connected : ID - " + Manager.Socket.Id);
        if (isReconnected)
        {
            GameManager.instance.AskToJoinTable();
        }
    }

    private void OnServerDisconnected()
    {
        Debug.Log("Disconnected " + Manager.Socket.Id);
    }
    
    private void OnApplicationQuit()
    {
        Debug.Log("OnApplicationQuit");
        //isQuiting = true;
        CloseAllConnections();
    }

}
