using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BaseFramework;
using System;

public class ServicesHandler : IndestructibleSingleton<ServicesHandler>
{
    public void CallRegisterService(string username, string password, string email)
    {
        KVPList<string, string> serviceData = new KVPList<string, string>();

        serviceData.Add("sEmail", email);
        serviceData.Add("sPassword", password);
        serviceData.Add("sUserName", username);

        Services.Post(APIData.API_register, serviceData, (response) =>
        {
            Events.OnWebRequestComplete(API_TYPE.API_register, response);
        });
    }

    public void CallLoginService(string email, string password)
    {
        KVPList<string, string> serviceData = new KVPList<string, string>();

        serviceData.Add("sEmail", email);
        serviceData.Add("sPassword", password);

        Services.Post(APIData.API_login, serviceData, (response) =>
        {
            Events.OnWebRequestComplete(API_TYPE.API_login, response);
        });
    }

    public void CallGetProtoList()
    {
        Services.Get(APIData.API_getProtoList,(response) =>
        {
            Events.OnWebRequestComplete(API_TYPE.API_getProtoList, response);
        });
    }

    public void CallBoardJoin(string protoId)
    {
        KVPList<string, string> serviceData = new KVPList<string, string>();
        
        serviceData.Add("iProtoId",protoId);
        Services.Post(APIData.API_boardJoin,serviceData, (response) =>
        {
            Events.OnWebRequestComplete(API_TYPE.API_boardJoin, response);
        });
        Preloader.instance.Show();
    }

    public void CallViewProfileService()
    {
        Services.Get(APIData.API_viewProfile,(response) =>
        {
            Events.OnWebRequestComplete(API_TYPE.API_viewProfile, response);
        });
    }

    public void CallGetPing()
    {
        Services.Get(APIData.API_ping, (respomse) =>
        {
            Events.OnWebRequestComplete(API_TYPE.API_ping,respomse);
        });
    }
}
