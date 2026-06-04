using UnityEngine;
using TMPro;
public class Shop : MonoBehaviour
{
    Currency currency;
    [SerializeField] GameObject shop;
    [SerializeField] TMP_Text moneyTMP;
    PlayerHealth player;
    void Start()
    {
        currency = Currency.Instance;
        player = PlayerHealth.Instance;
        shop.SetActive(false);
        Waves.Instance.OnChangeWave += OpenShop;
        Time.timeScale = 1f;
    }

    void Update()
    {
        
    }
    public void BuyBeer()
    {
        if(currency.GetMoney() >= 4.20f)
        {
            currency.ChangeMoney(-4.20f, 0);
            ChangeMoneyTMP();
            player.GetLife(25 * player.MaxHealth / 100);
        }
    }
    public void BuyWine()
    {
        if (currency.GetMoney() >= 7.00f)
        {
            currency.ChangeMoney(-7.00f, 0);
            ChangeMoneyTMP();
            player.GetLife(30 * player.MaxHealth / 100);
        }
    }
    public void BuyWhiskey()
    {
        if (currency.GetMoney() >= 9.90f)
        {
            currency.ChangeMoney(-9.90f, 0);
            ChangeMoneyTMP();
            player.GetLife(30 * player.MaxHealth / 100);
        }
    }
    public void BuyMartini()
    {
        if (currency.GetMoney() >= 12.50f)
        {
            currency.ChangeMoney(-12.50f, 0);
            ChangeMoneyTMP();
            player.GetLife(50 * player.MaxHealth / 100);
        }
    }
    void OpenShop()
    {
        if (Waves.Instance.GetWave() % 2 == 0 && Waves.Instance.GetWave()!= 0)
        {
            Time.timeScale = 0f;
            ChangeMoneyTMP();
            shop.SetActive(true);
        }
    }
    public void CloseShop()
    {
        Time.timeScale = 1f;
        shop.SetActive(false);
    }
    void ChangeMoneyTMP()
    {
        moneyTMP.text = $"Dinero: {Mathf.Round(currency.GetMoney())}$";
    }
}
