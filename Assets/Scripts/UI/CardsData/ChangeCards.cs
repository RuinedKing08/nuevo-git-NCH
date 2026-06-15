using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class ChangeCards : MonoBehaviour
{
    GameObject cardSelection;
    [SerializeField] List<CardView> cardViews;
    [SerializeField] List<CardList> cards;
    
    void Start()
    {
        Waves.Instance.OnChangeWave += OpenCardSelection;
        CardConfirm.Instance.OnChangeCards += OpenCardSelection;
        Exp.Instance.OnFullExp += OpenCardSelection;
        cardSelection = transform.Find("CardsSelection").gameObject;
        cardSelection.SetActive(false);
    }
    public void OpenCardSelection()
    {
        if (Waves.Instance.GetWave() % 1 == 0 && Waves.Instance.GetWave() != 0)
        {
            cardSelection.SetActive(true);
            CreateCards();
        }
    }
    void CreateCards()
    {
        //Debug.Log("XDDDDDD");
        for (int i = 0; i < cardViews.Count; i++)
        {
            /*Debug.Log("creating card");
            Debug.Log(cardViews[i]);
            Debug.Log(CD[i]);*/
            cardViews[i].CardSetUp(ChoseRandomCard());
        }
        //Debug.Log("Bro");
        Time.timeScale = 0f;
    }
    CardData ChoseRandomCard()
    {
        CardList CL = cards[Random.Range(0, cards.Count)];
        int prob = Random.Range(1, 101);
        int level = 3;
        if (prob > 40) level = 1;
        else if (prob > 15) level = 2;

        CardData randomCard = CL.cardsData.FirstOrDefault(card => card.level == level);
        if (CL.cardsData.Count > 0 && randomCard == null) { return CL.cardsData[0]; }

        return randomCard;

        if (prob > 40)
        {
            foreach(CardData card in CL.cardsData)
            {
                if(card.level == 1)
                {
                    return card;
                }
            }
        }
        else if (prob > 15)
        {
            foreach (CardData card in CL.cardsData)
            {
                if (card.level == 2)
                {
                    return card;
                }
            }
        }
        else
        {
            foreach (CardData card in CL.cardsData)
            {
                if (card.level == 3)
                {
                    return card;
                }
            }
        }
        if(CL.cardsData.Count > 0) { return CL.cardsData[0]; }

        return null;
    }
}
