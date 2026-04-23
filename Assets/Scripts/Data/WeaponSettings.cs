using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "Game/WeaponSettings")]
public class WeaponSettings : ScriptableObject
{
    [FormerlySerializedAs("Damague")]
    public float Damage;

    [Header("Throw")]
    public float maxDistance;
    public LayerMask aimMask;
    public float ThrowForce;
}
