using UnityEngine;
using UnityEngine.UI;

public class LowHP : MonoBehaviour
{
    [SerializeField] private GameObject heartbeatObject;
    [SerializeField] private Image heartbeatImage;

    [SerializeField] private bool heartbeatActive;

    [SerializeField] private float minAlpha = 0.05f;
    [SerializeField] private float maxAlpha = 0.45f;
    [SerializeField] private float pulseSpeed = 5f;

    [SerializeField] private KeyCode testKey = KeyCode.T;

    private void Start()
    {
        SetHeartbeat(heartbeatActive);
    }

    private void Update()
    {
        if (Input.GetKeyDown(testKey))
        {
            SetHeartbeat(!heartbeatActive);
        }

        if (!heartbeatActive)
            return;

        float pulse = (Mathf.Sin(Time.time * pulseSpeed) + 1f) * 0.5f;
        float alpha = Mathf.Lerp(minAlpha, maxAlpha, pulse);

        Color color = heartbeatImage.color;
        color.a = alpha;
        heartbeatImage.color = color;
    }

    public void SetHeartbeat(bool active)
    {
        heartbeatActive = active;

        heartbeatObject.SetActive(active);

        if (!active && heartbeatImage != null)
        {
            Color color = heartbeatImage.color;
            color.a = 0f;
            heartbeatImage.color = color;
        }
    }
}