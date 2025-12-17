using KinematicCharacterController;
using UnityEngine;

public interface ICharacterMovementState
{
    

    void Enter();
    void Exit();

    
    
    public void ProcessInput(characterMotorInput input);

    public void UpdateBody();
    
    public void AfterCharacterUpdate(float deltaTime);

    public void BeforeCharacterUpdate(float deltaTime);

    public bool IsColliderValidForCollisions(Collider coll);

    public void OnDiscreteCollisionDetected(Collider hitCollider);

    public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport);

    public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport);

    public void PostGroundingUpdate(float deltaTime);

    public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport);

    public void UpdateRotation(ref Quaternion currentRotation, float deltaTime);


    public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime);

}
