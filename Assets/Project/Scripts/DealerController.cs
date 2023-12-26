using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;

public class DealerController : MonoBehaviour
{

    public List<Player> players;
    public List<Player> Players;
    public CardDisplay cardDisplay;
    public ObjectPoolController cardPool;

    public CardHolder DeckCardHolder;

    public GameObject PlayersHolder;

    Dealer dealer;

    IEnumerator Start()
    {
        players = (PlayersHolder.GetComponentsInChildren<Player>()).ToList();

        //dealer = new Dealer(players);

        Debug.Log("In Dealer Controller");
        List<CardDisplay> cards = new List<CardDisplay>();

       /* foreach (Card card in dealer.deck.deckcards)
        {
            CardDisplay spawnedCard = cardPool.SpawnObject();
            spawnedCard.SetCard(card);
            cards.Add(spawnedCard);
        }
*/
        DeckCardHolder.Initialize(cards);

        DeckCardHolder.SetArrangementTime(0);

        yield return new WaitForSeconds(2f);

        yield return StartCoroutine(DistributeCards(3));

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



    /* yield return new WaitForSeconds(10f);


     Player Winner = dealer.DetermineWinner();

     Winner.holder.transform.DOScale(Vector3.one * 1.3f, 0.2f).SetLoops(20,LoopType.Yoyo);

     yield return new WaitForSeconds(5f);

     SceneManager.LoadSceneAsync(0).allowSceneActivation = true;*/
}

