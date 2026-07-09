using UnityEngine;
using TMPro;
public class ScorePopUp : MonoBehaviour
{
    [SerializeField] GameObject prefabScorePopUp, prefabLifeLostPopUp, prefabExpPopUp, prefabHealLifePopUp;
    [SerializeField] GameObject scorePopUpArea, lifeLostPopUpArea, expPopUpArea, healPopUpArea;
    TMP_Text popUpTMP;
    RectTransform lifeLostSpawnZone;
    RectTransform healLifeSpawnZone;
    RectTransform expSpawnZone;
    public static ScorePopUp Instance;
    Collider scoreSpawnZone;
    void Awake()
    {
        Instance = this;
        scoreSpawnZone = scorePopUpArea.GetComponent<Collider>();
        lifeLostSpawnZone = lifeLostPopUpArea.GetComponent<RectTransform>();
        healLifeSpawnZone = healPopUpArea.GetComponent<RectTransform>();
        expSpawnZone = expPopUpArea.GetComponent<RectTransform>();
    }
    public void CreateScorePopUp(float score)
    {
        Vector3 randomPoint = GetRandomPointInCollider(scoreSpawnZone);
        popUpTMP = Instantiate(prefabScorePopUp, randomPoint, scorePopUpArea.transform.rotation , scorePopUpArea.transform).transform.Find("PopUpTMP").GetComponent<TMP_Text>();
        popUpTMP.text = $"{score * Currency.Instance.GetScoreMultiplier()}";

    }
    public void CreateLostLifePopUp(float lifeLost)
    {
        Vector3 randomPoint = GetRandomPointInUI(lifeLostSpawnZone);
        popUpTMP = Instantiate(prefabLifeLostPopUp, randomPoint, lifeLostPopUpArea.transform.rotation, lifeLostPopUpArea.transform).transform.Find("PopUpTMP").GetComponent<TMP_Text>();
        popUpTMP.text = $"-{lifeLost}";
    }
    public void CreateHealLifePopUp(float healLife)
    {
        Vector3 randomPoint = GetRandomPointInUI(healLifeSpawnZone);
        popUpTMP = Instantiate(prefabHealLifePopUp, randomPoint, healPopUpArea.transform.rotation, healPopUpArea.transform).transform.Find("PopUpTMP").GetComponent<TMP_Text>();
        popUpTMP.text = $"+{healLife}";
    }
    public void CreateExpPopUp(float exp)
    {
        Vector3 randomPoint = GetRandomPointInUI(expSpawnZone);
        popUpTMP = Instantiate(prefabExpPopUp, randomPoint, expPopUpArea.transform.rotation, expPopUpArea.transform).transform.Find("PopUpTMP").GetComponent<TMP_Text>();
        popUpTMP.text = $"+{exp}";
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

    Vector3 GetRandomPointInUI(RectTransform rectTransform)
    {
        Rect rect = rectTransform.rect;
        float randomX = Random.Range(rect.xMin, rect.xMax);
        float randomY = Random.Range(rect.yMin, rect.yMax);
        Vector3 localPointUI = new Vector3(randomX, randomY, 0f);
        Vector3 worldPosition = rectTransform.TransformPoint(localPointUI);
        worldPosition.z = 0;
        return worldPosition;
    }
}
