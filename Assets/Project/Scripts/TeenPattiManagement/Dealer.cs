using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class Dealer : MonoBehaviour
{
    [Header("Player Data")]
    public GameObject PlayersHolder;
    public List<Player> Players;


    [Header("Player Data")]
    public CardHolder DeckCardHolder;
    public ObjectPoolController cardPool;

    [Header("Pot")]
    public Pot currentPot;


    [Header("Ugly")]
    public TextMeshProUGUI PotText;



    [Header("Events")]
    public UnityAction OnRoundStarted;
    public UnityAction OnRoundEnded;
    public UnityAction<Player> OnWinnerDetermined;
    public UnityAction<Player> OnTurnStart;
    public UnityAction<int> OnTurnEnd;
    public UnityAction OnCycleComplete;
    public UnityAction<Player> OnAPlayerCalledShow;


    private int turnIndex;
    private int CycleCount;

    Deck deck;

    public static bool AllowShow;
   public static Dealer instance;   
    private void Awake()
    {
        instance=this;
        DeckCardHolder = GetComponentInChildren<CardHolder>();
        // Players = (PlayersHolder.GetComponentsInChildren<Player>()).ToList();
    }

    private void Start()
    {
        //Initialize();
        //TableManager.instance.delaer = this;
    }


    public void Initialize()
    {
         Players = (PlayersHolder.GetComponentsInChildren<Player>()).ToList();
        deck = new Deck();
        currentPot = new Pot();

        foreach (var player in Players)
        {
            player.AddBalance(1000);
        }

        StartCoroutine(StartRoundRoutine());
    }

    public void ResetDealer()
    {
        turnIndex = 0;
        CycleCount = 0;
        AllowShow = false;
        Pot.currentBet = 10;
        Pot.currentBlindBet = Pot.currentBet / 2;
        currentPot.ResetPot();
    }

    public void ResetDeck()
    {
        deck.Shuffle();

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


    private IEnumerator StartRoundRoutine()
    {
        ResetDealer();

        ResetDeck();

        foreach (var player in Players)
        {
            player.OnPlayerMakeAMove += OnPlayerMadeMove;
            //player.Initialize(this);
            player.MakeABet(Pot.currentBet);
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
        EndRound();
    }

    private void StartPlayerTurn()
    {

        int activePlayersCount = 0;

        foreach (Player player in Players)
        {
            if (player.playerStatus != PlayerStatus.Packed)
            {
                activePlayersCount++;
            }
        }


        if (activePlayersCount == 2)
        {
            AllowShow = true;
        }

        if (activePlayersCount == 1)
        {
            // AllowShow = true;
            EndRound();
            return;
        }

        Player currentPlayer = Players[turnIndex];

        Debug.LogError("before on start called");
        if (currentPlayer.playerStatus != PlayerStatus.Packed)
        {
            Debug.LogError("on start called");
            OnTurnStart?.Invoke(currentPlayer);
        }
        else
        {
            MoveToNextPlayer();
        }
    }


    public Player DetermineWinner()
    {
        List<Player> ActivePlayerList = new List<Player>();

        foreach (var player in Players)
        {
            if (player.playerStatus != PlayerStatus.Packed)
            {
                ActivePlayerList.Add(player);
            }
        }

        Player winningPlayer = ActivePlayerList[0];

        for (int playerIndex = 1; playerIndex < ActivePlayerList.Count; playerIndex++)
        {

            if (winningPlayer.hand.CompareHand(ActivePlayerList[playerIndex].hand) != 1)
            {
                winningPlayer = ActivePlayerList[playerIndex];
            }
        }

        Debug.Log(winningPlayer.playerData.sUserName + " " + winningPlayer.hand.GetHandType());


        OnWinnerDetermined?.Invoke(winningPlayer);

        return winningPlayer;

    }


    public Player GetCurrentPlayer()
    {
        return Players[turnIndex];
    }

    public void EndRound()
    {
        StartCoroutine(RoundEndRoutine());
    }

    IEnumerator RoundEndRoutine()
    {
        DetermineWinner();

        yield return StartCoroutine(CleanupRoutine());
    }




    IEnumerator CleanupRoutine()
    {
        List<CardDisplay> cards = new List<CardDisplay>();

        foreach (var player in Players)
        {
            yield return StartCoroutine(player.ReturnCardRoutine());
            cards.AddRange(player.holder.cards);
            player.holder.CleanUp();
            yield return new WaitForSeconds(0.5f);
        }


        cards.AddRange(DeckCardHolder.cards);

        foreach (CardDisplay cardDisplay in cards)
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

        yield return new WaitForSeconds(2);

        yield return StartCoroutine(StartRoundRoutine());
    }

}




