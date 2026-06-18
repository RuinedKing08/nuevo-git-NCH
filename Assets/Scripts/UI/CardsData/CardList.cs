using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[CreateAssetMenu(fileName = "CardList", menuName = "Lists/NewCardList")]
public class CardList : ScriptableObject
{
    public List<CardData> cardsData = new List<CardData>();
    public CardData GetCardByID(string id)
    {
        return cardsData.Find(card => card.id == id);
    }
}
