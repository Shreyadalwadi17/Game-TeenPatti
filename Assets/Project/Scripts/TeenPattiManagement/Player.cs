using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using SimpleJSON;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public enum PlayerStatus
{
    Folded,
    Seen,
    Packed
}

[Serializable]
public class PlayerData
{
    public string iUserId;
    public int nSeat;
    public string eUserType;
    public string sUserName;
    public int nChips;
    public string eState;
    public int nTurnMissed;
    public AClosedDeck[] aHand;
    public bool bCardSeen;
    public string handType;
}


[System.Serializable]
public class Player : MonoBehaviour
{
    public PlayerData playerData;
    public CardHolder holder;
    public PlayerStatus playerStatus;
    public Hand hand;
    public TextMeshPro textMeshPro;
    public TextMeshPro nameLbl;
    public Button seeCardBtn;
    public TextMeshProUGUI eventPlayedLbl;
    public TextMeshProUGUI cardSeenLbl;
    public Image timer;
    TableManager currentDealer;
    public Transform chipsHolder;
    
    public GameObject chipPrefab;
    public UnityAction<int, Move, Player> OnPlayerMakeAMove;

    bool isMyTurn = false;
    Vector3 initalScale;

    public bool IsInGame => playerStatus == PlayerStatus.Folded;

    public int Balance
    {
        get => playerData.nChips;
        set
        {
            playerData.nChips = value;
            textMeshPro.text = value.ToString();
        }
    }

    private void Start()
    {
        holder = GetComponentInChildren<CardHolder>();
        initalScale = transform.localScale;
        nameLbl.text = playerData.sUserName; 
        timer.fillAmount = 0;
        eventPlayedLbl.enabled = false;
        cardSeenLbl.gameObject.SetActive(false);
        
    }

    public void Initialize(TableManager dealer)
    {
        currentDealer = dealer;
        Debug.LogError("the current dealer "+dealer.gameObject.name);
        hand = null;
        playerStatus = PlayerStatus.Folded;
        
        dealer.OnTurnStart += OnTurnStart;
        dealer.OnTurnEnd += OnTurnEnd;
        dealer.OnWinnerDetermined += WinnerDetermined;
        dealer.OnAPlayerCalledShow += OnAPlayerCalledShow;
    }

    

    public void ResetPlayer()
    {
        hand = null;
        playerStatus = PlayerStatus.Folded;
        playerData.bCardSeen = false;
        currentDealer.OnTurnStart -= OnTurnStart;
        currentDealer.OnWinnerDetermined -= WinnerDetermined;
        currentDealer.OnAPlayerCalledShow -= OnAPlayerCalledShow;
        currentDealer.OnTurnEnd -= OnTurnEnd;
        //currentDealer = null;
        playerData.eState = "playing";
        cardSeenLbl.gameObject.SetActive(false);
        eventPlayedLbl.text =String.Empty;
        eventPlayedLbl.enabled = false;
        transform.DOScale(initalScale, 0.3f);
        foreach (Transform child in chipsHolder) {
            
            Destroy(child.gameObject);
        }
       
        transform.DOScale(initalScale, 0.4f);
    }

    public async void PayChips()
    {
        for (int i = 0; i < 10; i++)
        {
            GameObject chip = Instantiate(chipPrefab, chipsHolder);
           
            chip.transform.DOMove(GenerateRandomPosition(), 1f).OnComplete(() =>
            {
                chip.transform.SetParent(TableManager.instance.potCenter);
            });
            TableManager.instance.chips.Add(chip);
            await Task.Delay(1);
        }
        await Task.Delay(2);
    }
    Vector3 GenerateRandomPosition()
    {
        if (TableManager.instance.potCenter != null)
        {
            Vector3 targetPosition = TableManager.instance.potCenter.transform.position;
            Vector3 randomOffset = Random.insideUnitSphere * 0.25f;
            Vector3 randomPosition = targetPosition + new Vector3(randomOffset.x, randomOffset.y, randomOffset.z);
           
            return randomPosition;
        }
        else
        {
            Debug.LogError("Target object not assigned!");
            return Vector3.zero;
        }
        
    }

