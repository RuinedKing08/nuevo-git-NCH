using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using TMPro;
public class ChangeCards : MonoBehaviour
{
    GameObject cardSelection;
    [SerializeField] List<CardView> cardViews;
    [SerializeField] List<CardList> cards;
    [SerializeField] TMP_Text extraUpgradesLeftTMP;
    [SerializeField] GameObject startTimeTMP;
    [SerializeField] TMP_Text timeTMP; 
    public event ChangeWaves OnChangeWave;
    public static ChangeCards Instance;
    public delegate void ChangeWaves();
    public static bool cardsOpened;
    public static bool cardsFinished;
    public bool cardsClosed;
    bool waveFinished;
    [SerializeField] float timer;
    private void Awake()
    {
        Instance = this;
        cardSelection = transform.Find("CardsSelection").gameObject;
        cardSelection.SetActive(true);
        cardsOpened = false;
    }
    void Start()
    {
        Waves.Instance.OnChangeWave += OpenCardSelection;
        CardConfirm.Instance.OnChangeCards += CreateCards;
        //Exp.Instance.OnFullExp += OpenCardSelection;
        cardSelection.SetActive(false);
        startTimeTMP.SetActive(false);
        waveFinished = false;
       InputsParent.Instance.InteractionInput().performed += ctx => OpenCardsHUD();
    }
    public void OpenCardSelection()
    {
        if (Waves.Instance.GetWave() % 1 == 0 && Waves.Instance.GetWave() != 0)
        {
            cardsFinished = false;
            startTimeTMP.SetActive(true);
            waveFinished = true;
        }
    }
    public void InvokeChangeWave() { OnChangeWave?.Invoke(); }
    private void Update()
    {
        TimeToNextWave();
    }
    void TimeToNextWave()
    {
        if (waveFinished)
        {
            timer -= Time.deltaTime;
            timeTMP.text = $"{Mathf.RoundToInt(timer)}";
        }
        if(timer <= 0)
        {
            if (!cardsFinished) Exp.Instance.ChangeFloatExtraUpgrade(1);
            InvokeChangeWave();
            cardsOpened = false;
            cardsClosed = false;
            startTimeTMP.SetActive(false);
            waveFinished = false;
            timer = 10f;
        }
    }
    void OpenCardsHUD()
    {
        if (!cardsFinished)
        {
            if (waveFinished && !cardsOpened)
            {
                cardsOpened = true;
                CreateCards();
            }
            else if (cardsClosed)
            {
                cardsClosed = false;
                Time.timeScale = 0f;
                cardSelection.SetActive(true);
            }
            else if (cardsOpened && !cardsClosed)
            {
                cardsClosed = true;
                Time.timeScale = 1f;
                Debug.Log("cards se cierra");
                cardSelection.SetActive(false);
            }
        }
    }
    void CreateCards()
    {
        //Debug.Log("XDDDDDD");
        CardData[] saves = new CardData[3];

        saves[0] = ChoseRandomCard();
        saves[1] = ChoseRandomCard();
        saves[2] = ChoseRandomCard();
        while (saves[0].title == saves[1].title || saves[1].title == saves[2].title || saves[0].title == saves[2].title)
        {
            saves[0] = ChoseRandomCard();
            saves[1] = ChoseRandomCard();
            saves[2] = ChoseRandomCard();
        }
        
        for (int i = 0; i < cardViews.Count; i++)
        {
            /*Debug.Log("creating card");
            Debug.Log(cardViews[i]);
            Debug.Log(CD[i]);*/
            
            cardViews[i].CardSetUp(saves[i]);
            
        }
        //Debug.Log("Bro");
        cardSelection.SetActive(true);
        Time.timeScale = 0f;
        extraUpgradesLeftTMP.text = $": {Exp.Instance.GetExtraUpdrageFloat()}";
    }
    CardList RandomCard()
    {
        CardList CL = cards[Random.Range(0, cards.Count)];
        return CL;
    }
    CardData ChoseRandomCard()
    {
        CardList CL = RandomCard();
        int prob = Random.Range(1, 101);
        int level = 3;
        if (prob > 40) level = 1;
        else if (prob > 15) level = 2;
        CardData randomCard = CL.cardsData.FirstOrDefault(card => card.level == level);
        /*if(randomCard.type == CardType.Extra)
        {
            while(randomCard.level != 3)
            {
                CL = RandomCard();
                prob = Random.Range(1, 101);
                level = 3;
                if (prob > 40) level = 1;
                else if (prob > 15) level = 2;
                randomCard = CL.cardsData.FirstOrDefault(card => card.level == level);
            }
        }*/
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
