using UnityEngine;

[CreateAssetMenu(fileName = "InteractSettings", menuName = "Scriptable Objects/InteractSettings")]
public class InteractSettings : ScriptableObject
{
    public float maxDistance;
    public float sphereRadius;
    public LayerMask interactableMask;
}
