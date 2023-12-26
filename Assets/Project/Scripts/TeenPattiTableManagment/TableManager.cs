using System;
using System.Collections;
using System.Collections.Generic;
using BaseFramework;
using BestHTTP.SocketIO;
using DG.Tweening;
using Newtonsoft.Json;
using SimpleJSON;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;
using System.Linq;
public class TableManager : IndestructibleSingleton<TableManager>
{
    [Header("Player Data")] 
    public Player PlayerPrefab;
    public List<Transform> PlayersHolders;
    public List<Player> Players;
    
    public Player currentPlayer;

    [Header("Player Data")]
    public CardHolder DeckCardHolder;
    public ObjectPoolController cardPool;

    [Header("Pot")]
    public Pot currentPot;

    [Header("Table Timer")]
    public TextMeshProUGUI gameStartinmgTimer;
    
    [Header("Table Pot Text")] 
    public TextMeshProUGUI PotText;

    public Button backBtn; 
    public GameUI gameUI;
    public WinnerPopupUI winnerPopup;
    public CurrentTurn currentTurn;

    public int maxPotLimit;
    public int currentPotValue;
    public PotData potData;
    public List<GameObject> chips;
    public Transform potCenter;
    [Header("Events")]
    public UnityAction OnRoundStarted;
    public UnityAction OnRoundEnded;
    public UnityAction<Player> OnWinnerDetermined;
    public UnityAction<Player> OnTurnStart;
    public UnityAction<Player> OnTurnEnd;
    public UnityAction OnCycleComplete;
    public UnityAction<Player> OnAPlayerCalledShow;

    public const string WinnerTag = "winner";

    private int turnIndex;
    private int CycleCount;
    
    public float totalTime;
    public float turnTime;
    protected int turnTick;
    int SeatOffset;



    public TableDetails tableDetails;
    public ReconnectTableAndPlayer reconnectingData;
    Deck deck;

