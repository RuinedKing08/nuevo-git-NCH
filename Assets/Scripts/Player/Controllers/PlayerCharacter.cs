using KinematicCharacterController;
using System;
using UnityEngine;

public class PlayerCharacter : MonoBehaviour,ICharacterController
{

    #region References

    [SerializeField] private KinematicCharacterMotor motor;
    [SerializeField] private Transform camTargetInCharacter;
    [SerializeField] private Transform weaponPos;
    public Transform CamPos => camTargetInCharacter;
    public Transform WeaponPos => weaponPos;

    [Header("Settings")]
    [SerializeField] private MovementSettings standSettings;
    [SerializeField] private MovementSettings crouchSettings;
    [SerializeField] private AirMovementSettings airSettings;

    #endregion


    #region Runtime
    [NonSerialized] public PlayerController controller;
    [NonSerialized] public CharacterState _characterState;

    [NonSerialized] public CharacterState _lastState;
    [NonSerialized] public CharacterState _tempState;

    protected Vector3 _externalForces;

    [NonSerialized] public float _timeSinceUngrounded;
    [NonSerialized] public float _timeSinceJumpRequest;
    [NonSerialized] public bool _ungroundedDueToJump;
    [NonSerialized] public Collider[] _uncrouchOverlapResults = new Collider[8];
    public Vector3 ExternalForces => _externalForces;


    private ICharacterMovementState currentState;
    public KinematicCharacterMotor Motor => motor;
    public MovementSettings StandSettings => standSettings;
    public MovementSettings CrouchSettings => crouchSettings;
    public AirMovementSettings AirSettings => airSettings;

    #endregion

    #region Settings


    #endregion



    public void Initialize(PlayerController controller)
    {
        this.controller = controller;
        _characterState.Stance = Stance.Stand;
        _lastState = _characterState;
        motor.CharacterController = this;
        motor.GroundDetectionExtraDistance = 0.1f;

        SetState(new DefaultCharacterState(this));

    }
    public void ProcessInput(characterMotorInput input)
    {
        currentState?.ProcessInput(input);
    }

    public void UpdateBody()
    {

    }

    public void AfterCharacterUpdate(float deltaTime)
    {

    }

    public void BeforeCharacterUpdate(float deltaTime)
    {

    }


    public bool IsColliderValidForCollisions(Collider coll)
    {
        return true;
    }

    public void OnDiscreteCollisionDetected(Collider hitCollider)
    {

    }

    public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
    {

    }

    public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
    {

    }

    public void PostGroundingUpdate(float deltaTime)
    {

    }

    public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport)
    {

    }


    public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
    {
        currentState?.UpdateRotation(ref currentRotation, deltaTime);
    }

    public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
    {
        currentState?.UpdateVelocity(ref currentVelocity, deltaTime);
    }


    public void SetState(ICharacterMovementState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState.Enter();
        Debug.Log("Settign state " + _characterState.behaviourState);
    }
    public KinematicCharacterMotor getMotor()
    {
        return motor;
    }
    public void SetExternalForces(Vector3 direc, float force = 1f)
    {
        _externalForces = direc * force;

    }

    public void UpdateCharacterState(PlayerState state)
    {
        state.characterState = _characterState;
    }
}
