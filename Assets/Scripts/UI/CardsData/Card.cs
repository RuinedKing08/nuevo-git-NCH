using UnityEngine;
using TMPro;
public class Card
{
    public readonly CardData cardData;

    public string title;
    public string description;
    public Sprite sprite;
    public CardType type;
    public float amount;
    public float extraMaxLife;
    public float prob;
    public bool canAcumulate;
    public bool canChangeMaxLife;
    public Color backgroundColor;
    public TMP_ColorGradient textColor;
    public Card(CardData _cardData)
    {
        cardData = _cardData;
        title = cardData.title;
        description = cardData.description;
        type = cardData.type;
        sprite = cardData.artwork;
        amount = cardData.amount;
        extraMaxLife = cardData.extraMaxLife;
        prob = cardData.prob;
        canAcumulate = cardData.canAcumulate;
        backgroundColor = cardData.backgroundColor;
        textColor = cardData.textColor;
        canChangeMaxLife = cardData.canChangeMaxLife;
    }
}