    public static bool AllowShow;
    

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
            case API_TYPE.API_boardJoin:
                Debug.Log("TableManager" + response);
                Preloader.instance.Hide();
                break;
        }
    }
    
    #region Player Actions for Game

    public void PackByCurrentPlayer()
    {
        if (currentPlayer.playerData.iUserId.Equals(GameManager.instance.myPlayerId))
        {
            currentPlayer.Pack();
        }
    }
    public void ShowByCurrentPlayer()
    {
        if (currentPlayer.playerData.iUserId.Equals(GameManager.instance.myPlayerId))
        {
            currentPlayer.Show();
        }
    }
    public void SeeHandByCurrentPlayer()
    {
        if (currentPlayer.playerData.iUserId.Equals(GameManager.instance.myPlayerId))
        {
            currentPlayer.SeeCards();
        }
    }
    public void BlindByCurrentPlayer()
    {
        if (currentPlayer.playerData.iUserId.Equals(GameManager.instance.myPlayerId))
        {
            currentPlayer.Blind();
        }
    }
    public void BlindRaiseByCurrentPlayer()
    {
        if (currentPlayer.playerData.iUserId.Equals(GameManager.instance.myPlayerId))
        {
            currentPlayer.BlindRaise();
        }
    }
    public void RaiseByCurrentPlayer()
    {
        if (currentPlayer.playerData.iUserId.Equals(GameManager.instance.myPlayerId))
        {
            currentPlayer.Raise();
        }
    }
    public void CallByCurrentPlayer()
    {
        if (currentPlayer.playerData.iUserId.Equals(GameManager.instance.myPlayerId))
        {
            currentPlayer.Call();
        }
    }
    
    public void SideShowByCurrentPlayer()
    {
        if (currentPlayer.playerData.iUserId.Equals(GameManager.instance.myPlayerId))
        {
            //currentPlayer.SideShow();
        }
    }
    #endregion

    
    
    public void Initialize()
    {
        deck = new Deck(tableDetails.aClosedDeck);
        currentPot = new Pot();
        currentPot.AddToPot(currentPotValue);

        /*foreach (var player in Players)
        {
            player.AddBalance(1000);
        }*/
        Debug.Log("Creating Deck and Setting Pot value");
        StartCoroutine(StartRoundRoutine());
    }

   
    

    public void ResetDealer()
    {
        turnIndex = 0;
        CycleCount = 0;
        AllowShow = false;
        Pot.currentBet = 0;
        Pot.currentBlindBet = Pot.currentBet / 2;
        currentPot.ResetPot();
    }

    public void ResetDeck()
    {
        //deck.Shuffle();
        List<CardDisplay> cards = new List<CardDisplay>();

        foreach (Card card in deck.deckcards)
        {
            CardDisplay spawnedCard = cardPool.SpawnObject();
            spawnedCard.SetCard(card);
            cards.Add(spawnedCard);
        }

        DeckCardHolder.SetArrangementTime(0);

        DeckCardHolder.Initialize(cards);
    }

    public void ExitGame()
    {
        //Do Exit Functionality.
        foreach (Player player in Players)
        {
            player.ResetPlayer();
        }
        SocketHandler.Instance.RemoveTableListener(GameManager.instance.TableId);
        SocketHandler.Instance.CloseAllConnections();
        ServicesHandler.instance.CallGetProtoList();
        StartCoroutine(LoadScene(1));
    }
    IEnumerator LoadScene(int sceneNum)
    {
        yield return new WaitForSeconds(1f);
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneNum);
        asyncOperation.allowSceneActivation = false;
        while (!asyncOperation.isDone)
        {
            //Output the current progress
            //ProgressBar.value = asyncOperation.progress;

            if (asyncOperation.progress >= 0.9f)
            {
                yield return new WaitForSeconds(0.5f);
               
                asyncOperation.allowSceneActivation = true;
            }

            yield return null;
        }
    }
    public IEnumerator DistributeCards(int cardCountPerPlayer, float distributionInterval = 0.1f, float CycleInterval = 0.4f)
    {
        yield return new WaitForSeconds(1f);

        int index = 0;

        for (int i = 0; i < cardCountPerPlayer; i++)
        {
            for (int j = 0; j < Players.Count; j++)
            {
                yield return new WaitForSeconds(distributionInterval);
                CardHolder.MoveToHolder(DeckCardHolder.GetCardFromTop(), Players[index].holder);

                index++;

                if (index == Players.Count)
                {
                    index = 0;
                    yield return new WaitForSeconds(CycleInterval);
                }
            }
        }

        foreach (Player player in Players)
        {
            player.OnCardsRecieved();
        }
    }


    //public IEnumerator DistributeCardsToPlayer(int cardCountPerPlayer, float distributionInterval = 0.1f, float CycleInterval = 0.4f)
    //{

    //    yield return new WaitForSeconds(1f);

    //    int cardCount = cardCountPerPlayer * Players.Count;
    //    int index = 0;
    //    DeckCardHolder.myPlayerCardIndex = 0;

    //    for (int i = 0; i < cardCount; i++)
    //    {
    //        yield return new WaitForSeconds(distributionInterval);

    //        CardHolder.MoveToHolder(DeckCardHolder.GetCardFromPlayerHandList(Players[index]), Players[index++].holder);

    //        if (index == Players.Count)
    //        {
    //            yield return new WaitForSeconds(CycleInterval);
    //            index = 0;
    //        }
    //    }


    //    foreach (Player player in Players)
    //    {
    //        player.OnCardsRecieved();
    //    }
    //}

    public IEnumerator StartRoundRoutine()
    {

        ResetDealer();

        ResetDeck();

        foreach (var player in Players)
        {

            player.OnPlayerMakeAMove += OnPlayerMadeMove;
            player.Initialize(this);
            player.MakeABet(0);
        }

        StartPlayerTurn();
        yield return StartCoroutine(DistributeCards(3));
    }
    private void OnPlayerMadeMove(int amount, Move move, Player player)
    {
        Debug.Log(amount);
        currentPot.AddToPot(amount);
        PotText.text = "Pot Value : "+currentPot.totalPot.ToString();

        switch (move)
        {
            case Move.Show:
                APlayerCalledShow(player);
                break;
        }

        MoveToNextPlayer();
    }

    public void MoveToNextPlayer()
    {
OnTurnEnd?.Invoke(currentPlayer);
        turnIndex++;

        turnIndex = turnIndex % Players.Count;


        if (turnIndex == 0)
        {
            OnCycleComplete?.Invoke();
            CycleCount++;
        }

        StartPlayerTurn();

    }

    private void APlayerCalledShow(Player player)
    {
        OnAPlayerCalledShow?.Invoke(player);
        //EndRound();
    }

    private void StartPlayerTurn()
    {

        Debug.LogError("start player turn");
        foreach (var player in Players)
        {
            if (currentTurn.iUserId.Equals(player.playerData.iUserId))
            {
                currentPlayer = player;
                HaveIOpenedMyCards(currentPlayer.playerData.bCardSeen);
                if (currentPlayer != null)
                {
                    currentPlayer.timer.color = Color.green;
                    turnTime = currentTurn.ttl/1000;
                    totalTime = currentTurn.nTotalTurnTime/1000;
                    turnTick = (int)currentTurn.ttl;
                    
                    TickTimer.OnUpdateTick += OnUpdateTick;
                    TickTimer.OnTick += OnTick;
                }
                else
                {
                    Debug.Log("Current player Not Found");
                }
                
            }
        }

        if (currentPlayer.playerStatus != PlayerStatus.Packed)
        {
            
            OnTurnStart?.Invoke(currentPlayer);
        }
        else
        {
            MoveToNextPlayer();
        }
    }

    public void HaveIOpenedMyCards(bool playerDataBCardSeen)
    {
        if (!playerDataBCardSeen)
        {
            Debug.Log("player Card Seen "+ playerDataBCardSeen);
            gameUI.callBtn.SetActive(false);
            gameUI.raiseBtn.SetActive(false);
            gameUI.blindBtn.SetActive(true);
            gameUI.blindRaiseBtn.SetActive(true);
        }
        else
        {
            Debug.Log("player Card Seen "+ playerDataBCardSeen);
            gameUI.blindBtn.SetActive(false);
            gameUI.blindRaiseBtn.SetActive(false);
            gameUI.callBtn.SetActive(true);
            gameUI.raiseBtn.SetActive(true);

        }
    }


    public Player DetermineWinner()
    {
        Player winningPlayer = new Player();
        
        foreach (var player in Players)
        {
            if (player.playerData.eState == "winner")
            {
                Debug.Log("Winning PLayer ="+player.playerData.sUserName);
                winningPlayer = player;
                OnWinnerDetermined?.Invoke(player);
            }
        }
       return winningPlayer;
    }


    public Player GetCurrentPlayer()
    {
        return Players[turnIndex];
    }

    public void EndRound()
    {
        Debug.Log("End Round");
        StartCoroutine(RoundEndRoutine());
    }

    IEnumerator RoundEndRoutine()
    {
        yield return StartCoroutine(CleanupRoutine());
    }
    
    IEnumerator CleanupRoutine()
    {
        PotText.text = "0";
        currentPot.ResetPot();
        foreach (var player in Players)
        { 
            yield return StartCoroutine(player.ReturnCardRoutine());
            player.holder.CleanUp();
            yield return new WaitForSeconds(0.5f);
        }

        foreach (CardDisplay cardDisplay in DeckCardHolder.cards)
        {
            cardPool.ReturnObject(cardDisplay);
        }
        DeckCardHolder.CleanUp();
        currentPot.ResetPot();

        foreach (Player player in Players)
        {
            player.ResetPlayer();
            player.OnPlayerMakeAMove -= OnPlayerMadeMove;
        }

    }

    #region Socket Handler Methods

    public void SetUserHand(string data)
    {
        foreach (Player player in Players)
        {
            if (player.playerData.iUserId.Equals(GameManager.instance.myPlayerId))
            {
                player.playerData.aHand = JsonHelper1.FromJsonArray<AClosedDeck>(data);
                break;

            }
        }
    }

    public void ChangePlayerStates(string data)
    {
        JSONNode node = JSONNode.Parse(data);
        foreach (var player in Players)
        {
            if (GameManager.instance.myPlayerId.Equals(node["iUserId"]))
            {
                player.playerData.eState = node["eState"];
            }
        }
    }

    public void PlayerTurnMissed(string data)
    {
        JSONNode node = JSONNode.Parse(data);
        foreach (var player in Players)
        {
            if (GameManager.instance.myPlayerId.Equals(node["iUserId"]))
            {
                player.playerData.nTurnMissed = node["nTurnMissed"];
            }
        }
    }

    public void PlayerLeft(string data)
    {
        JSONNode node = JSONNode.Parse(data);
        foreach (var player in Players)
        {
            if (player.playerData.iUserId.Equals(node["iUserId"]))
            {
               //Show Msg of player left
               Debug.Log("player :"+ player.playerData.sUserName +" has Left the Game.");
               Debug.Log("Reason : " + node["sReason"]);
               Players.Remove(player);
            }
        }
    }

    public void ShowResult(string data)
    {
        tableDetails.aParticipant = JsonHelper1.FromJsonArray<AParticipant>(data);
        Debug.LogError("the count of participants "+tableDetails.aParticipant);
        for (int i = 0; i < tableDetails.aParticipant.Length; i++)
        {
            if (tableDetails.aParticipant[i].eState == "winner")
            {
                var player = Players.Find(x => x.playerData.iUserId == tableDetails.aParticipant[i].iUserId);
                player.playerData.eState = tableDetails.aParticipant[i].eState;
                /*player.playerData.handType = tableDetails.aParticipant[i].currentHandRank;*/
                player.playerData.handType = player.hand.GetHandType().ToString();
                SetWinnerPopup(DetermineWinner());
            }
            else if (tableDetails.aParticipant[i].eState == "packed")
            {
                var player = Players.Find(x => x.playerData.iUserId == tableDetails.aParticipant[i].iUserId);
                player.eventPlayedLbl.text = tableDetails.aParticipant[i].eState;
            }
        }
    }

    public void SetWinnerPopup(Player winningPlayer)
    {
        winnerPopup.playerNameLbl.text = winningPlayer.playerData.sUserName;
        winnerPopup.cardTypeLbl.text = winningPlayer.playerData.handType;
        
        int index = 0;
        // Display Cards of Players who are playing
        foreach (CardDisplayForWinner card in winnerPopup.cards)
        {
            card.SetCard(winningPlayer.holder.cards[index++].card);
            card.FrontFace.SetActive(true);
        }
    }

    public void HideWinner()
    {
        winnerPopup.playerNameLbl.text = String.Empty;
        winnerPopup.cardTypeLbl.text = String.Empty;
        winnerPopup.gameObject.SetActive(false);
       
    }
    public void ShowWinner()
    {
        winnerPopup.gameObject.SetActive(true);
        Invoke("HideWinner",4f);
    }

    // public void GeneratePlayer(string data)
    // {
    //      int secondseatnumber=2;  
    //     int movethreshold=0; 
    //     // iu have to chanfge here only
    //     // int secondPlayerSeatNumber=2;
    //     JSONNode node = JSONNode.Parse(data);
        
    //     int seatNumber = node["nSeat"];
       
    //     if (GameManager.instance.myPlayerId.Equals(node["iUserId"]))
    //     {
    //         seatNumber = 2;
    //     }
    //     if(!Players.Find(p => p.name == node["sUserName"]))
    //     {
    //     Player player = Instantiate(PlayerPrefab,PlayersHolders[seatNumber]);
        
    //     player.playerData = JsonUtility.FromJson<PlayerData>(data);
    //     player.name = player.playerData.sUserName;
    //     Debug.Log("This player -> "+player.name+" Joined");
    //     Players.Add(player);
    //     }
    //     SeatOffset = tableDetails.aParticipant.Length; 
    //     int movethreshold=secondseatnumber-player.nSeat; 
    //     foreach (AParticipant participant in tableDetails.aParticipant)
    //     {
           
    //         if (GameManager.instance.myPlayerId.Equals(participant.iUserId))
    //         {
    //             Debug.Log("This player -> "+participant.sUserName+" was already in");
             
    //         }
    //         else
    //         {
    //             if (!Players.Find(x=> x.playerData.iUserId.Equals(participant.iUserId)) )
    //             {
                    
    //                 Debug.Log("Creating new player because it was not in the Players List");
    //                 int placementId = participant.nSeat - SeatOffset;
    //                 int seatIndex = placementId < 0 ? 5 + placementId : placementId;
    //                 int newseatIndex=participant
    //                 // Player  newPlayer = Instantiate(PlayerPrefab,PlayersHolders[participant.nSeat]);
    //                 Player  newPlayer = Instantiate(PlayerPrefab,PlayersHolders[participant.nSeat]);
    //                 newPlayer.playerData = JsonUtility.FromJson<PlayerData>(JsonUtility.ToJson(participant));
    //                 newPlayer.name = newPlayer.playerData.sUserName;
    //                 Debug.LogError("the player name instantiated"+newPlayer.transform.name);
    //                 Debug.Log("This player -> "+newPlayer.name+" Joined");
    //                 Players.Add(newPlayer);
    //             }
    //         }
    //     }

        
    // }
    public void GeneratePlayer(string data)
    {
         int secondseatnumber=2;  
        int movethreshold=0; 
        // iu have to chanfge here only
        // int secondPlayerSeatNumber=2;
        JSONNode node = JSONNode.Parse(data);
        
        int seatNumber = node["nSeat"];
       
        if (GameManager.instance.myPlayerId.Equals(node["iUserId"]))
        {
            seatNumber = 2;
        }
        if(!Players.Find(p => p.name == node["sUserName"]))
        {
        Player player = Instantiate(PlayerPrefab,PlayersHolders[seatNumber]);
        Debug.LogError("here in instantiarte");
        player.playerData = JsonUtility.FromJson<PlayerData>(data);
        player.name = player.playerData.sUserName;
        Debug.Log("This player -> "+player.name+" Joined");
        movethreshold=secondseatnumber-player.playerData.nSeat; 
        Players.Add(player);
        }
        SeatOffset = tableDetails.aParticipant.Length; 
        foreach (AParticipant participant in tableDetails.aParticipant)
        {
           
            if (GameManager.instance.myPlayerId.Equals(participant.iUserId))
            {
                Debug.Log("This player -> "+participant.sUserName+" was already in");
             
            }
            else
            {
                if (!Players.Find(x=> x.playerData.iUserId.Equals(participant.iUserId)) )
                {
                    
                    Debug.Log("Creating new player because it was not in the Players List");
                    // int seatIndex = placementId < 0 ? 5 + placementId : placementId;
                    int newseatIndex=participant.nSeat+movethreshold;
                    if(newseatIndex<0)
                    {
                        newseatIndex=PlayersHolders.Count +newseatIndex;
                    }
                    else if(newseatIndex>=PlayersHolders.Count)
                    {
                        newseatIndex=newseatIndex-PlayersHolders.Count;
                    }
                    Debug.LogError("the new index "+newseatIndex);
                    Player  newPlayer = Instantiate(PlayerPrefab,PlayersHolders[newseatIndex]);
                    newPlayer.playerData = JsonUtility.FromJson<PlayerData>(JsonUtility.ToJson(participant));
                    newPlayer.name = newPlayer.playerData.sUserName;
                    Debug.LogError("the player name instantiated"+newPlayer.transform.name);
                    Debug.Log("This player -> "+newPlayer.name+" Joined");
                    Players.Add(newPlayer);
                }
            }
        }

        
    }



    public void SetTableDetails(string data)
    {
        Debug.LogError("set table details");
        JSONNode node = JSONNode.Parse(data);
        tableDetails = JsonUtility.FromJson<TableDetails>(node.ToString());
        maxPotLimit = tableDetails.nBoardPotLimit;
        currentPotValue = tableDetails.nBootValue;
        Pot.currentBet = tableDetails.nCurrentBootValue;
        currentPot.AddToPot(currentPotValue);
        PotText.text = "Pot Value : "+currentPot.totalPot.ToString();
        // SeatOffset = tableDetails.aParticipant.Length;
        // foreach (AParticipant participant in tableDetails.aParticipant)
        // {
        //     if (GameManager.instance.myPlayerId.Equals(participant.iUserId))
        //     {
        //         Debug.Log("This player -> "+participant.sUserName+" was already in");
        //     }
        //     else
        //     {
        //         if (!Players.Find(x=> x.playerData.iUserId.Equals(participant.iUserId)))
        //         {
        //             Debug.Log("Creating new player because it was not in the Players List");
        //             int placementId = participant.nSeat - SeatOffset;
        //             int seatIndex = placementId < 0 ? 5 + placementId : placementId;
                    
        //             Player  newPlayer = Instantiate(PlayerPrefab,PlayersHolders[participant.nSeat]);
    
        //             newPlayer.playerData = JsonUtility.FromJson<PlayerData>(JsonUtility.ToJson(participant));
        //             newPlayer.name = newPlayer.playerData.sUserName;
        //             Debug.LogError("the player name "+newPlayer.transform.name);
        //             Debug.Log("This player -> "+newPlayer.name+" Joined");
        //             Players.Add(newPlayer);
        //         }
        //     }
        // }

        Debug.Log("Player count = "+Players.Count);
        if (tableDetails.nMaxPlayer == Players.Count)
        {
            Debug.Log("Tabel has reached maximum capacity of Player");
            Initialize();
        }
        
    }

    public void SetGameStartingTimer(string data)
    {
        JSONNode node = JSONNode.Parse(data);
        float timer = node["value"];
        
        if (timer >= 0)
        {
            gameStartinmgTimer.gameObject.SetActive(true);
            gameStartinmgTimer.text = timer.ToString();
            gameStartinmgTimer.transform.DOScale(Vector3.one * 1.2f, 0.5f).OnComplete(() =>
            {
                gameStartinmgTimer.transform.DOScale(Vector3.one,0.5f);
                gameStartinmgTimer.gameObject.SetActive(false);
            });
        }
        else
        {
            gameStartinmgTimer.gameObject.SetActive(false);
        }
        
    }

    public void SetPlayerTurn(string data)
    {
        JSONNode node = JSONNode.Parse(data);
        currentTurn = JsonUtility.FromJson<CurrentTurn>(node.ToString());
        
        StartPlayerTurn();
    }

    public void ShowHandForEveryOne(string data)
    {
        
        tableDetails.aParticipant = JsonHelper1.FromJsonArray<AParticipant>(data);
        TickTimer.OnUpdateTick -= OnUpdateTick;
        TickTimer.OnTick -= OnTick;
        if (currentPlayer !=null)
        {
            currentPlayer.ResetTimer();
        }
         //Find Player to change Data from above callback 
        foreach (Player player in Players)
        {
            //Find Data of player
            if (player.playerData.iUserId.Equals(GameManager.instance.myPlayerId))
            {
             //   return;
            }
            else
            {
                foreach (AParticipant participant in tableDetails.aParticipant)
                {
                
                    //Should not be me 
                    if (!participant.iUserId.Equals(GameManager.instance.myPlayerId))
                    {
                        //Finding for player card details
                        /*Debug.Log("player.holder.cards.Count = "+player.holder.cards.Count);*/
                        for (int i = 0; i < player.holder.cards.Count; i++)
                        {
                            /*Debug.Log($"Setting card {i} Data for Other Player");

                            Debug.Log($"Player Holder card: Suit "+ player.holder.cards[i].card.suit+" Rank "+player.holder.cards[i].card.rank);
                            Debug.Log("Card Which will be set into player Holder cards : Suit"+(Suit)Enum.Parse(typeof(Suit),participant.aHand[i].suit)+
                                      " Rank "+deck.FindRank(participant.aHand[i].rank));*/
                            player.holder.cards[i].card = new Card(deck.FindRank(participant.aHand[i].rank),
                                (Suit)Enum.Parse(typeof(Suit),participant.aHand[i].suit),
                                participant.aHand[i].value,
                                participant.aHand[i]._id);
                            
                            DeckCardHolder.RemoveCard(player.holder.cards[i]);
                        }
                    }
                }
                
                foreach (CardDisplay cardDisplay in player.holder.cards)
                {
                    cardDisplay.SetCard(cardDisplay.card, false, player.holder);
                }

                player.hand = new Hand(player.holder.GetCards());
            }
        }
        
        foreach (var player in Players)
        {
            player.SeeCards();
        }
       
    }
    
    public void SetPotValue(string data)
    {
        potData = JsonUtility.FromJson<PotData>(data);
        if (currentPlayer.playerData.iUserId.Equals(potData.iUserId))
        {
            currentPlayer.eventPlayedLbl.text = potData.sHandState;
            currentPlayer.eventPlayedLbl.enabled = true;
        }
        
        currentPotValue = potData.nBoardPotValue;
        currentPot.AddToPot(currentPotValue);
        PotText.text = "Pot Value : "+currentPot.totalPot.ToString();
    }
    
    public void ResetTableForNewRound(string data)
    {
        EndRound();
    }
    public void SetCardSeenLable(JSONNode node)
    {
        string uId = node["oData"]["iUserId"].ToString();
        Debug.Log("player id => "+uId);
        foreach (Player player in Players)
        {
            if (player.playerData.iUserId == uId)
            {
                Debug.Log("CardSeen by this player ="+ player.playerData.sUserName);
                player.cardSeenLbl.gameObject.SetActive(true);
            }
        }
        
    }
    
    public void RejoinGame(string data)
    {
        reconnectingData = JsonUtility.FromJson<ReconnectTableAndPlayer>(data);
        SetReconnectedTable();// For Setting Table Details.
        SetReconnectedPlayer();// For Setting players.
        SetReconnectedPlayerTimer();// For Setting Timer of Current player if game is not Finished.
    }

    public void SetReconnectedTable()
    {
        //setting Data as classes don't match.
        tableDetails._id = reconnectingData.oTable._id;
        tableDetails.iProtoId = reconnectingData.oTable.iProtoId;
        tableDetails.nBoardPotLimit = reconnectingData.oTable.nBoardPotLimit;
        tableDetails.eState = reconnectingData.oTable.eState;
        tableDetails.iUserTurn = reconnectingData.oTable.iUserTurn;
        tableDetails.nBoardPotValue = reconnectingData.oTable.nBoardPotValue;
        tableDetails.nBoardPotValue = reconnectingData.oTable.nBootValue;
        tableDetails.nCurrentBootValue = reconnectingData.oTable.nCurrentBootValue;
        tableDetails.nMaxBootValue = reconnectingData.oTable.nMaxBootValue;
    }

    public void SetReconnectedPlayer()
    {
        tableDetails.aParticipant = JsonHelper1.FromJsonArray<AParticipant>(JsonUtility.ToJson(reconnectingData.aParticipants));
    }

    public void SetReconnectedPlayerTimer()
    {
        
    }

    #endregion
    
    public void OnUpdateTick()
    {
        if (turnTime > 0f)
        {
            turnTime -= Time.deltaTime;
            if (currentPlayer == null) return;

            currentPlayer.timer.fillAmount = turnTime / totalTime;//clamp01(turnTime, 0f, 1);
            
            if (currentPlayer.timer.fillAmount < 0.25f)
            {
                Color c = Color.red;
                c.a = Mathf.PingPong(Time.time * 2, 0.8f);
                currentPlayer.timer.color = c;
            }
            if (turnTime < 0.5f && currentPlayer.playerData.iUserId.Equals(GameManager.instance.myPlayerId))
            {
                turnTime = 0;
            }
        }
        else
        {
            //Fold();
            turnTime = 0f;
        }
        
    }
    
    public void OnTick()
    {
        if (turnTick > 0)
        {
            turnTick--;
            if (turnTick <= 2)
            {
                //msg
            }
        }
        else
        {
            turnTick = 0;
            currentPlayer.ResetTimer();
        }
    }
    public float clamp01(float val, float min, float max)
    {
        return (val - min) / (max - min);
    }


   
}
public static class JsonHelper1
{
    public static T[] FromJsonArray<T>(string json)
    {
        string newJson = "{ \"Items\": " + json + "}";
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
        return wrapper.Items;
    }
    [Serializable]
    private class Wrapper<T>
    {
        public T[] Items;
    }
}

