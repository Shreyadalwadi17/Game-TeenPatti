using System.Collections.Generic;

public class Hand
{
    private List<Card> cards;
    public HandType handType;

    public Hand(List<Card> cards)
    {
        this.cards = cards;
        handType = GetHandType();
    }


    public HandType GetHandType()
    {
        if (IsTrail())
        {
            return HandType.Trail;
        }
        else if (IsPureSequence())
        {
            return HandType.PureSequence;
        }
        else if (IsSequence())
        {
            return HandType.Sequence;
        }
        else if (IsColor())
        {
            return HandType.Color;
        }
        else if (IsPair())
        {
            return HandType.Pair;
        }
        else
        {
            return HandType.HighCard;
        }
    }

    /* private bool IsTrail()
     {
         return (cards[0].rank == cards[1].rank && cards[1].rank == cards[2].rank);
     }*/

    public bool IsTrail()
    {
        // Check if all cards have the same rank
        Rank firstCardRank = cards[0].rank;
        for (int i = 1; i < cards.Count; i++)
        {
            if (cards[i].rank != firstCardRank)
            {
                return false;
            }
        }

        return true;
    }

    /*    private bool IsPureSequence()
        {
            if (cards[0].suit == cards[1].suit && cards[1].suit == cards[2].suit)
            {
                if ((cards[2].rank - cards[1].rank == 1) && (cards[1].rank - cards[0].rank == 1))
                {
                    return true;
                }
            }
            return false;
        }*/

    public bool IsPureSequence()
    {
        // Sort the cards by rank
        cards.Sort((a, b) => a.rank.CompareTo(b.rank));

        // Check if all cards are of the same suit
        Suit firstCardSuit = cards[0].suit;
        for (int i = 1; i < cards.Count; i++)
        {
            if (cards[i].suit != firstCardSuit)
            {
                return false;
            }
        }

        // Check if all cards form a sequence
        for (int i = 0; i < cards.Count - 1; i++)
        {
            if (cards[i + 1].rank - cards[i].rank != 1)
            {
                return false;
            }
        }

        return true;
    }


    /*    private bool IsSequence()
        {
            if ((cards[2].rank - cards[1].rank == 1) && (cards[1].rank - cards[0].rank == 1))
            {
                return true;
            }
            return false;
        }*/

    private bool IsSequence()
    {
        cards.Sort((a, b) => a.rank.CompareTo(b.rank));

        // Check if all cards form a sequence
        for (int i = 0; i < cards.Count - 1; i++)
        {
            if (cards[i + 1].rank - cards[i].rank != 1)
            {
                return false;
            }
        }

        return true;
    }

    /*private bool IsColor()
    {
        return (cards[0].suit == cards[1].suit && cards[1].suit == cards[2].suit);
    }*/

    private bool IsColor()
    {
        cards.Sort((a, b) => a.rank.CompareTo(b.rank));

        Suit firstCardSuit = cards[0].suit;
        for (int i = 1; i < cards.Count; i++)
        {
            if (cards[i].suit != firstCardSuit)
            {
                return false;
            }
        }

        return true;
    }

    /*    private bool IsPair()
        {
            if (cards[0].rank == cards[1].rank || cards[1].rank == cards[2].rank || cards[0].rank == cards[2].rank)
            {
                return true;
            }
            return false;
        }*/

    public bool IsPair()
    {
        // Count the number of cards with each rank
        Dictionary<Rank, int> rankCount = new Dictionary<Rank, int>();
        foreach (Card card in cards)
        {
            if (rankCount.ContainsKey(card.rank))
            {
                rankCount[card.rank]++;
            }
            else
            {
                rankCount[card.rank] = 1;
            }
        }

        // Check if there is a pair
        foreach (int count in rankCount.Values)
        {
            if (count >= 2)
            {
                return true;
            }
        }

        return false;
    }


    public int CompareHand(Hand otherHand)
    {
        HandType thisHandType = GetHandType();
        HandType otherHandType = otherHand.GetHandType();

        if (thisHandType > otherHandType)
        {
            return 1;
        }
        else if (thisHandType < otherHandType)
        {
            return -1;
        }
        else
        {
            // Hands are of the same type, compare based on ranks
            List<Rank> thisHandRanks = GetSortedRanks();
            List<Rank> otherHandRanks = otherHand.GetSortedRanks();

            for (int i = 0; i < thisHandRanks.Count; i++)
            {
                if (thisHandRanks[i] > otherHandRanks[i])
                {
                    return 1;
                }
                else if (thisHandRanks[i] < otherHandRanks[i])
                {
                    return -1;
                }
            }

            // All ranks are equal, hands are tied
            return 0;
        }
    }

    private List<Rank> GetSortedRanks()
    {
        List<Rank> ranks = new List<Rank>();
        foreach (Card card in cards)
        {
            ranks.Add(card.rank);
        }
        ranks.Sort();
        ranks.Reverse(); // Sort in descending order
        return ranks;
    }

    public void Clear()
    {
        cards.Clear();
        handType = HandType.None;
    }
}

public enum HandType
{
    None,
    HighCard,
    Pair,
    Color,
    Sequence,
    PureSequence,
    Trail
}
