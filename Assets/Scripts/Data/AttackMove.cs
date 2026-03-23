using System;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
[CreateAssetMenu(fileName = "AttackMove", menuName = "Scriptable Objects/AttackMove")]
public class AttackMove : ScriptableObject
{
    [Header("Animation")]
    public AnimationClip animationClip;
    public AudioClip audioClip;
    public AudioClip hitClip;

    [Header("Damage")]
    public float damageMod; // + or -

    [Header("HitBox")]
    
    public Vector3 hitboxSize;
    

    [Header("Movement")]
    public bool allowMovement; 
    public float movementMultiplayer;

    [Header("Requeriments")]
    public float InputWindow; // seconds : 0.3f

    [Header("Branches")]
   [SerializeReference] public List<AttackBranch> movements;
}
