using UnityEngine;
using TMPro;
[CreateAssetMenu(fileName = "CardData", menuName = "Cards/New Card")]
public class CardData : ScriptableObject
{
    public string id;
    public string title;
    public string description;
    public CardType type;
    public float amount;
    public float extraMaxLife;
    public float prob;
    public bool canAcumulate;
    public bool canChangeMaxLife;
    public Color backgroundColor;
    public Sprite artwork;
    public TMP_ColorGradient textColor;
}
