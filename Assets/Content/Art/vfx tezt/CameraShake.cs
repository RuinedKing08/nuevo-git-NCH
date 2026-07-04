using System.Collections;
using UnityEngine;
using Unity.Cinemachine;

public class CameraShake : MonoBehaviour
{
    [SerializeField] private CinemachineCamera cinemachineCamera;

    [SerializeField] private float defaultAmplitude = 2f;
    [SerializeField] private float defaultFrequency = 8f;
    [SerializeField] private float defaultDuration = 0.5f;

    [SerializeField] private AnimationCurve shakeCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);

    private CinemachineBasicMultiChannelPerlin noise;
    private Coroutine shakeRoutine;

    private void Awake()
    {
        noise = cinemachineCamera.GetComponent<CinemachineBasicMultiChannelPerlin>();

        if (noise != null)
        {
            noise.AmplitudeGain = 0f;
            noise.FrequencyGain = 0f;
        }
    }

    public void Shake()
    {
        Shake(defaultAmplitude, defaultFrequency, defaultDuration);
    }

    public void Shake(float amplitude, float frequency, float duration)
    {
        if (noise == null)
            return;

        if (shakeRoutine != null)
            StopCoroutine(shakeRoutine);

        shakeRoutine = StartCoroutine(ShakeRoutine(amplitude, frequency, duration));
    }

    private IEnumerator ShakeRoutine(float amplitude, float frequency, float duration)
    {
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            float normalizedTime = Mathf.Clamp01(timer / duration);
            float curveValue = shakeCurve.Evaluate(normalizedTime);

            noise.AmplitudeGain = amplitude * curveValue;
            noise.FrequencyGain = frequency * curveValue;

            yield return null;
        }

        noise.AmplitudeGain = 0f;
        noise.FrequencyGain = 0f;

        shakeRoutine = null;
    }
}