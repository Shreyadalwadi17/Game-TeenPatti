using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TableListElementUI : MonoBehaviour
{
    public Button joinBtn;
    public TextMeshProUGUI tableTypeLbl;
    public TextMeshProUGUI WiningAmountLbl;
    public TextMeshProUGUI MaxPlayerLbl;
    public TextMeshProUGUI JoiningFeeLbl;
    public string protoId;


    private void Awake()
    {
        joinBtn.onClick.AddListener(JoinTable);
    }

    private void JoinTable()
    {
        ServicesHandler.instance.CallBoardJoin(protoId);
     
        
    }
}



