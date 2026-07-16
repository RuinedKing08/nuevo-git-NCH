using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.ParticleSystemJobs;
public class DJVFX : MonoBehaviour
{
    public static DJVFX Instance;

    public VisualEffect hit1Right;
    public VisualEffect hit1Left;
    public ParticleSystem hit2;
    public ParticleSystem heal;
    public VisualEffect dmgBoost;
    public ParticleSystem forcePush;

    private void Awake()
    {
        Instance = this;
        hit1Right.SendEvent("OnPlay");
        hit1Right.SendEvent("OnStop");
        hit1Left.SendEvent("OnPlay");
        hit1Left.SendEvent("OnStop");
        dmgBoost.SendEvent("OnStop");
    }

    public void PlayHit1Right()
    {
        hit1Right.SendEvent("OnPlay");
    }
    public void PlayHit1Left()
    {
        hit1Left.SendEvent("OnPlay");
    }
    public void PlayHit2()
    {
        hit2.Play();
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
