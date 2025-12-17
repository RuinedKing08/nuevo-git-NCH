using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(fileName = "MovementSettings", menuName = "Scriptable Objects/MovementSettings")]
public class MovementSettings : ScriptableObject
{
    public float speed;
    public float response;
    public float SprintSpeed;
}
