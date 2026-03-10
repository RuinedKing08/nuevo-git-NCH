using UnityEngine;

[CreateAssetMenu(fileName = "FireWeaponSettings", menuName = "Scriptable Objects/FireWeaponSettings")]
public class FireWeaponSettings : WeaponSettings
{
    [Header("AimDirectDeteccion")]
    public float maxDistance;
    public LayerMask aimMask;

    public FireMode _fireMode;
}
