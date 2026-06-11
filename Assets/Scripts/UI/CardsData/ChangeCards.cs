using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChangeCards : MonoBehaviour
{
    GameObject cardSelection;
    [SerializeField] List<CardView> cardViews;
    [SerializeField] List<CardData> CD;
    void Start()
    {
        Waves.Instance.OnChangeWave += OpenCardSelection;
        cardSelection = transform.Find("CardsSelection").gameObject;
        cardSelection.SetActive(false);
    }

    private void OpenCardSelection()
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
            cardViews[i].CardSetUp(CD[Random.Range(0, CD.Count)]);
        }
        //Debug.Log("Bro");
        Time.timeScale = 0f;
    }
}
