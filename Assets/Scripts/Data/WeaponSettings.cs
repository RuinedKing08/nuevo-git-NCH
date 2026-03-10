
using UnityEngine;


public class WeaponSettings : ScriptableObject
{
    public float Damague;

    [Header("Animation")]
    public float AttackDuration;

    [Header("Throw")]
    public float maxDistance;
    public LayerMask aimMask;
    public float ThrowForce;
}
