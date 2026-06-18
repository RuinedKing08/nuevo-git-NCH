using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
public class Exp : MonoBehaviour
{
    Image expBar;
    static float exp;
    bool extraUpgrade;
    public static Exp Instance;
    public event FullExp OnFullExp;
    public delegate void FullExp();
    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        expBar = transform.Find("PlayerExpBar").GetComponent<Image>();
        ChangeExpHUD();
    }
    public void InvokeFullExp() { OnFullExp?.Invoke(); }
    public void ChangeExp(float exp)
    {
        Exp.exp += exp;
        if(Exp.exp < 0)
        {
            Exp.exp = 0;
        }
        ScorePopUp.Instance.CreateExpPopUp(exp);
        if(Exp.exp >= 100)
        {
            ChangeBoolExtraUpgrade(true);
            OpenExtraShop();
        }
        ChangeExpHUD();
    }
    public void ChangeBoolExtraUpgrade(bool extraUpgrade)
    {
        this.extraUpgrade = extraUpgrade;
    }
    public bool GetExtraUpdrageBool()
    {
        return extraUpgrade;
    }
    void OpenExtraShop()
    {
        InvokeFullExp();
        exp -= 100;
    }
    void ChangeExpHUD()
    {
        expBar.fillAmount = exp / 100;
    }
}