    public async void WinningChips(Player player)
    {
        foreach (var chip in TableManager.instance.chips)
        {
            chip.transform.DOMove(chipsHolder.position, 1f).OnComplete(() =>
            {
                chip.transform.SetParent(player.chipsHolder);
                
            });
            await Task.Delay(1);
        }
        await Task.Delay(5);
       
        TableManager.instance.chips.Clear();
        TableManager.instance.ShowWinner();
    }


    private void OnAPlayerCalledShow(Player player)
    {
        if (player != this && playerStatus != PlayerStatus.Packed)
        {
            holder.ToggleOpenCloseCards(true);
        }
    }

    private void WinnerDetermined(Player winningPlayer)
    {
        if (this == winningPlayer)
        {
            TableManager.instance.gameUI.HideButtonsToPlayer();
            Debug.Log("Yoyo effect Starting");
            transform.DOScale(Vector3.one * 1.2f, 0.2f).SetLoops(10, LoopType.Yoyo).OnComplete(() =>
            {
                Debug.Log("Yoyo effect Complete");
                WinningChips(this);
            });
            WinPot(currentDealer.currentPot.totalPot);
        }
        
    }
    
    
    private void OnTurnStart(Player player)
    {
        Debug.Log("OnTurnStart " + player.playerData.iUserId);
        Debug.LogError("in player on start");
        if (TableManager.instance.currentPlayer != null)
        {
            Debug.Log("Current Player"+ TableManager.instance.currentPlayer.playerData.sUserName);
            //TableManager.instance.currentPlayer.SetinitalScaleForPLayer();
            //TickTimer.OnUpdateTick -= OnUpdateTick;            
        }
        if (player == this)
        {
            Debug.LogError("first");
            if (playerStatus != PlayerStatus.Packed)
            {
                isMyTurn = true;
                   Debug.LogError("second");
                transform.DOScale(initalScale + (Vector3.one * 0.3f), 0.5f);
                if (player.playerData.iUserId.Equals(GameManager.instance.myPlayerId))
                {
                    Debug.LogError("third hide 1");
                    TableManager.instance.gameUI.ShowButtonsToPlayer();
                }
                else
                {
                     Debug.LogError("third hide 2");
                    TableManager.instance.gameUI.HideButtonsToPlayer();
                }
            }

            StartCoroutine(TurnRoutine());
        }
        else
        {
            isMyTurn = false;
            timer.color = Color.white;
        }
    }
    private void OnTurnEnd(Player player)
    { 
        Debug.Log("OnTurnEnd " + player.playerData.iUserId);
        if (player == this)
        {
            if (playerStatus != PlayerStatus.Packed)
            {
                isMyTurn = false;
                transform.DOScale(initalScale, 0.4f);
                //SetinitalScaleForPLayer();
                
            }
        }
    }

    IEnumerator TurnRoutine()
    {
        while (isMyTurn)
        {
            yield return null;
        }

        transform.DOScale(initalScale, 0.4f);
    }


    public void AddBalance(int chips)
    {
        Debug.Log("AddToPot " + chips);
        Balance += chips;
    }

    public void MakeABet(int chips)
    {
        Balance -= chips;
    }

    public void OnCardsRecieved()
    {
        holder.SortCards();
        hand = new Hand(holder.GetCards());
        if (!GameManager.instance.myPlayerId.Equals(playerData.iUserId))
        {
            seeCardBtn.gameObject.SetActive(false);
        }
        else
        {
            seeCardBtn.gameObject.SetActive(true);
        }
        
    }

    public void MakeMove(Move move)
    {
        Debug.Log(playerData.sUserName + " " + move);

        isMyTurn = false;


        int chips = 0;

        switch (move)
        {
            case Move.Blind:
                // Implement Blind move logic here
                chips = Blind();
                break;

            case Move.Call:
                // Implement Call move logic here
                chips = Call();
                break;

            case Move.Raise:
                // Implement Raise move logic here
                chips = Raise();
                break;

            case Move.Pack:
                // Implement Pack move logic here
                chips = Pack();
                break;

            case Move.SideShow:
                chips = SideShow();
                // Implement side show move logic here
                break;

            case Move.Show:
                chips = Show();
                break;

            default:
                throw new ArgumentException("Invalid move.");
        }

        OnPlayerMakeAMove?.Invoke(chips, move, this);
    }

