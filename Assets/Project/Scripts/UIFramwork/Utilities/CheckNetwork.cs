using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseFramework;
using Newtonsoft.Json;
using SimpleJSON;
using UnityEngine;
using UnityEngine.Networking;
/// <summary>
/// Check network.
/// </summary>
public class CheckNetwork : IndestructibleSingleton<CheckNetwork>
{

    /// <summary>
    /// Is the internet rechable.
    /// </summary>
    /// <returns>The internet rechable.</returns>
    public async Task<bool> IsInternetRechable()
    {
        Debug.Log("Calling IsInternetRechable ");
        UnityWebRequest req = UnityWebRequest.Get("https://www.google.com");
        await req.SendWebRequest();
        return req.responseCode == 200;
    }

    /// <summary>
    /// Is the server reachable.
    /// </summary>
    /// <returns>The server reachable.</returns>
    public async Task<bool> IsServerReachable()
    {
        Debug.Log("Calling IsServerReachable ");
        UnityWebRequest req = UnityWebRequest.Get(APIData.finalBaseSocketURL+"/ping");
        await req.SendWebRequest();
        return req.responseCode == 200;
    }

    #region  PING HANDLING

    List<float> pingReceived; bool isCanPing; int pingCount = 1; float pingTimer; float averagePing;
    internal float pingInterval;

    public  override void OnAwake()
    {
        pingReceived = new List<float>();
        pingReceived.Add(0.14f);
    }

    public void StartPingChecker()
    {
        pingReceived = new List<float>();
        isCanPing = true;
    }

    public int getPing()
    {
        return (int)(averagePing * 1000);
    }
    public void StopPingChecker()
    {
        isCanPing = false;
        ResetPingChecker();
    }

    public void ResetPingChecker()
    {
        pingCount = 1;
        pingTimer = 0f;
        averagePing = 0f;
        pingReceived.Clear();
    }

    private void Update()
    {
        if (!isCanPing) return;

        pingTimer += Time.deltaTime;

        if (pingTimer > 1)//GS.Instance.globalSetting.oPingInfo.nPingInterval
        {
            pingTimer -= 1;//GS.Instance.globalSetting.oPingInfo.nPingInterval;
            OnCheckPing();
            pingCount++;
            if (pingCount % 3 == 0)
            {
                pingTimer -= 3;//GS.Instance.globalSetting.oPingInfo.nPingDelay;
                averagePing = NormalisePingInterval();
                pingReceived.Clear();
            }
        }
    }

    public float SignalStrength
    {
        get
        {
            /*return 1f - TableManager.instance.clamp01(averagePing, 0, GS.Instance.globalSetting.oPingInfo.nPingLatency);*/
            return 1f - TableManager.instance.clamp01(averagePing, 0, 1);
        }
    }

    float NormalisePingInterval()
    {
        return pingReceived.Sum() / pingReceived.Count;

        //float n = 0;
        //foreach (float f in pingReceived)
        //{
        //    n += f;
        //}
        //n /= pingReceived.Count;
        //return n;
    }

    void OnCheckPing()
    {
        Debug.Log("OnCheckPing Fired");
        JSONNode data = new JSONObject();
        data.Add("message", "");
        if (SocketHandler.Instance.root.IsOpen)
        { 
            JSONNode node = new JSONObject();
            SocketHandler.Instance.root.ExpectAcknowledgement<object>(OnCheckPingCallBack)
                    .Emit(GameManager.instance.TableId,new {sEventName = SocketServerEventType.reqPack.ToString()});
        }
        //else
        //{
        //    Alert.Instance.Show(LocalizationManager.Instance.data.labels.internet_connection, LocalizationManager.Instance.data.messages.internetAlert, -1f, false, (res) =>
        //    {
        //        if (res)
        //        {

        //           ApiManager.Instance.CheckAndReconnectToInternet();
        //        }
        //    }, LocalizationManager.Instance.data.labels.connect);
        //    // Alert.Instance.Show(LocalizationManager.Instance.data.labels.connection_error, LocalizationManager.Instance.data.messages.disconnectedFromGame);
        //    //if (GS.Instance.gameState == EGameState.Table)
        //    //    GameManager.Inst.BackToDashboard();
        //}
    }

    private void OnCheckPingCallBack(object obj)
    {
        float dt = Time.realtimeSinceStartup;
        var jsonString = JsonConvert.SerializeObject(obj);
        JSONNode json = JSONNode.Parse(jsonString);
        Debug.Log(json.ToString());
        string error = json["error"];
        print("<color=purple> OnAnsCallBack </color>" + json.ToString());
        if (string.IsNullOrEmpty(error))
        {
            pingInterval = Time.realtimeSinceStartup - dt;
            pingReceived.Add(pingInterval);
            //MyDebug.LOG("<color=purple>Ping Interval: " + pingInterval + "</color>");
        }

    }
    #endregion

}
