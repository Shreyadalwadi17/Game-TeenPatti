using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using BaseFramework;
using UnityEngine;
using UnityEngine.Networking;


public class KVPList<TKey, TValue> : List<KeyValuePair<TKey, TValue>>
{
    public void Add(TKey Key, TValue Value)
    {
        base.Add(new KeyValuePair<TKey, TValue>(Key, Value));
    }
}

public class APIRequest
{
    public METHOD RequestMethod;
    public string URL;
    public KVPList<string, string> data;
    public KVPList<string, byte[]> rawdata;
    public Action<string> OnServiceCallBack;
    public bool shouldAuthorize = true;
    public bool withTimeOut = false;

    public APIRequest(METHOD requestMethod, string requestURL, Action<string> requestCallBack, KVPList<string, string> requestData = null, KVPList<string, byte[]> requestRawdata = null, bool shouldRequestAuthorize = true, bool requestwithTimeOut = false)
    {
        RequestMethod = requestMethod;
        URL = requestURL;
        OnServiceCallBack = requestCallBack;
        data = requestData;
        rawdata = requestRawdata;
        shouldAuthorize = shouldRequestAuthorize;
        withTimeOut = requestwithTimeOut;
    }
}

public class RequestScheduler
{

    List<APIRequest> RequestQueue;
    public bool isRequestActive;

    bool ActiveScheduler;

    public RequestScheduler()
    {
        RequestQueue = new List<APIRequest>();

        ActiveScheduler = true;
        RequestsHandler();
    }

    public void KillRequestSchedular()
    {
        ActiveScheduler = false;
    }

    public bool IsRequeastPresentInQueue(string URL)
    {

        foreach (var req in RequestQueue)
        {
            if (req.URL.Contains(URL))
            {
                return true;
            }
        }

        return false;
    }

    public async void RequestsHandler()
    {
        do
        {

            if (/*(Sockets._socket != null && Sockets._socket.IsOpen) || */Application.internetReachability != NetworkReachability.NotReachable)
            {
                if (RequestQueue.Count > 0 && !isRequestActive)
                {
                    SendRequest(RequestQueue[0]);
                }
            }


            //Debug.Log("Scheduler Active");
            await Task.Delay(10);

        } while (ActiveScheduler);
    }

    public void SendRequest(APIRequest request)
    {
        Debug.Log("<color='magenta'>Sending Request " + request.URL + "</color>");


        isRequestActive = true;

        switch (request.RequestMethod)
        {
            case METHOD.GET:

                Services.Get(request.URL, (response) =>
                {

                    RequestQueue.RemoveAt(0);
                    isRequestActive = false;

                    Debug.Log("<color='magenta'>Completed Request : " + request.URL + "</color>");

                    if (Services.IsValidJson(response))
                    {
                        request.OnServiceCallBack(response);
                    }
                    else
                    {
                        AddRequest(request);
                    }


                }, request.shouldAuthorize, request.withTimeOut, false, true);


                break;
            case METHOD.POST:
                {
                    Services.Post(request.URL, request.data, (response) =>
                    {

                        RequestQueue.RemoveAt(0);
                        isRequestActive = false;

                        Debug.Log("<color='magenta'>Completed Request : " + request.URL + "</color>");

                        if (Services.IsValidJson(response))
                        {
                            request.OnServiceCallBack(response);
                        }
                        else
                        {
                            AddRequest(request);
                        }




                    }, request.shouldAuthorize, false, true);

                    break;
                }



              
        }
    }

    public void AddRequest(APIRequest request)
    {


        APIRequest lookreq = RequestQueue.Find(x => x.URL.Equals(request.URL));

        if (lookreq != null)
        {
            RequestQueue.Remove(lookreq);
            Debug.Log("<color='magenta'>Removed Request : " + request.URL + "</color>");

        }

        RequestQueue.Add(request);

        Debug.Log("<color='magenta'>Added Request : " + request.URL + "</color>");

        Debug.Log("<color='green'>Queue count : " + RequestQueue.Count + "</color>");
    }


}