    public int Pack()
    {
        Debug.Log("Pack Called by Player");
        eventPlayedLbl.text = "PACK";
        eventPlayedLbl.enabled = true;
        if (TableManager.instance.currentPlayer !=null)
        {
            TickTimer.OnUpdateTick -= TableManager.instance.OnUpdateTick;
            TickTimer.OnTick -= TableManager.instance.OnTick;
            TableManager.instance.currentPlayer.ResetTimer();
        }
        TableManager.instance.gameUI.HideButtonsToPlayer();
        if (SocketHandler.Instance.root.IsOpen)
        {
            JSONNode node = new JSONObject();
            node.Add("sEventName", SocketServerEventType.reqPack.ToString());
            JSONNode n1 = new JSONObject();
           
            node.Add("oData", n1);
            print("<color=green> " + SocketServerEventType.reqPack.ToString()+ "</color>");
            SocketHandler.Instance.root.ExpectAcknowledgement<object>(OnreqPlayerPack_Callback).Emit(
                GameManager.instance.TableId,
                new { sEventName = SocketServerEventType.reqPack.ToString(), oData = node.ToString()});
        }
        else
        {
            SocketHandler.Instance.root.Manager.Open();
        }
        
        return 0;
    }

    private void OnreqPlayerPack_Callback(object obj)
    {
        Debug.Log("OnreqPlayerPack_Callback :");
        
        OnPlayerMakeAMove?.Invoke(0, Move.Pack, this);
        transform.DOScale(initalScale, 0.4f);
        playerStatus = PlayerStatus.Packed;
        playerData.bCardSeen = false;
        holder.ToggleOpenCloseCards(false);
    }

    public void SeeCards()
    {
        if (SocketHandler.Instance.root.IsOpen)
        {
            JSONNode node = new JSONObject();
            node.Add("sEventName", SocketServerEventType.reqCardSeen.ToString());
            JSONNode n1 = new JSONObject();
           
            node.Add("oData", n1);
            print("<color=green> " + SocketServerEventType.reqCardSeen.ToString()+ "</color>");
            SocketHandler.Instance.root.ExpectAcknowledgement<object>(OnreqPlayerSeeHand_Callback)
                .Emit(GameManager.instance.TableId,
                    new { sEventName = SocketServerEventType.reqCardSeen.ToString(), oData = node.ToString()});
        }
        else
        {
            SocketHandler.Instance.root.Manager.Open();
        }
    }

    private void OnreqPlayerSeeHand_Callback(object obj)
    {
        Debug.Log("OnreqPlayerSeeHand_Callback :");
        seeCardBtn.gameObject.SetActive(false);
        playerStatus = PlayerStatus.Seen;
        playerData.bCardSeen = true;
        if (playerData.iUserId.Equals(GameManager.instance.myPlayerId))
        {
            TableManager.instance.HaveIOpenedMyCards(playerData.bCardSeen);
            seeCardBtn.gameObject.SetActive(false);
        }
        
        holder.ToggleOpenCloseCards(true);
    }

    public int Show()
    {
        Debug.Log("Show Called by Player");
        eventPlayedLbl.text = "SHOW";
        eventPlayedLbl.enabled = true;
        TableManager.instance.gameUI.HideButtonsToPlayer();
        if (TableManager.instance.currentPlayer !=null)
        {
            TickTimer.OnUpdateTick -= TableManager.instance.OnUpdateTick;
            TickTimer.OnTick -= TableManager.instance.OnTick;
            TableManager.instance.currentPlayer.ResetTimer();
        }
        
        if (SocketHandler.Instance.root.IsOpen)
        {
            JSONNode node = new JSONObject();
            node.Add("sEventName", SocketServerEventType.reqShowHand.ToString());
            JSONNode n1 = new JSONObject();
            
            node.Add("oData", n1);
            SocketHandler.Instance.root.ExpectAcknowledgement<object>(OnreqPlayerShow_Callback)
                .Emit(GameManager.instance.TableId, 
                    new { sEventName = SocketServerEventType.reqShowHand.ToString(), oData = node.ToString()});
        }
        else
        {
            SocketHandler.Instance.root.Manager.Open();
        }
        
        return Pot.currentBet;
    }

