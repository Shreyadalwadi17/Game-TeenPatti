using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class CardDisplay : MonoBehaviour
{
    [Header("Card Design Data")]
    public CardColorData cardColorData;

    [Header("Card Info")]
    public Card card;

    [Header("Display References")]
    public bool isFaceUp;
    [Header("Front Side")]
    public GameObject FrontFace;
    public TextMeshPro rankText;
    public SpriteRenderer SuitSprite;
    public SpriteRenderer SuitMiniSprite;

    [Header("Back Side")]
    public GameObject BackFace;
    public SpriteRenderer BackSprite;



    private SpriteRenderer[] renderers;
    private TextMeshPro[] texts;

    private void Awake()
    {
        renderers = GetComponentsInChildren<SpriteRenderer>();
        texts = GetComponentsInChildren<TextMeshPro>();
    }

    /* private void Start()
     {
         SetCardIndex();
     }*/

    public void SetCard(Card card, bool faceUp = false, CardHolder holder = null)
    {
        this.card = card;

        SuitSprite.sprite = cardColorData.Colors.Find(x => x.suit == card.suit).SuitSprite;
        SuitMiniSprite.sprite = cardColorData.Colors.Find(x => x.suit == card.suit).SuitSprite;
        rankText.color = cardColorData.Colors.Find(x => x.suit == card.suit).color;



        if (holder != null)
        {
            SetCardParent(holder.transform);
        }

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

        BackSprite.sprite = cardColorData.BackDesign;

        SetCardFace(faceUp);
    }

    public void SetCardParent(Transform newParent = null)
    {
        transform.SetParent(newParent);
    }

    [ContextMenu("Flip Card")]
    public void FlipCard(bool _isFaceup, float flippingTime = 0.05f)
    {
        isFaceUp = _isFaceup;

        if (isFaceUp)
        {
            FrontFace.transform.localScale = new Vector3(0, 1, 1);
            FrontFace.transform.DOScaleX(1, flippingTime).OnComplete(() =>
            {
                SetCardFace(isFaceUp);
            });
            BackFace.transform.DOScaleX(0, flippingTime);

        }
        else
        {
            BackFace.transform.localScale = new Vector3(0, 1, 1);
            BackFace.transform.DOScaleX(1, flippingTime).OnComplete(() =>
            {
                SetCardFace(isFaceUp);
            });
            FrontFace.transform.DOScaleX(0, flippingTime);

        }
    }

    public void SetCardFace(bool faceUp)
    {
        isFaceUp = faceUp;

        FrontFace.SetActive(isFaceUp);
        BackFace.SetActive(!isFaceUp);
    }

    public void ResetCard()
    {
        SetCardFace(false);
    }
}
