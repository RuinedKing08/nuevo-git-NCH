using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
public class CardView : MonoBehaviour
{
    [SerializeField]Card card;
    [SerializeField] CardView[] cardViews;
    [SerializeField] PlayerHealth player;
    [SerializeField] GameObject cardSelection;
    [SerializeField] private Image sr;
    [SerializeField] private Image frame;
    [SerializeField] private Image highlight;
    [SerializeField] private TMP_Text title;
    [SerializeField] private TMP_Text description;
    [SerializeField] float timer, maxTimer;
    Color initialHighlightColor;
    [SerializeField] Color highlightColor;
    bool animateFrame;
    private void Awake()
    {
        player = GameObject.FindWithTag("Player").GetComponent<PlayerHealth>();
        frame = GetComponent<Image>();
        sr = transform.Find("Artwork").GetComponent<Image>();
        title = transform.Find("Title").GetComponent<TMP_Text>();
        description = transform.Find("Description").GetComponent<TMP_Text>();
        highlight = transform.Find("Highlight").GetComponent<Image>();
        cardSelection = GameObject.Find("Canvas").transform.Find("CardsSelection").gameObject;
        cardViews = FindObjectsByType<CardView>(FindObjectsSortMode.None);
    }
    void Start()
    {
        initialHighlightColor = highlight.color;
        highlight.gameObject.SetActive(false);
    }
    private void Update()
    {
        if (animateFrame)
        {
            Animate();
        }
    }
    public void CardSetUp(CardData CD)
    {
        card = new Card(CD);
        //if (CD != null) Debug.Log("yo card" + CD);
        frame.color = card.backgroundColor;
        sr.sprite = card.sprite;
        title.text = card.title;
        title.colorGradientPreset = card.textColor;
        description.text = card.description;
        description.colorGradientPreset = card.textColor;
    }

    public void Effects()
    {
        switch (card.type)
        {
            case CardType.Attack:
                Debug.Log("Mejora de ataque +" +card.amount);
                break;
            case CardType.Healing:
                player.GetLife(card.amount * player.MaxHealth / 100);
                break;
            case CardType.Protection:
                Debug.Log("Mejora de protección");
                break;
            case CardType.Score:
                Debug.Log($"Mejora de puntaje antes: {Currency.Instance.GetScoreMultiplier()}");
                Currency.Instance.ChangeScoreMultiplier(card.amount / 100);
                Debug.Log($"Mejora de puntaje ahora: {Currency.Instance.GetScoreMultiplier()}");
                break;
        }
        if (card.canChangeMaxLife)
        {
            Debug.Log("vida max antes: " + player.MaxHealth);
            player.GetMaxLife(card.extraMaxLife);
            player.GetLife(card.extraMaxLife);
            Debug.Log("vida max ahora: " + player.MaxHealth);
        }
        for (int i = 0; i < cardViews.Length; i++)
        {
            cardViews[i].animateFrame = false;
            cardViews[i].frame.fillAmount = 1;
        }
        Time.timeScale = 1;
        cardSelection.SetActive(false);
    }
    public void OnMouseEnter()
    {
        //Debug.Log("Mouse Encima");
        animateFrame = true;
    }
    public void OnMouseExit()
    {
        frame.fillAmount = 1;
        animateFrame = false;
        highlight.color = initialHighlightColor;
        highlight.gameObject.SetActive(false);
    }
    private void OnMouseDown()
    {
        ChangeHighlight();
    }
    void Animate()
    {
        timer += Time.unscaledDeltaTime;
        frame.fillAmount = timer / maxTimer;
        highlight.gameObject.SetActive(true);
        if (timer >= maxTimer)
        {
            timer = 0;
        }
    }
    public void ChangeHighlight()
    {
        highlight.color = highlightColor;
    }
}
