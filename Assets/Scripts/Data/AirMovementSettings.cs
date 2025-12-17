using UnityEngine;

[CreateAssetMenu(fileName = "AirMovementSettings", menuName = "Scriptable Objects/AirMovementSettings")]
public class AirMovementSettings : ScriptableObject
{
    public float AirSpeed,AirAcceleration,JumpSpeed,Gravity,CoyoteTime;
}
