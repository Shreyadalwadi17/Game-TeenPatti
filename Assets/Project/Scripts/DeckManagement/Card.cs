
[System.Serializable]
public class Card
{
    public Rank rank;
    public Suit suit;
    public int value;
    public string id;

    public Card(Rank rank, Suit suit, int value, string id)
    {
        this.rank = rank;
        this.suit = suit;
        this.value = value;
        this.id = id;
    }
    
}
