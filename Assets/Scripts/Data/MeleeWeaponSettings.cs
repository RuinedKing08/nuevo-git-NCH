using UnityEngine;

[CreateAssetMenu(fileName = "MeleeWeaponSettings", menuName = "Scriptable Objects/MeleeWeaponSettings")]
public class MeleeWeaponSettings : WeaponSettings
{
    public float baseDamage;
    public AttackMove entryMove;
}
