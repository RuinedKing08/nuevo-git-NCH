using UnityEngine;

public class Currency : MonoBehaviour
{
    private static float money;
    private static float finalScore;
    private static float scoreMultiplier = 1;
    private float totalScore;
    public static Currency Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        Waves.Instance.OnChangeWave += ChangeFinalScore;
    }

    void Update()
    {
        
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
    }
    void ChangeFinalScore()
    {
        if (Waves.Instance.GetWave() % 5 == 0) finalScore += totalScore;
    }
}
