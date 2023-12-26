using System;
using System.Collections.Generic;

[System.Serializable]
public class Deck
{
    public List<Card> deckcards;

    public Deck()
    {

        deckcards = new List<Card>();

        for (int i = 2; i <= 14; i++)
        {
            deckcards.Add(new Card((Rank)i, Suit.Hearts,i,null));
            deckcards.Add(new Card((Rank)i, Suit.Diamonds,i,null));
            deckcards.Add(new Card((Rank)i, Suit.Clubs,i,null));
            deckcards.Add(new Card((Rank)i, Suit.Spades,i,null));
        }

        Shuffle();
    }
    public Deck(List<AClosedDeck> aClosedDeck)
    {

        deckcards = new List<Card>();

        for (int i = 0; i < aClosedDeck.Count; i++)
        {
            deckcards.Add(new Card(FindRank(aClosedDeck[i].rank),
                (Suit)Enum.Parse(typeof(Suit),aClosedDeck[i].suit),
                aClosedDeck[i].value,
                aClosedDeck[i]._id));
        }
        
    }

    public Rank FindRank(string letter)
    {
        if (letter == "J")
        {
            return Rank.Jack;
        }
        else if (letter == "Q")
        {
            return Rank.Queen;
        }
        else if (letter == "K")
        {
            return Rank.King;
        }
        else if (letter == "A")
        {
            return Rank.Ace;
        }
        else
        {
            return (Rank)Enum.Parse(typeof(Rank), letter);
        }
        
    }

    public void Shuffle()
    {
        for (int i = 0; i < deckcards.Count; i++)
        {
            int randomIndex = new Random().Next(i, deckcards.Count);

            Card temp = deckcards[i];
            deckcards[i] = deckcards[randomIndex];
            deckcards[randomIndex] = temp;
        }
    }

    public List<Card> DrawCards(int Count)
    {
        List<Card> cards = new List<Card>();

        for (int cardCount = 0; cardCount < Count; cardCount++)
        {
            Card card = deckcards[0];
            deckcards.RemoveAt(0);
            cards.Add(card);
        }

        return cards;
    }


    public List<Card>[] DistributeCards(int playerCount, int cardCountForEachPlayer)
    {
        List<Card>[] playerHands = new List<Card>[playerCount];

        for (int i = 0; i < playerCount; i++)
        {
            playerHands[i] = new List<Card>();
        }

        // Deal cards to each player
        for (int playerIndex = 0; playerIndex < playerCount; playerIndex++)
        {
            for (int cardCount = 0; cardCount < cardCountForEachPlayer; cardCount++)
            {
                Card card = deckcards[0];
                deckcards.RemoveAt(0);
                playerHands[playerIndex].Add(card);
            }
        }

        return playerHands;
    }

}
