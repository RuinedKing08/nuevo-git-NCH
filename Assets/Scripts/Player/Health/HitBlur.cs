using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class HitBlur : MonoBehaviour
{
    [SerializeField] private RawImage blurOverlay;
    [SerializeField] private float fadeInTime = 0.05f;
    [SerializeField] private float fadeOutTime = 0.2f;
    [SerializeField] private float maxAlpha = 0.45f;
    [SerializeField] private Transform cameraTransform;

    private Coroutine routine;

    private void Awake()
    {
        SetAlpha(0f);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.K))
        {
            Play();
        }
    }

    public void Play()
    {
        if (routine != null)
            StopCoroutine(routine);

        routine = StartCoroutine(PlayRoutine());
        StartCoroutine(CamShake());
    }

    private IEnumerator PlayRoutine()
    {
        float t = 0f;

        while (t < fadeInTime)
        {
            t += Time.deltaTime;
            SetAlpha(Mathf.Lerp(0f, maxAlpha, t / fadeInTime));
            yield return null;
        }

        t = 0f;

        while (t < fadeOutTime)
        {
            t += Time.deltaTime;
            SetAlpha(Mathf.Lerp(maxAlpha, 0f, t / fadeOutTime));
            yield return null;
        }

        SetAlpha(0f);
        routine = null;
    }

    private void SetAlpha(float alpha)
    {
        Color c = blurOverlay.color;
        c.a = alpha;
        blurOverlay.color = c;
    }

    public IEnumerator CamShake()
    {
        Vector3 originalPos = cameraTransform.localPosition;
        float duration = 0.12f;
        float strength = 0.08f;
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            cameraTransform.localPosition = originalPos + new Vector3(Random.Range(-strength, strength), Random.Range(-strength, strength), 0f);

            yield return null;
        }

        cameraTransform.localPosition = originalPos;
    }
}
