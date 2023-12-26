using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WinnerPopupUI : MonoBehaviour
{
    [Header("UI Elements")] 
    public TextMeshProUGUI playerNameLbl;
    public TextMeshProUGUI cardTypeLbl;

    [Header("Cards")]
    public List<CardDisplayForWinner> cards;
    
    
}
