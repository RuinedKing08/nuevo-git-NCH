using System;
using UnityEngine;

public class ItemBase : MonoBehaviour
{
    [SerializeField] private Holding itemType;
    public Holding ItemType => itemType;

}