    private void OnreqPlayerShow_Callback(object obj)
    {
        Debug.Log("OnreqPlayerShow_Callback :");
       
        MakeABet(TableManager.instance.currentTurn.nCurrentBootValue);
    }

    

    private int SideShow()
    {
        TableManager.instance.gameUI.HideButtonsToPlayer();
        if (TableManager.instance.currentPlayer !=null)
        {
            TickTimer.OnUpdateTick -= TableManager.instance.OnUpdateTick;
            TickTimer.OnTick -= TableManager.instance.OnTick;
            TableManager.instance.currentPlayer.ResetTimer();
        }
        return 0;
    }
    public int Call()
    {
        Debug.Log("Call Called by Player");
        eventPlayedLbl.text = "CALL";
        eventPlayedLbl.enabled = true;
        TableManager.instance.gameUI.HideButtonsToPlayer();
        if (TableManager.instance.currentPlayer !=null)
        {
            TickTimer.OnUpdateTick -= TableManager.instance.OnUpdateTick;
            TickTimer.OnTick -= TableManager.instance.OnTick;
            TableManager.instance.currentPlayer.ResetTimer();
        }
        if (SocketHandler.Instance.root.IsOpen)
        {
            JSONNode node = new JSONObject();
            node.Add("sEventName", SocketServerEventType.reqBet.ToString());
            JSONNode n1 = new JSONObject();
            n1.Add("bCardSeen", playerData.bCardSeen);//Change Bool Value According to player Hand Status
            n1.Add("b2XBet", false);
            node.Add("oData", n1);
            SocketHandler.Instance.root.ExpectAcknowledgement<object>(OnreqPlayerCall_Callback)
                .Emit(GameManager.instance.TableId, 
                    new { sEventName = SocketServerEventType.reqBet.ToString(), oData = node.ToString()});
        }
        else
        {
            SocketHandler.Instance.root.Manager.Open();
        }
        
        return Pot.currentBet;
    }

    private void OnreqPlayerCall_Callback(object obj)
    {
        Debug.Log("OnreqPlayerCall_Callback :");
        
        OnPlayerMakeAMove?.Invoke(TableManager.instance.currentTurn.nCurrentBootValue, Move.Call, this);
        MakeABet(TableManager.instance.currentTurn.nCurrentBootValue);
    }

    public int Raise()
    {
        Debug.Log("Raise Called by Player");
        eventPlayedLbl.text = "RAISE";
        eventPlayedLbl.enabled = true;
        TableManager.instance.gameUI.HideButtonsToPlayer();
        if (TableManager.instance.currentPlayer !=null)
        {
            TickTimer.OnUpdateTick -= TableManager.instance.OnUpdateTick;
            TickTimer.OnTick -= TableManager.instance.OnTick;
            TableManager.instance.currentPlayer.ResetTimer();
        }
        
        if (SocketHandler.Instance.root.IsOpen)
        {
            JSONNode node = new JSONObject();
            node.Add("sEventName", SocketServerEventType.reqBet.ToString());
            JSONNode n1 = new JSONObject();
            n1.Add("bCardSeen", playerData.bCardSeen);//Change Bool Value According to player Hand Status
            n1.Add("b2XBet", true);
            node.Add("oData", n1);
            SocketHandler.Instance.root.ExpectAcknowledgement<object>(OnreqPlayerRaise_Callback)
                .Emit(GameManager.instance.TableId, 
                    new { sEventName = SocketServerEventType.reqBet.ToString(), oData = node.ToString()});
        }
        else
        {
            SocketHandler.Instance.root.Manager.Open();
        }
       
        return Pot.currentBet;
    }

    private void OnreqPlayerRaise_Callback(object obj)
    {
        Debug.Log("OnreqPlayerRaise_Callback :");
        OnPlayerMakeAMove?.Invoke(TableManager.instance.currentTurn.nCurrentBootValue, Move.Raise, this);
        Pot.currentBet *= 2;

        MakeABet(TableManager.instance.currentTurn.nCurrentBootValue);
        
    }

