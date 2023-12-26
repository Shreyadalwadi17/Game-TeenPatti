using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardColors", menuName = "Card/Card Colors")]
public class CardColorData : ScriptableObject
{
    [Header("Front")]
    public List<CardColors> Colors;
    [Header("Back")]
    public Sprite BackDesign;
}


[System.Serializable]
public class CardColors {
    public Suit suit;
    public Color color;
    public Sprite SuitSprite;
}
