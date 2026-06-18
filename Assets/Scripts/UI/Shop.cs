using UnityEngine;
using TMPro;
public class Shop : MonoBehaviour
{
    [SerializeField] GameObject shop;
    [SerializeField] TMP_Text moneyTMP;
    [SerializeField] PlayerHealth player;
    void Start()
    {
        shop.SetActive(false);
        Waves.Instance.OnChangeWave += OpenShop;
        //Time.timeScale = 1f;
    }

    void Update()
    {
        
    }
    public void BuyBeer()
    {
        if(Currency.Instance.GetMoney() >= 4.20f)
        {
            Currency.Instance.ChangeMoney(-4.20f, 0);
            ChangeMoneyTMP();
            player.GetLife(25 * player.MaxHealth / 100);
        }
    }
    public void BuyWine()
    {
        if (Currency.Instance.GetMoney() >= 7.00f)
        {
            Currency.Instance.ChangeMoney(-7.00f, 0);
            ChangeMoneyTMP();
            player.GetLife(30 * player.MaxHealth / 100);
        }
    }
    public void BuyWhiskey()
    {
        if (Currency.Instance.GetMoney() >= 9.90f)
        {
            Currency.Instance.ChangeMoney(-9.90f, 0);
            ChangeMoneyTMP();
            player.GetLife(30 * player.MaxHealth / 100);
        }
    }
    public void BuyMartini()
    {
        if (Currency.Instance.GetMoney() >= 12.50f)
        {
            Currency.Instance.ChangeMoney(-12.50f, 0);
            ChangeMoneyTMP();
            player.GetLife(50 * player.MaxHealth / 100);
        }
    }
    void OpenShop()
    {
        /*if (Waves.Instance.GetWave() % 1 == 0 && Waves.Instance.GetWave()!= 0)
        {
            Time.timeScale = 0f;
            ChangeMoneyTMP();
            shop.SetActive(true);
        }*/
    }
    public void CloseShop()
    {
        Time.timeScale = 1f;
        shop.SetActive(false);
    }
    void ChangeMoneyTMP()
    {
        moneyTMP.text = $"Dinero: {Mathf.Round(Currency.Instance.GetMoney())}$";
    }
}