    public int Blind()
    {
        Debug.Log("Blind Called by Player");
        eventPlayedLbl.text = "BLIND";
        eventPlayedLbl.enabled = true;
        TableManager.instance.gameUI.HideButtonsToPlayer();
        if (TableManager.instance.currentPlayer !=null)
        {
            TickTimer.OnUpdateTick -= TableManager.instance.OnUpdateTick;
            TickTimer.OnTick -= TableManager.instance.OnTick;
            TableManager.instance.currentPlayer.ResetTimer();
        }
        
        if (SocketHandler.Instance.root.IsOpen)
        {
            JSONNode node = new JSONObject();
            node.Add("sEventName", SocketServerEventType.reqBet.ToString());
            JSONNode n1 = new JSONObject();
            n1.Add("bCardSeen", playerData.bCardSeen);//Change Bool Value According to player Hand Status
            n1.Add("b2XBet", false);
            node.Add("oData", n1);
            SocketHandler.Instance.root.ExpectAcknowledgement<object>(OnreqPlayerBlind_Callback)
                .Emit(GameManager.instance.TableId, 
                    new { sEventName = SocketServerEventType.reqBet.ToString(), oData = node.ToString()});
        }
        else
        {
            SocketHandler.Instance.root.Manager.Open();
        }
        
        return Pot.currentBlindBet;
    }

    private void OnreqPlayerBlind_Callback(object obj)
    {
        Debug.Log("OnreqPlayerBlind_Callback :");
        
        OnPlayerMakeAMove?.Invoke(0, Move.Blind, this);
        MakeABet(TableManager.instance.currentTurn.nCurrentBootValue);
    }

    public int BlindRaise()
    {
        Debug.Log("Blind Raise Called by Player");  
        eventPlayedLbl.text = "2XBlind";
        eventPlayedLbl.enabled = true;
        TableManager.instance.gameUI.HideButtonsToPlayer();
        if (TableManager.instance.currentPlayer !=null)
        {
            TickTimer.OnUpdateTick -= TableManager.instance.OnUpdateTick;
            TickTimer.OnTick -= TableManager.instance.OnTick;
            TableManager.instance.currentPlayer.ResetTimer();
        }
        
        if (SocketHandler.Instance.root.IsOpen)
        {
            JSONNode node = new JSONObject();
            node.Add("sEventName", SocketServerEventType.reqBet.ToString());
            JSONNode n1 = new JSONObject();
            n1.Add("bCardSeen", playerData.bCardSeen);//Change Bool Value According to player Hand Status
            n1.Add("b2XBet", true);
            node.Add("oData", n1);
            SocketHandler.Instance.root.ExpectAcknowledgement<object>(OnreqPlayerBlindRaise_Callback)
                .Emit(GameManager.instance.TableId, 
                    new { sEventName = SocketServerEventType.reqBet.ToString(), oData = node.ToString()});
        }
        else
        {
            SocketHandler.Instance.root.Manager.Open();
        }
       
        return Pot.currentBlindBet;
    }

    private void OnreqPlayerBlindRaise_Callback(object obj)
    {
        Debug.Log("OnreqPlayerBlindRaise_Callback :");
        
        OnPlayerMakeAMove?.Invoke(TableManager.instance.currentTurn.nCurrentBootValue, Move.Raise, this);
        Pot.currentBlindBet *= 2;

        MakeABet(TableManager.instance.currentTurn.nCurrentBootValue);
    }


    public IEnumerator ReturnCardRoutine(float returnTime = 0.1f, float interval = 0.1f)
    {
       currentDealer.DeckCardHolder.SetArrangementTime(returnTime);

        holder.ToggleOpenCloseCards(false);
        
        foreach (CardDisplay cardDisplay in holder.cards)
        {
            CardHolder.MoveToHolder(cardDisplay, currentDealer.DeckCardHolder);

            yield return new WaitForSeconds(interval);
        }
    }



    public void WinPot(int chips)
    {
        Balance += chips;
    }
    
    public void ResetTimer()
    {
        Debug.Log("Resetting Timer");
        TableManager.instance.turnTime = 0;
        TableManager.instance.totalTime = 0;
        timer.fillAmount = 0;
        
    }
    
   
}

[Serializable]
public class PlayerCardsList
{
    public List<Card> cards;
}

public enum Move
{
    Blind,
    Call,
    Raise,
    Pack,
    SideShow,
    Show
}

