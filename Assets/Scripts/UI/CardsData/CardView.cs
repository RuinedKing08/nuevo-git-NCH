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
    public static bool clicked;
    bool thisClicked;
    private void Awake()
    {
        player = GameObject.FindWithTag("Player").GetComponent<PlayerHealth>();
        //frame = GetComponentInParent<Image>();
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
        clicked = false;
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
        title.text = card.title + $" Nivel {card.level}";
        title.colorGradientPreset = card.textColor;
        description.colorGradientPreset = card.textColor;
        switch (card.title)
        {
            case "Gin":
                description.text = card.description;
                break;
            case "Manhattan":
                description.text = card.description + $" ({UniqueUpgrades.manhattanAmount})";
                break;
            case "Ron":
                description.text = card.description + $" ({UniqueUpgrades.ronAmount}%)";
                break;
            case "Brandy":
                description.text = card.description + $" ({UniqueUpgrades.brandyAmount}%)";
                break;
            case "Margarita":
                description.text = card.description + $" ({UniqueUpgrades.margaritaAmount})";
                break;
            case "Cerveza":
                description.text = card.description + $" ({Mathf.RoundToInt(player.CurrentHealth)} / {player.MaxHealth})";
                break;
            case "Vino":
                description.text = card.description + $" ({Mathf.RoundToInt(player.CurrentHealth)} / {player.MaxHealth})";
                break;
            case "Tequila":
                description.text = card.description + $" ({player.GetDamageResistance() * 100}% / 50%)";
                break;
            case "Martini":
                description.text = card.description + $" ({Currency.Instance.GetScoreMultiplier()})";
                break;
            case "Whiskey":
                description.text = card.description;
                break;
            case "Reroll":
                description.text = card.description;
                break;
        }            
    }
    public Card GetCard()
    {
        return card;
    }
    public void Clicked()
    {
        if (clicked)
        {
            DeactiveAnimateFrame();
            OnMouseEnter();
        }
        clicked = true;
        for (int i = 0; i < cardViews.Length; i++)
        {
            cardViews[i].thisClicked = false;
        }
        thisClicked = true;
        OnMouseExit();
        CardConfirm.Instance.CanConfirm(this);
    }
    public void Effects()
    {
        switch (card.type)
        {
            case CardType.Attack:
                Debug.Log("Mejora de ataque +" + card.amount);
                break;
            case CardType.Healing:
                player.GetLife(card.amount * player.MaxHealth / 100);
                break;
            case CardType.Protection:
                Debug.Log("Mejora de protección");
                player.ModifyDamageResistance(card.amount);
                break;
            case CardType.Score:
                Debug.Log($"Mejora de puntaje antes: {Currency.Instance.GetScoreMultiplier()}");
                Currency.Instance.ChangeScoreMultiplier(card.amount / 100);
                Debug.Log($"Mejora de puntaje ahora: {Currency.Instance.GetScoreMultiplier()}");
                break;
            case CardType.Extra:
                CardConfirm.Instance.UseReroll();
                return;
                break;
        }
        if (card.canChangeMaxLife)
        {
            Debug.Log("vida max antes: " + player.MaxHealth);
            player.GetMaxLife(card.extraMaxLife);
            player.GetLife(card.extraMaxLife);
            Debug.Log("vida max ahora: " + player.MaxHealth);
        }
        CloseAll();
    }
    public void CloseAll()
    {
        DeactiveAll();
        Time.timeScale = 1;
        cardSelection.SetActive(false);
    }
    public void DeactiveAll()
    {
        for (int i = 0; i < cardViews.Length; i++)
        {
            cardViews[i].animateFrame = false;
            cardViews[i].frame.fillAmount = 1;
            cardViews[i].thisClicked = false;
        }
    }
    public void OnMouseEnter()
    {
        //Debug.Log("Mouse Encima");
        if (clicked)
        {
            OnMouseExit();
        }
        animateFrame = true;
    }
    public void OnMouseExit()
    {
        DeactiveAnimateFrame();        
    }
    void DeactiveAnimateFrame()
    {
        for (int i = 0; i < cardViews.Length; i++)
        {
            if (cardViews[i].thisClicked) continue;
            cardViews[i].animateFrame = false;
            cardViews[i].frame.fillAmount = 1;
            cardViews[i].highlight.color = initialHighlightColor;
            cardViews[i].highlight.gameObject.SetActive(false);
        }
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
