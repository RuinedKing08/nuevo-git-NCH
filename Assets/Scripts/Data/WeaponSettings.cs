
using UnityEngine;


public class WeaponSettings : ScriptableObject
{
    [Header("AimDirectDeteccion")]
    public float maxDistance;
    public LayerMask aimMask;

    [Header("Animation")]



    public float ThrowForce;
}
