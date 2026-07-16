using UnityEngine;
using TMPro;
public class Currency : MonoBehaviour
{
    private static float money;
    public static float finalScore;
    private static float scoreMultiplier = 1;
    private float totalScore;
    public static Currency Instance { get; private set; }
    TMP_Text scoreTMP;
    private void Awake()
    {
        Instance = this;
        scoreTMP = transform.Find("ScoreTMP").GetComponent<TMP_Text>();
    }
    void Start()
    {
        Waves.Instance.OnChangeWave += ChangeFinalScore;
        ChangeMoney(0, 0);
        ChangeScoreMultiplier(-scoreMultiplier + 1);
    }

    public float GetMoney()
    {
        return money;
    }
    public float GetScoreMultiplier()
    {
        return scoreMultiplier;
    }
    public void ChangeScoreMultiplier(float multiplier)
    {
        scoreMultiplier += multiplier;
    }
    public void ChangeMoney(float sum, float score)
    {
        money += sum;
        totalScore += score * scoreMultiplier;
        scoreTMP.text = $"Score: {totalScore}";
    }
    void ChangeFinalScore()
    {
        if (Waves.Instance.GetWave() % 5 == 0) finalScore += totalScore;
    }
}
