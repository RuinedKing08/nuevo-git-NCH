using UnityEngine;

public class UniqueUpgrades : MonoBehaviour
{
    public static bool brandyBuff;
    public static float brandyLevel;
    public static float brandyAmount;
    public static bool ronBuff;
    public static float ronLevel;
    public static float ronAmount;
    public static bool margaritaBuff;
    public static float margaritaLevel;
    public static float margaritaAmount;
    public static bool ginBuff;
    public static float ginLevel;
    public static float ginAmount;
    public static bool whiskeyBuff;
    public static float whiskeyLevel;
    public static float whiskeyAmount;
    public static bool manhattanBuff;
    public static float manhattanLevel;
    public static float manhattanAmount;
    [SerializeField]string[] titles;
    public static UniqueUpgrades Instance;
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
       /* whiskeyBuff = true;
        whiskeyLevel = 3;*/
    }
    public void UUpgrades(Card CD)
    {
        switch (CD.title)
        {
            case "Brandy":
                brandyBuff = true;
                brandyLevel = CD.level;
                brandyAmount = CD.amount;
                break;
            case "Ron":
                ronBuff = true;
                ronLevel = CD.level;
                ronAmount += CD.amount;
                break;
            case "Bourbon":
                margaritaBuff = true;
                margaritaLevel = CD.level;
                margaritaAmount += CD.amount;
                break;
            case "Gin":
                ginBuff = true;
                ginLevel = CD.level;
                ginAmount = CD.amount;
                ChangeDamage();
                break;
            case "Whiskey":
                whiskeyBuff = true;
                whiskeyLevel = CD.level;
                whiskeyAmount = CD.amount;
                break;
            case "Absenta":
                manhattanBuff = true;
                manhattanLevel = CD.level;
                manhattanAmount += CD.amount;
                manhattanAmount = Mathf.Clamp(manhattanAmount, 0, 3);
                Shield();
                break;
            default:
                Debug.Log("ErrorEnUU");
                break;
        }
    }
    public void ChangeDamage()
    {
        if (ginBuff)
        {
            DJVFX.Instance.PlayDmg();
            PlayerHitCollider.Instance.ModifyDamage(Mathf.RoundToInt(ginAmount));
        }
        else
        {
            DJVFX.Instance.StopDmg();
            PlayerHitCollider.Instance.ModifyDamage(-Mathf.RoundToInt(ginAmount));
        }
    }
    public void Shield()
    {
        if (manhattanBuff)
        {
            ShieldActivations.Instance.ShowShield(manhattanAmount);
        }
    }
    public string[] GetTitles()
    {
        return titles;
    }
}
