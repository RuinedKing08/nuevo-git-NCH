using UnityEngine;
using UnityEngine.UI;
public class CardConfirm : MonoBehaviour
{
    Button thisButton;
    public static CardConfirm Instance;
    public event ChangeWaves OnChangeWave;
    public delegate void ChangeWaves();
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
        Time.timeScale = 1;
        InvokeChangeWave();

    }
    public Button GetButton()
    {
        return thisButton;
    }
}
