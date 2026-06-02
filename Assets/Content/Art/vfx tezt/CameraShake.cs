using System.Collections;
using UnityEngine;
using Unity.Cinemachine;

public class CameraShake : MonoBehaviour
{
    [SerializeField] private CinemachineCamera cinemachineCamera;

    [SerializeField] private float defaultAmplitude = 2f;
    [SerializeField] private float defaultFrequency = 8f;
    [SerializeField] private float defaultDuration = 0.15f;

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

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.X))
        {
            Shake();
        }

    }

    public void Shake()
    {
        Shake(defaultAmplitude, defaultFrequency, defaultDuration);
    }

    public void Shake(float amplitude, float frequency, float duration)
    {

        if (shakeRoutine != null)
            StopCoroutine(shakeRoutine);

        shakeRoutine = StartCoroutine(ShakeRoutine(amplitude, frequency, duration));
    }

    private IEnumerator ShakeRoutine(float amplitude, float frequency, float duration)
    {
        noise.AmplitudeGain = amplitude;
        noise.FrequencyGain = frequency;

        yield return new WaitForSeconds(duration);

        noise.AmplitudeGain = 0f;
        noise.FrequencyGain = 0f;

        shakeRoutine = null;
    }
}