public enum METHOD
{
    GET,
    POST,
    POST_RAW,
    POST_MIX,
}

public class Services : Singleton<Services>
{
    public static RequestScheduler APIRequestScheduler;

    static bool isSchedulerImplemented = false;



    public override void OnAwake()
    {
        APIData.isLive = false;

        APIData.SetupBaseURL();
        APIData.SetupBaseSocketURL();
        
    }

    private void Start()
    {
        APIRequestScheduler = new RequestScheduler();
    }

    private void OnDestroy()
    {
        APIRequestScheduler.KillRequestSchedular();
    }

    #region POST METHOD

    public static async void Post(string postURL, KVPList<string, string> data, Action<string> OnServiceCallBack,
        bool shouldAuthorize = true, bool isToBeAddedToScheduler = true, bool isFromScheduler = false)
    {

        if (isToBeAddedToScheduler && isSchedulerImplemented)
        {

            APIRequestScheduler.AddRequest(new APIRequest(METHOD.POST, postURL, OnServiceCallBack, data, null, shouldAuthorize));
            return;
        }


        postURL = postURL.FinalizeURL();

        Debug.Log("<color='blue'> URL :: " + postURL + "</color>");


        WWWForm formData = new WWWForm();

        string m_body = "";

        for (int count = 0; count < data.Count; count++)
        {
            m_body = m_body + data[count].Key + ":" + data[count].Value + "\n";
            formData.AddField(data[count].Key, data[count].Value.ToString());
        }

        Debug.Log("Body " + "\n" + m_body);

        UnityWebRequest request = UnityWebRequest.Post(postURL, formData);

        if (shouldAuthorize)
        {
            request.SetRequestHeader("authorization", APIData.APIAuthToken);
        }


        request.timeout = 10;


        await request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success && IsValidJson(request.downloadHandler.text))
        {
            string authToken = request.GetResponseHeader("Authorization");
            if (!string.IsNullOrEmpty(authToken))
            {
                APIData.APIAuthToken = authToken;
            }
            
            Debug.Log("<color=green>" + request.downloadHandler.text + "</color>");
            OnServiceCallBack(request.downloadHandler.text);
        }
        else
        {
            Debug.LogError("API ERROR : " + request.error +" msg :"+ request.downloadHandler.text);

            if (isFromScheduler)
            {
                OnServiceCallBack(request.downloadHandler.text);
            }
        }
    }

    #endregion

    #region GET METHOD

    public static async void Get(string getURL, Action<string> OnServiceCallBack,
        bool shouldAuthorize = true, bool withTimeOut = false, bool isToBeAddedToScheduler = true, bool isFromScheduler = false)
    {

        if (isToBeAddedToScheduler && isSchedulerImplemented)
        {

            APIRequestScheduler.AddRequest(new APIRequest(METHOD.GET, getURL, OnServiceCallBack, null, null, shouldAuthorize, withTimeOut));
            return;
        }

        getURL = getURL.FinalizeURL();

        Debug.Log("URL :: " + getURL);

        UnityWebRequest request = UnityWebRequest.Get(getURL);


        if (withTimeOut)
        {
            request.timeout = 5;
        }


        if (shouldAuthorize)
        {
            request.SetRequestHeader("Authorization", APIData.APIAuthToken);
        }

        await request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success && IsValidJson(request.downloadHandler.text))
        {
            Debug.Log("<color=green>" + request.downloadHandler.text + "</color>");
            OnServiceCallBack(request.downloadHandler.text);
        }
        else
        {

            Debug.LogError(getURL + " API ERROR : " + request.downloadHandler.text);


            if (isFromScheduler)
            {
                OnServiceCallBack(request.downloadHandler.text);
            }

        }
    }


    #region IMAGE_UPLOAD

    public static async void UploadImage(string PostURL, KVPList<string, byte[]> data, Action<string> OnServiceCallBack,
        bool shouldAuthorize = true)
    {
        PostURL = PostURL.FinalizeURL();

        Debug.Log("<color='blue'> URL :: " + PostURL + "</color>");


        WWWForm formData = new WWWForm();


        for (int count = 0; count < data.Count; count++)
        {
            formData.AddBinaryData(data[count].Key, data[count].Value);
        }


        UnityWebRequest request = UnityWebRequest.Post(PostURL, formData);

        if (shouldAuthorize)
        {
            request.SetRequestHeader("Authorization", APIData.APIAuthToken);
        }

        request.timeout = 10;

        await request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success && IsValidJson(request.downloadHandler.text))
        {
            Debug.Log("<color=green>" + request.downloadHandler.text + "</color>");
            OnServiceCallBack(request.downloadHandler.text);
        }
        else
        {
            Debug.LogError("API ERROR : " + request.downloadHandler.text);


        }
    }


    #endregion


    public static async Task DownloadThisImage(string imgUrl, Action<Texture2D> downloadedTexture)
    {
        string imgFileName = Path.GetFileNameWithoutExtension(imgUrl);

        string imgFolder = APIData.ImagesDirectoryPath + "/";
        if (!Directory.Exists(imgFolder)) Directory.CreateDirectory(imgFolder);

        bool imgExists = false;
        if (File.Exists(imgFolder + imgFileName))
        {
            imgExists = true;
            imgUrl = "file://" + imgFolder + imgFileName;
        }

        UnityWebRequest www = UnityWebRequestTexture.GetTexture(imgUrl);

        await www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(www.url + " - " + www.error);
        }
        else
        {
            Texture myTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;
            downloadedTexture.Invoke(myTexture as Texture2D);
            if (!imgExists)
            {
                File.WriteAllBytes(Path.Combine(Application.persistentDataPath, imgFolder + imgFileName),
                    www.downloadHandler.data);
            }
        }
    }

    #endregion

    #region MixedData

    public static async void PostMixedData(string PostURL, KVPList<string, string> data, KVPList<string, byte[]> rawdata,
        Action<string> OnServiceCallBack,
        bool shouldAuthorize = true)
    {
        PostURL = PostURL.FinalizeURL();

        Debug.Log("<color='blue'> URL :: " + PostURL + "</color>");


        WWWForm formData = new WWWForm();

        string m_body = "";

        for (int count = 0; count < data.Count; count++)
        {
            m_body = m_body + data[count].Key + ":" + data[count].Value + "\n";
            formData.AddField(data[count].Key, data[count].Value.ToString());
        }

        for (int count = 0; count < rawdata.Count; count++)
        {
            m_body = m_body + rawdata[count].Key + ":" + rawdata[count].Value + "\n";
            formData.AddBinaryData(rawdata[count].Key, rawdata[count].Value);
        }

        Debug.Log("Body " + "\n" + m_body);

        UnityWebRequest request = UnityWebRequest.Post(PostURL, formData);

        if (shouldAuthorize)
        {
            request.SetRequestHeader("Authorization", APIData.APIAuthToken);
        }

        request.timeout = 10;

        await request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success && IsValidJson(request.downloadHandler.text))
        {
            Debug.Log("<color=green>" + request.downloadHandler.text + "</color>");
            OnServiceCallBack(request.downloadHandler.text);
        }
        else
        {
            Debug.LogError("API ERROR : " + request.downloadHandler.text);

        }
    }

    #endregion


    #region Utilities

    public static bool IsValidJson(string strInput)
    {
        strInput = strInput.Trim();
        if (!string.IsNullOrEmpty(strInput)) //For array
        {
            try
            {
                var obj = JsonUtility.ToJson(strInput);
            }
            catch (Exception ex) //some other exception
            {
                Console.WriteLine(ex.ToString());
                return false;
            }

            return true;
        }
        else
        {
            return false;
        }
    }

    #endregion

}
