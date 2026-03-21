using UnityEngine;

[CreateAssetMenu(fileName = "FireWeaponSettings", menuName = "Scriptable Objects/FireWeaponSettings")]
public class FireWeaponSettings : WeaponSettings
{
    [Header("AimDirectDeteccion")]
   

    public FireMode _fireMode;
}
