using UnityEngine;

public class DefaultCharacterState : CharacterMovementStateBase
{
    
    public DefaultCharacterState(PlayerCharacter character) : base(character) { }

    public override void Enter()
    {
        
        character._characterState.behaviourState = CharacterBehaviourState.DEFAULT;
    }

    public override void ProcessInput(characterMotorInput input)
    {
        _requestedRotation = input.Rotation;
        _requestedMovement = new Vector3(input.Move.x, 0f, input.Move.y);
        _requestedMovement = Vector3.ClampMagnitude(_requestedMovement, 1f);
        _rawInput = _requestedMovement;

        _requestedMovement = input.Rotation * _requestedMovement;

        var wasResquestedJump = _requestedJump;
        _requestedJump = _requestedJump || input.Jump;
        if (_requestedJump && wasResquestedJump)
        {
            character._timeSinceJumpRequest = 0f;
        }

     

    }

    public override void UpdateBody()
    {

    }

    public override void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
    {

        var forward = Vector3.ProjectOnPlane(
        _requestedRotation * Vector3.forward,
         character.Motor.CharacterUp
        );

        currentRotation = Quaternion.LookRotation(forward, character.Motor.CharacterUp);
    }

    public override void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
    {
        character._characterState.Acceleration = Vector3.zero;
        if (character.Motor.GroundingStatus.IsStableOnGround)
        {
            character._timeSinceUngrounded = 0f;
            character._ungroundedDueToJump = false;

            var groundedMovement = character.Motor.GetDirectionTangentToSurface
            (
              direction: _requestedMovement,
              surfaceNormal: character.Motor.GroundingStatus.GroundNormal
            ) * _requestedMovement.magnitude;




            float speed = character._characterState.Stance is Stance.Stand ? character.StandSettings.speed : character.CrouchSettings.speed;

            float response = character._characterState.Stance is Stance.Stand ? character.StandSettings.response : character.CrouchSettings.response;

            Vector3 targetVelocity = groundedMovement * speed;
            Vector3 moveVelocity = Vector3.Lerp
                (
                    a: currentVelocity,
                    b: targetVelocity,
                    t: 1f - Mathf.Exp(-response * deltaTime)
                );
            character._characterState.Acceleration = moveVelocity - currentVelocity;
            currentVelocity = moveVelocity;

            character._characterState.isGrounded = true;
            character._characterState.isJumping = false;
            character._characterState.hasStopped = _rawInput.magnitude > 0.01f ? false : true;
            var clampMoveSpeed = Vector3.ClampMagnitude(currentVelocity, 7f);
            character._characterState.MoveSpeed = clampMoveSpeed.magnitude;
            character._characterState.inputDirection = new Vector2(_rawInput.x, _rawInput.z);
          

        }
        else // in the air
        {
            character._characterState.isGrounded = false;
            character._timeSinceUngrounded += deltaTime;
            if (_requestedMovement.sqrMagnitude > 0f)
            {
                var planarMovement = Vector3.ProjectOnPlane
                (
                    vector: _requestedMovement,
                    planeNormal: character.Motor.CharacterUp
                ) * _requestedMovement.magnitude;

                var currentPlanarVelocity = Vector3.ProjectOnPlane
                (
                    vector: currentVelocity,
                    planeNormal: character.Motor.CharacterUp
                );
                
                var movementForce = planarMovement * character.AirSettings.AirAcceleration * deltaTime;

                if (currentPlanarVelocity.magnitude < character.AirSettings.AirSpeed)
                {
                    var targetPlanarVelocity = currentPlanarVelocity + movementForce;

                    targetPlanarVelocity = Vector3.ClampMagnitude(targetPlanarVelocity, character.AirSettings.AirSpeed);
                    movementForce = targetPlanarVelocity - currentPlanarVelocity;
                }

                else if (Vector3.Dot(currentPlanarVelocity, movementForce) > 0f)
                {
                    var contrainedMovementForce = Vector3.ProjectOnPlane
                    (
                        vector: movementForce,
                        planeNormal: currentPlanarVelocity.normalized
                    );
                    movementForce = contrainedMovementForce;

                }

                if (character.Motor.GroundingStatus.FoundAnyGround) // prevent wall climbing in the air
                {
                    if (Vector3.Dot(movementForce, currentVelocity + movementForce) > 0f)
                    {
                        var obstructedNormal = Vector3.Cross
                        (
                            character.Motor.CharacterUp,
                            Vector3.Cross
                            (
                                character.Motor.CharacterUp,
                                character.Motor.GroundingStatus.GroundNormal
                            )
                        ).normalized;
                        movementForce = Vector3.ProjectOnPlane(movementForce, obstructedNormal);
                    }
                }


                currentVelocity += movementForce;
            }
            currentVelocity += character.Motor.CharacterUp * character.AirSettings.Gravity * deltaTime;

        }

        if (_requestedJump)
        {
            var grounded = character.Motor.GroundingStatus.IsStableOnGround;
            bool canCoyoteTime = character._timeSinceUngrounded < character.AirSettings.CoyoteTime && !character._ungroundedDueToJump;
            if (grounded || canCoyoteTime)
            {
                Debug.Log("Jumping");
                _requestedJump = false;
                character._characterState.isJumping = true;


                character.Motor.ForceUnground(time: 0.1f);
                character._ungroundedDueToJump = true;

                var currentVerticalSpeed = Vector3.Dot(currentVelocity, character.Motor.CharacterUp);
                var targetVerticalSpeed = Mathf.Max(currentVerticalSpeed, character.AirSettings.JumpSpeed);

                currentVelocity += character.Motor.CharacterUp * (targetVerticalSpeed - currentVerticalSpeed);

            }
            else
            {
                character._timeSinceJumpRequest += deltaTime;
                bool canJumpLater = character._timeSinceJumpRequest < (character.AirSettings.CoyoteTime * 0.16);
                _requestedJump = canJumpLater;


            }
        }
        if (character.ExternalForces.magnitude > 0)
        {
            character.Motor.ForceUnground();

            currentVelocity += character.ExternalForces;
            character.SetExternalForces(Vector3.zero);
        }
    }

    

}
    


