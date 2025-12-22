using System;
using UnityEngine;

public enum GameState
{
    Menu,
    Gameplay,
    Inventory
}

public enum InputState
{
    UI,
    Player,
    Inventory,
    Vehicle
}


public struct characterMotorInput
{
    public Vector2 Move;
    public bool Crouch;
    public Quaternion Rotation;
    public bool Jump;

}

public struct CameraInput
{
    public Vector2 Rotation;

}

public struct ActionInput
{
    public bool Interact;
    public bool Attack;
    public bool Aim;
    public bool Recharge;
}

public struct GeneralInput
{
    public bool movementInputTapped;
    public bool movementInputPressed;
    public bool movementInputHeld;
    public float movementInputDuration;
    public bool ChangueFromToMenu;
    public Vector2 inputDirection;
    
}

[Serializable]

public struct PlayerState
{
    public CharacterState characterState;
    public PlayerActionState actionState;
    public CameraState cameraState;
    public GeneralInput generalInput;
}

[Serializable]
public struct CharacterState
{
    public bool isGrounded;
    public bool hasStopped;
    public Stance Stance;
    public bool canUseItems;
    public CharacterBehaviourState behaviourState;
    public Vector3 Acceleration;
    public float MoveSpeed;
    public bool isJumping;
    public Vector2 inputDirection;
}

[Serializable]
public enum CharacterBehaviourState
{
    DEFAULT,
    CLIMBING,
    DISABLED
}

[Serializable]
public struct PlayerActionState
{
    public ActionState actionState;
    public WeaponState weaponState;
    public Holding HoldingState;
}

[Serializable]
public enum ActionState
{
    NONE,
    RECHARGE,
    ATTACK,
    INTERACTING,
    CHANGING
}
[Serializable]
public enum WeaponState
{
    AIMING,
    DEFAULT,
}

public enum Holding
{
    None,
    FireArm,
    MeleeItem
}

[Serializable]
public enum CameraState
{
    FIRST,

}

public enum Stance
{
    Stand,
    Crouch
}


