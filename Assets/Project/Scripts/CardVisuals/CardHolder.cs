using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class CardHolder : MonoBehaviour
{
    [Header("Arrangement Configration")]
    [Range(0f, 0.15f)]
    public float cardSpacing = 0.2f;

    [Range(0, 30f)]
    public float cardRotation;

    public List<CardDisplay> cards;

    public UnityAction<CardDisplay> OnCardAdded;
    public UnityAction<CardDisplay> OnCardRemoved;
    public int myPlayerCardIndex;
    public TextMeshProUGUI srengthText;


    public void Initialize(List<CardDisplay> Cards)
    {
        cards = new List<CardDisplay>(Cards);

        foreach (CardDisplay card in cards)
        {
            card.SetCardParent(transform);
        }

        ArrangeCards();
    }

    public void AddCard(CardDisplay cardDisplay)
    {
        if (cards == null)
        {
            cards = new List<CardDisplay>();
        }

        cardDisplay.SetCardParent(transform);
        cards.Add(cardDisplay);
        ArrangeCards();
    }

    float arrangementTime = 0.5f;
    public void SetArrangementTime(float arrangementTime = 0.5f)
    {
        this.arrangementTime = arrangementTime;
    }

    public CardDisplay RemoveCard(CardDisplay cardDisplay)
    {
        cards.Remove(cardDisplay);
        OnCardRemoved?.Invoke(cardDisplay);
        ArrangeCards();


        return cardDisplay;
    }

    public void SortCards()
    {
        cards.Sort((card1, card2) =>
        {
            if (card1.card.rank == card2.card.rank)
            {
                return card1.card.suit.CompareTo(card2.card.suit);
            }
            else
            {
                return card1.card.rank.CompareTo(card2.card.rank);
            }
        });

        ArrangeCards();

    }

    private void ArrangeCards()
    {
        int numCards = cards.Count;

        // Calculate the rotation angle for each card
        float angleBetweenCards = cardRotation;

        if (numCards > 2)
        {
            angleBetweenCards = 2 * cardRotation / (numCards - 1);
        }


        for (int i = 0; i < cards.Count; i++)
        {
            Vector3 cardPosition = new Vector3(i * cardSpacing, 0f, (-i * 0.1f) - 0.1f);

            // Set card rotation
            float cardAngle = (i - (numCards - 1) / 2f) * angleBetweenCards;
            if (numCards % 2 == 0 && i == numCards / 2)
            {
                cardAngle = 0f; // Center the middle card if there are an even number of cards
            }

            Quaternion cardRotation = Quaternion.Euler(new Vector3(0, 0, -cardAngle));
            cards[i].transform.DOLocalMove(cardPosition, arrangementTime).SetEase(Ease.OutCubic);
            cards[i].transform.DOLocalRotateQuaternion(cardRotation, arrangementTime).SetEase(Ease.OutCubic);
            cards[i].transform.DOScale(Vector3.one, arrangementTime).SetEase(Ease.OutCubic);
        }
    }

    public void ToggleOpenCloseCards(bool IsOpen)
    {
        foreach (CardDisplay cardDisplay in cards)
        {
            cardDisplay.FlipCard(IsOpen);
        }
    }


    public List<Card> GetCards()
    {
        List<Card> tempcards = new List<Card>();

        foreach (var card in cards)
        {
            tempcards.Add(card.card);
        }

        return tempcards;
    }

    public CardDisplay GetCardFromTop()
    {
        if (cards.Count > 0)
        {
            return RemoveCard(cards.Last());
        }

        return null;
    }

    public CardDisplay GetCardFromPlayerHandList(Player player)
    {
        //Get the Card From Player Data and Find the Card in deck pick and Distribute the the card from deck 

        if (cards.Count > 0)
        {
            if (player.playerData.iUserId.Equals(GameManager.instance.myPlayerId))
            {
                if (player.playerData.aHand.Length > myPlayerCardIndex)
                {
                    CardDisplay card = cards.Find(x => x.card.id == player.playerData.aHand[myPlayerCardIndex]._id);
                    myPlayerCardIndex++;
                    return RemoveCard(card);
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return RemoveCard(CreateEmptyCard());
            }
        }

        return null;
    }

    public CardDisplay CreateEmptyCard()
    {
        CardDisplay spawnedCard = TableManager.instance.cardPool.SpawnObject();
        spawnedCard.FlipCard(false);
        cards.Add(spawnedCard);
        return spawnedCard;
    }

    public static void MoveToHolder(CardDisplay card, CardHolder destinationHolder)
    {
        if (card != null)
        {
            destinationHolder.AddCard(card);
        }
    }

    public void CleanUp()
    {
        cards = null;
    }

    // strength of the hand
    public string EvaluateHand()
    {
        SortCards();

        if (IsTrail()) return "Trail";
        if (IsPair()) return "Pair";
        if (IsSequence()) return "Sequence";
        if (IsPureSequence()) return "Pure Sequence";
        if (IsFlush()) return "Color";

        return "High Card";
    }

    public void DisplayHandEvaluation()
    {
        string result = EvaluateHand();
        srengthText.text = result.ToString();
    }

    private bool IsSequence()
    {
        for (int i = 0; i < cards.Count - 1; i++)
        {
            if (cards[i].card.rank + 1 != cards[i + 1].card.rank) return false;
        }
        return true;
    }

    private bool IsPureSequence()
    {
        if (cards.Count < 3) return false; 

        List<CardDisplay> sortedBySuit = cards.OrderBy(c => c.card.suit).ThenBy(c => c.card.rank).ToList();

        int consecutiveCount = 1;
        for (int i = 0; i < sortedBySuit.Count - 1; i++)
        {
      
            if ((sortedBySuit[i].card.suit == sortedBySuit[i + 1].card.suit) &&
                (sortedBySuit[i].card.rank + 1 == sortedBySuit[i + 1].card.rank))
            {
                consecutiveCount++;
                if (consecutiveCount >= 3) return true;
            }
            else
            {
                consecutiveCount = 1; 
            }
        }
        return false;
    }


    private bool IsFlush()
    {
        var suit = cards[0].card.suit;
        return cards.All(cardDisplay => cardDisplay.card.suit == suit);
    }
    private bool IsTrail()
    {
        for (int i = 0; i < cards.Count - 2; i++) 
        {
            if (cards[i].card.rank == cards[i + 1].card.rank && cards[i].card.rank == cards[i + 2].card.rank)
                return true;
        }
        return false;
    }

    private bool IsPair()
    {
        for (int i = 0; i < cards.Count - 1; i++)
        {
            for (int j = i + 1; j < cards.Count; j++)
            {
                if (cards[i].card.rank == cards[j].card.rank)
                    return true;
            }
        }
        return false;
    }

}