#region Table settings

[Serializable]
public class AClosedDeck
{
    public string suit;
    public string rank;
    public int value;
    public string _id;
    public int nGroupId;
}

[Serializable]
    public class AParticipant
    {
        public string iUserId;
        public int nSeat;
        public string eUserType;
        public string sUserName;
        public int nChips;
        public string sRootSocket;
        public string eState;
        public int nTurnMissed;
        public AClosedDeck[] aHand;
        public bool bCardSeen;
        public string currentHandRank;
    }

[Serializable]
    public class TableDetails
    {
        public string _id;
        public List<AClosedDeck> aClosedDeck;
        public string eBoardType;
        public string iProtoId;
        public string iUserTurn;
        public int nBoardFee;
        public int nMaxPlayer;
        public OSetting oSetting;
        public int nAmountIn;
        public int nAmountOut;
        public List<int> aWinningAmount;
        public string eState;
        public int nBoardPotLimit;
        public int nBoardPotValue;
        public int nBootValue;
        public int nCurrentBootValue;
        public int nMaxBootValue;
        public AParticipant[] aParticipant;
    }

[Serializable]
    public class OSetting
    {
        public int nCardDistributionDelay;
        public int nBeginCountdown;
        public int nDistributeCardAnimationDelay;
        public int nAnimationCountdown;
        public int nInitializeTimer;
        public int nMaxWaitingTime;
        public int nMaxTurnMissAllowed;
        public int nTurnBuffer;
        public int nTurnTime;
    }


[Serializable]
    public class Details
    {
        public TableDetails oData;
    }

#endregion

#region Turn

[Serializable]
public class CurrentTurn
{
    public string iUserId;
    public float ttl;
    public float nTotalTurnTime;
    public int nCurrentBootValue;
}

#endregion

#region Pot

[Serializable]
public class PotData
{
    public string iUserId;
    public int nCurrentBootValue;
    public int nBoardPotValue;
    public string sHandState;
}

#endregion

#region Reconnection


[Serializable]
public class OTable
{
    public string _id;
    public string iProtoId;
    public object aWinner;
    public int nBoardPotLimit;
    public string eState; 
    public string iUserTurn;
    public int nBoardPotValue;
    public int nBootValue;
    public int nCurrentBootValue;
    public int nMaxBootValue;
}
[Serializable]
public class ReconnectTableAndPlayer
{
    public TurnInfo turnInfo;
    public OTable oTable;
    public AParticipant[] aParticipants;
}
[Serializable]
public class TurnInfo
{
    public object aWinner;
    public object nBoardPotLimi;
}

#endregion