using UnityEngine;
using TMPro;
public class ScorePopUp : MonoBehaviour
{
    [SerializeField] GameObject prefabScorePopUp;
    TMP_Text scorePopUpTMP;
    public static ScorePopUp Instance;
    Collider spawnZone;
    void Awake()
    {
        Instance = this;
        spawnZone = GetComponent<Collider>();
    }
    public void CreateScorePopUp(float score)
    {
        Vector3 randomPoint = GetRandomPointInCollider(spawnZone);
        scorePopUpTMP = Instantiate(prefabScorePopUp,randomPoint,gameObject.transform.rotation ,gameObject.transform).transform.Find("ScorePopUpTMP").GetComponent<TMP_Text>();
        scorePopUpTMP.text = $"{score * Currency.Instance.GetScoreMultiplier()}";

    }

    Vector3 GetRandomPointInCollider(Collider targetCollider)
    {
        Bounds limits = targetCollider.bounds;
        Vector3 randomPoint;
        int tries = 0;
        do
        {
            float randomX = Random.Range(limits.min.x, limits.max.x);
            float randomY = Random.Range(limits.min.y, limits.max.y);
            float randomZ = Random.Range(limits.min.z, limits.max.z);
            randomPoint = new Vector3(randomX, randomY, randomZ);
            tries++;
            if (tries > 100) break;
        } while (targetCollider.ClosestPoint(randomPoint) != randomPoint);
        return randomPoint;
    }
}
