using UnityEngine;

public class EnemyBehindHUD : MonoBehaviour
{
    public static EnemyBehindHUD Instance;
    int enemiesBehindCount;
    public event ChangeEnemyBehindCount OnChangeEnemyBehindCount;
    public delegate void ChangeEnemyBehindCount();
    private void Awake()
    {
        Instance = this;
    }

    public void ChangeEnemiesBehindCount(int sum)
    {
        enemiesBehindCount += sum;
        OnChangeEnemyBehindCount?.Invoke();
    }
    public int GetEnemiesBehindCount()
    {
        return enemiesBehindCount;
    }
}
