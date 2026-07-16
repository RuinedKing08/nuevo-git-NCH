using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class DirectionalWarning : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private RectTransform indicatorCenter;
    [SerializeField] private RectTransform arrowPrefab;

    [SerializeField] private string enemyTag = "DamageToEnemies";
    [SerializeField] private float detectionRadius = 15f;

    [SerializeField] private float circleRadius = 180f;
    [SerializeField] private float fadeSpeed = 10f;
    [SerializeField] private float rotationOffset = 0f;

    [SerializeField] private float minAlpha = 0.15f;
    [SerializeField] private float maxAlpha = 1f;

    [SerializeField] private int maxIndicators = 8;

    private readonly List<RectTransform> arrows = new List<RectTransform>();
    private readonly List<Image> arrowImages = new List<Image>();
    private readonly List<Transform> nearbyEnemies = new List<Transform>();

    private void Awake()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        if (indicatorCenter == null)
            indicatorCenter = GetComponent<RectTransform>();

        CreateArrowPool();
    }

    private void CreateArrowPool()
    {
        for (int i = 0; i < maxIndicators; i++)
        {
            RectTransform arrow = Instantiate(arrowPrefab, indicatorCenter);
            arrow.gameObject.SetActive(false);

            arrows.Add(arrow);
            arrowImages.Add(arrow.GetComponent<Image>());
        }
    }

    private void Update()
    {
        if (player == null || mainCamera == null || arrowPrefab == null)
            return;

        FindNearbyEnemies();

        for (int i = 0; i < arrows.Count; i++)
        {
            if (i < nearbyEnemies.Count)
            {
                UpdateIndicator(i, nearbyEnemies[i]);
            }
            else
            {
                HideIndicator(i);
            }
        }
    }

    private void FindNearbyEnemies()
    {
        nearbyEnemies.Clear();

        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);

        List<EnemyDistance> enemyDistances = new List<EnemyDistance>();

        foreach (GameObject enemy in enemies)
        {
            if (enemy == null)
                continue;

            float distance = Vector3.Distance(player.position, enemy.transform.position);

            if (distance <= detectionRadius)
            {
                enemyDistances.Add(new EnemyDistance(enemy.transform, distance));
            }
        }

        enemyDistances.Sort((a, b) => a.distance.CompareTo(b.distance));

        int amount = Mathf.Min(enemyDistances.Count, maxIndicators);

        for (int i = 0; i < amount; i++)
        {
            nearbyEnemies.Add(enemyDistances[i].enemy);
        }
    }

    private void UpdateIndicator(int index, Transform enemy)
    {
        RectTransform arrow = arrows[index];
        Image image = arrowImages[index];

        if (!arrow.gameObject.activeSelf)
            arrow.gameObject.SetActive(true);

        Vector3 directionToEnemy = enemy.position - player.position;
        directionToEnemy.y = 0f;

        if (directionToEnemy.sqrMagnitude <= 0.01f)
            return;

        Vector3 cameraForward = mainCamera.transform.forward;
        Vector3 cameraRight = mainCamera.transform.right;

        cameraForward.y = 0f;
        cameraRight.y = 0f;

        cameraForward.Normalize();
        cameraRight.Normalize();

        Vector3 flatDirection = directionToEnemy.normalized;

        float forwardAmount = Vector3.Dot(cameraForward, flatDirection);
        float rightAmount = Vector3.Dot(cameraRight, flatDirection);

        float angle = Mathf.Atan2(rightAmount, forwardAmount);

        float x = Mathf.Sin(angle) * circleRadius;
        float y = Mathf.Cos(angle) * circleRadius;

        arrow.anchoredPosition = new Vector2(x, y);

        float angleDegrees = angle * Mathf.Rad2Deg;
        arrow.localEulerAngles = new Vector3(0f, 0f, -angleDegrees + rotationOffset);

        float distance = Vector3.Distance(player.position, enemy.position);
        float closeness = 1f - Mathf.Clamp01(distance / detectionRadius);
        float alpha = Mathf.Lerp(minAlpha, maxAlpha, closeness);

        SetAlpha(image, alpha);
    }

    private void HideIndicator(int index)
    {
        if (arrows[index].gameObject.activeSelf)
            arrows[index].gameObject.SetActive(false);
    }

    private void SetAlpha(Image image, float alpha)
    {
        if (image == null)
            return;

        Color color = image.color;
        color.a = alpha;
        image.color = color;
    }

    private struct EnemyDistance
    {
        public Transform enemy;
        public float distance;

        public EnemyDistance(Transform enemy, float distance)
        {
            this.enemy = enemy;
            this.distance = distance;
        }
    }
}