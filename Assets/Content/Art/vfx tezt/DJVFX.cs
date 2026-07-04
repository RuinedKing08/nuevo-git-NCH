using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.ParticleSystemJobs;
public class DJVFX : MonoBehaviour
{
    public static DJVFX Instance;

    public VisualEffect hit1;
    public VisualEffect hit2;
    public ParticleSystem heal;
    public VisualEffect dmgBoost;
    public ParticleSystem forcePush;

    private void Awake()
    {
        Instance = this;
        hit1.SendEvent("OnStop");
        dmgBoost.SendEvent("OnStop");
    }

    public void PlayHit1()
    {
        hit1.SendEvent("OnPlay");
    }
    public void PlayHit2()
    {
        hit2.SendEvent("OnPlay");
    }
    public void PlayHeal()
    {
        heal.Play();
    }
    public void PlayDmg()
    {
        dmgBoost.SendEvent("OnPlay");
    }
    public void StopDmg()
    {
        dmgBoost.SendEvent("OnStop");
    }
    public void PlayPush()
    {
        forcePush.Play();
    }
}
