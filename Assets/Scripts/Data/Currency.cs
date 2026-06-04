using UnityEngine;

public class Currency : MonoBehaviour
{
    private static float money;
    private static float finalScore;
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
    public void ChangeMoney(float sum, float score)
    {
        money += sum;
        totalScore += score;
    }
    void ChangeFinalScore()
    {
        if (Waves.Instance.GetWave() % 5 == 0) finalScore += totalScore;
    }
}
