using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardDisplayForWinner : MonoBehaviour
{
    [Header("Card Design Data")]
    public CardColorData cardColorData;
    
    [Header("Card Info")]
    public Card card;
    
    [Header("Front Side of Card1")]
    public GameObject FrontFace;
    public TextMeshProUGUI rankText;
    public Image SuitSprite;
    public Image SuitMiniSprite;
    
    public void SetCard(Card card)
    {
        this.card = card;

        SuitSprite.sprite = cardColorData.Colors.Find(x => x.suit == card.suit).SuitSprite;
        SuitMiniSprite.sprite = cardColorData.Colors.Find(x => x.suit == card.suit).SuitSprite;
        rankText.color = cardColorData.Colors.Find(x => x.suit == card.suit).color;


        switch (card.rank)
        {
            case Rank.Two:
                rankText.text = "2";
                break;

            case Rank.Three:
                rankText.text = "3";
                break;

            case Rank.Four:
                rankText.text = "4";
                break;

            case Rank.Five:
                rankText.text = "5";
                break;

            case Rank.Six:
                rankText.text = "6";
                break;

            case Rank.Seven:
                rankText.text = "7";
                break;

            case Rank.Eight:
                rankText.text = "8";
                break;

            case Rank.Nine:
                rankText.text = "9";
                break;

            case Rank.Ten:
                rankText.text = "10";
                break;

            case Rank.Jack:
                rankText.text = "J";
                break;

            case Rank.Queen:
                rankText.text = "Q";
                break;

            case Rank.King:
                rankText.text = "K";
                break;

            case Rank.Ace:
                rankText.text = "A";
                break;

        }

    }
}
