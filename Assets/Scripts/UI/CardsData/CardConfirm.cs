using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class CardConfirm : MonoBehaviour
{
    Button thisButton;
    [SerializeField] Button rerollButton;
    [SerializeField] TMP_Text extraUpgradesLeftTMP;
    TMP_Text rerollTMP;
    public static CardConfirm Instance;
    public event ChangeWaves OnChangeWave;
    public delegate void ChangeWaves();
    public event ChangeCard OnChangeCards;
    public delegate void ChangeCard();
    CardView CV;
    public AudioData drinkAudio;
    private void Awake()
    {
        Instance = this;
        thisButton = GetComponent<Button>();
        rerollTMP = rerollButton.transform.Find("RerollTMP").GetComponent<TMP_Text>();
    }
    void Start()
    {
        thisButton.interactable = false;
    }
    public void InvokeChangeWave() { OnChangeWave?.Invoke(); }
    public void InvokeChangeCards() { OnChangeCards?.Invoke(); }
    public void CanConfirm(CardView CV)
    {
        if (CardView.clicked)
        {
            thisButton.interactable = true;
            this.CV = CV;
        }
    }
    public void Confirm()
    {
        foreach(string title in UniqueUpgrades.Instance.GetTitles())
        {
            if (title == CV.GetCard().title)
            {
                UniqueUpgrades.Instance.UUpgrades(CV.GetCard());
                CV.CloseAll();
                AudioManager.Instance.Play(drinkAudio);
                thisButton.interactable = false;
                Debug.Log("UsingUU");
                Time.timeScale = 1;
                CloseUpgrades();
                return;
            }
        }
        Debug.Log("UsingEffect");
        if(CV.GetCard().type == CardType.Extra)
        {
            Debug.Log("UsingExtraRerollIMDONE");
            CV.Effects();
            return;
        }
        else
        {
            CV.Effects();
        }
        AudioManager.Instance.Play(drinkAudio);
        thisButton.interactable = false;    
        Time.timeScale = 1;
        CloseUpgrades();
        ChangeExtraUpgadeTMP();
    }
    void CloseUpgrades()
    {
        if (Exp.Instance.GetExtraUpdrageFloat() > 0)
        {
            if (Waves.Instance.GetNewWaveBool())
            {
                InvokeChangeCards();
                Exp.Instance.ChangeFloatExtraUpgrade(-1);
            }
            /*else
            {
                Exp.Instance.ChangeFloatExtraUpgrade(-1);
            }*/
        }
        else
        {
            //InvokeChangeWave();
            ChangeRerollTMP(1);
            rerollButton.interactable = true;
            ChangeCards.Instance.cardsClosed = false;
            ChangeCards.cardsOpened = false;
            ChangeCards.cardsFinished = true;
        }
        
    }
    public void Reroll()
    {
        rerollButton.interactable = false;
        UseReroll();
        ChangeRerollTMP(0);
    }
    public void UseReroll()
    {
        if (CardView.clicked)
        {
            CV.DeactiveAll();
            thisButton.interactable = false;
            CardView.clicked = false;
        }
        InvokeChangeCards();
    }
    void ChangeRerollTMP(int uses)
    {
        rerollTMP.text = $"Rerolear {uses}/1";
    }
    public void ChangeExtraUpgadeTMP()
    {
        extraUpgradesLeftTMP.text = $"Mejoras extra restantes: {Exp.Instance.GetExtraUpdrageFloat()}";
    }
    public Button GetButton()
    {
        return thisButton;
    }
}
