using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace BaseFramework
{
    public static class APIData
    {

        #region API URLS

        public static bool isLive = false;
        public const string baseAPIURL = "https://backend.midnightnoise.com";
        public const string localAPIURL = "https://teenpatti-backend.lc.webdevprojects.cloud";

        public const string localAPIPortNum = "3000";

        public const string APIPostFixURL = "/api/v1/";
        public static string finalBaseSocketURL = "";
        public static string finalBaseURL = "";


        // ++++++++++++++++++++++++++++++++ ALL APIs ++++++++++++++++++++++++++++++++
        public const string API_register = "auth/register";
        public const string API_login = "auth/login/simple";
        public const string API_viewProfile = "profile/";
        public const string API_getProtoList = "teenpatti/board/list/cash";
        public const string API_boardJoin = "teenpatti/board/join";
        public const string API_ping = "/ping";
        


        public static void SetupBaseURL()
        {
            /*finalBaseURL = (isLive ? baseAPIURL : localAPIURL + ":" + localAPIPortNum) +
                           APIPostFixURL;*/
            finalBaseURL = (isLive ? baseAPIURL : localAPIURL) + APIPostFixURL;
        }

        public static void SetupBaseSocketURL()
        {
            //finalBaseSocketURL = (isLive ? baseSocketURL : localBaseSocketURL + ":" + localSocketPortNum);
            finalBaseSocketURL = (isLive ? baseSocketURL : localBaseSocketURL);
        }

        public static string FinalizeURL(this string m_url)
        {
            return finalBaseURL + m_url;
        }

        #endregion




        #region Sockets
        public const string baseSocketURL = "https://teenpatti-backend.lc.webdevprojects.cloud";

        public const string localBaseSocketURL = "https://teenpatti-backend.lc.webdevprojects.cloud";

        public const string localSocketPortNum = "3000";
        #endregion


        #region APP_Keys & IDs

        public static string APIAuthToken = "";

        #endregion

        public static string ImagesDirectoryPath = Application.persistentDataPath + "/img";

    }

    public enum API_TYPE
    {
        None,
        API_register,
        API_login,
        API_viewProfile,
        API_getProtoList,
        API_boardJoin,
        API_ping
    }
}