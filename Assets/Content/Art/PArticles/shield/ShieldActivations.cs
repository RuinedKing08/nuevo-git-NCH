using UnityEngine;
using UnityEngine.VFX;
using System.Collections;

public class ShieldActivations : MonoBehaviour
{
    [SerializeField] private VisualEffect shieldVFX;

    [SerializeField] private float fullShieldScale = 30f;
    [SerializeField] private float ring1FullScale = 135f;
    [SerializeField] private float ring2FullScale = 140f;
    [SerializeField] private float ring3FullScale = 145f;

    [SerializeField] private float appearTime = 0.4f;
    [SerializeField] private float damageTime = 0.2f;
    [SerializeField] private float delayBetweenRings = 0.08f;

    private int strength;

    private void Awake()
    {
        shieldVFX.SetFloat("visib", 0f);
        shieldVFX.SetFloat("Scaleee", 0f);
        shieldVFX.SetFloat("ShieldLevels", 0f);

        for (int ring = 1; ring <= 3; ring++)
        {
            shieldVFX.SetFloat($"Ring{ring}visib", 0f);
            shieldVFX.SetFloat($"Ring{ring}Scale", 0f);
        }

        shieldVFX.SetFloat("DmgFlash", 0f);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ShowShield(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ShowShield(2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ShowShield(3);
        }
        else if (Input.GetKeyDown(KeyCode.L))
        {
            ShieldDamage();
        }

    }

    public void ShowShield(int level)
    {
        StopAllCoroutines();

        strength = Mathf.Clamp(level, 1, 3);

        shieldVFX.SetFloat("ShieldLevels", strength);
        shieldVFX.SendEvent("ShieldAppear");

        StartCoroutine(AnimateFloat("visib", 1f, appearTime));

        StartCoroutine(AnimateFloat("Scaleee", fullShieldScale, appearTime));

        StartCoroutine(ShowRings());
    }

    private IEnumerator ShowRings()
    {
        for (int ring = 1; ring <= strength; ring++)
        {
            StartCoroutine(AnimateRing( ring, 1f,GetRingFullScale(ring),appearTime));

            yield return new WaitForSeconds(delayBetweenRings);
        }
    }

    public void ShieldDamage()
    {
        if (strength <= 0)
            return;

        int ringToRemove = strength;

        strength--;

        shieldVFX.SetFloat("ShieldLevels", strength);

        StartCoroutine(AnimateRing(ringToRemove, 0f, 0f, damageTime));

        StartCoroutine(DamageFlash());

        if (strength > 0)
        {
            shieldVFX.SendEvent("ShieldHit");
        }
        else
        {
            ShieldKill();
        }
    }

    public void ShieldKill()
    {
        StopAllCoroutines();

        strength = 0;

        shieldVFX.SetFloat("ShieldLevels", 0f);
        shieldVFX.SendEvent("ShieldBreak");

        StartCoroutine(AnimateFloat("visib", 0f, damageTime));

        StartCoroutine(AnimateFloat("Scaleee", 0f, damageTime));

        for (int ring = 1; ring <= 3; ring++)
        {
            StartCoroutine(AnimateRing(ring, 0f, 0f, damageTime));
        }
    }

    private IEnumerator DamageFlash()
    {
        yield return StartCoroutine(AnimateFloat("DmgFlash", 1f, 0.07f));

        yield return StartCoroutine(AnimateFloat("DmgFlash", 0f, 0.2f));
    }

    private IEnumerator AnimateRing(int ring, float targetVisibility, float targetScale, float duration)
    {
        string visibilityName = $"Ring{ring}visib";
        string scaleName = $"Ring{ring}Scale";

        float startingVisibility =
            shieldVFX.GetFloat(visibilityName);

        float startingScale =
            shieldVFX.GetFloat(scaleName);

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float progress = Mathf.Clamp01(elapsed / duration);

            shieldVFX.SetFloat(visibilityName, Mathf.Lerp(startingVisibility, targetVisibility, progress));

            shieldVFX.SetFloat(
                scaleName,
                Mathf.Lerp(
                    startingScale,
                    targetScale,
                    progress
                )
            );

            yield return null;
        }

        shieldVFX.SetFloat(visibilityName, targetVisibility);
        shieldVFX.SetFloat(scaleName, targetScale);
    }

    private IEnumerator AnimateFloat(
        string propertyName,
        float target,
        float duration)
    {
        float startingValue =
            shieldVFX.GetFloat(propertyName);

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float progress = Mathf.Clamp01(elapsed / duration);

            float newValue = Mathf.Lerp(
                startingValue,
                target,
                progress
            );

            shieldVFX.SetFloat(propertyName, newValue);

            yield return null;
        }

        shieldVFX.SetFloat(propertyName, target);
    }

    private float GetRingFullScale(int ring)
    {
        switch (ring)
        {
            case 1:
                return ring1FullScale;

            case 2:
                return ring2FullScale;

            case 3:
                return ring3FullScale;

            default:
                return 0f;
        }
    }
}