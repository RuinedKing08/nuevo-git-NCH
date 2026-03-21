using Sirenix.OdinInspector;
using System;
using UnityEngine;
[Serializable]
[InlineEditor]
public class AttackBranch
{
    public float MaxTimeSinceLastAttack;
    [InlineEditor]public AttackMove nextMove;
}
