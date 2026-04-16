using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(fileName = "MovementSettings", menuName = "Scriptable Objects/MovementSettings")]
public class MovementSettings : ScriptableObject
{
    public float speed;
    public float response;
    public float SprintSpeed;

    public float LerpRotationSpeed;    
    public float lockOnSpeedMultiplier = 0.7f;
}
