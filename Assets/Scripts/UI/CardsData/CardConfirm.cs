using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class CardConfirm : MonoBehaviour
{
    Button thisButton;
    [SerializeField] Button rerollButton;
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
        AudioManager.Instance.Play(drinkAudio);
        CV.Effects();
        thisButton.interactable = false;
        rerollButton.interactable = true;        
        Time.timeScale = 1;
        CloseUpgrades();

    }
    void CloseUpgrades()
    {
        if (Exp.Instance.GetExtraUpdrageBool())
        {
            if (Waves.Instance.GetNewWaveBool())
            {
                InvokeChangeCards();
                Exp.Instance.ChangeBoolExtraUpgrade(false);
            }
            else
            {
                Exp.Instance.ChangeBoolExtraUpgrade(false);
            }
        }
        else
        {
            InvokeChangeWave();
        }
        ChangeRerollTMP(1);
        ChangeCards.cardsOpened = false;
        
    }
    public void Reroll()
    {
        rerollButton.interactable = false;
        if (CardView.clicked)
        {
            CV.DeactiveAll();
            thisButton.interactable = false;
            ChangeRerollTMP(0);
            CardView.clicked = false;
        }
        InvokeChangeCards();
    }
    void ChangeRerollTMP(int uses)
    {
        rerollTMP.text = $"Rerolear {uses}/1";
    }
    public Button GetButton()
    {
        return thisButton;
    }
}
