using KinematicCharacterController;
using UnityEngine;

public abstract class CharacterMovementStateBase : ICharacterMovementState
{
    protected PlayerCharacter character;

   

    protected Vector3 _requestedMovement;
    protected Quaternion _requestedRotation;
    protected bool _requestedJump;
    protected Vector3 _rawInput;
    protected Vector3 _moveInput;
    
 
    protected CharacterMovementStateBase(PlayerCharacter character)
    {
        this.character = character;
    }

    public virtual void Enter()
    {
        
    }

    public virtual void Exit()
    {
        
    }
    public virtual void ProcessInput(characterMotorInput input)
    {
        
    }

    public virtual void UpdateBody()
    {
        
    }
    public virtual void AfterCharacterUpdate(float deltaTime)
    {
        
    }

    public virtual void BeforeCharacterUpdate(float deltaTime)
    {
        
    }


    public virtual bool IsColliderValidForCollisions(Collider coll)
    {
        return true;
    }

    public virtual void OnDiscreteCollisionDetected(Collider hitCollider)
    {
        
    }

    public virtual void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
    {
        
    }

    public virtual void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
    {
        
    }

    public virtual void PostGroundingUpdate(float deltaTime)
    {
        
    }

    public virtual void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport)
    {
        
    }


    public virtual void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
    {
        
    }

    public virtual void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
    {
        
    }

    
}
