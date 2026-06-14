using UnityEngine;
using UnityEngine.UI;
public class CardConfirm : MonoBehaviour
{
    Button thisButton;
    [SerializeField] Button rerollButton;
    public static CardConfirm Instance;
    public event ChangeWaves OnChangeWave;
    public delegate void ChangeWaves();
    public event ChangeCards OnChangeCards;
    public delegate void ChangeCards();
    CardView CV;
    private void Awake()
    {
        Instance = this;
        thisButton = GetComponent<Button>();
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
        CV.Effects();
        thisButton.interactable = false;
        rerollButton.interactable = true;
        Time.timeScale = 1;
        InvokeChangeWave();

    }
    public void Reroll()
    {
        rerollButton.interactable = false;
        if (CardView.clicked)
        {
            CV.DeactiveAll();
            thisButton.interactable = false;
            CardView.clicked = false;
        }
        InvokeChangeCards();
    }
    public Button GetButton()
    {
        return thisButton;
    }
}
