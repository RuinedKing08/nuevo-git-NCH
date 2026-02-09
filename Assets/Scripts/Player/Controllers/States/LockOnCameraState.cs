using JetBrains.Annotations;
using UnityEngine;

public class LockOnCameraState : CameraBehaviourBase
{
    private Transform target;

    public LockOnCameraState(PlayerCamera playerCamera) : base(playerCamera)
    {
    }

    public override void Enter()
    {
        target = playerCamera.lockOnTarget;
        if(target == null)
        {
            playerCamera.SetState(new DefaultCameraState(playerCamera));
            return;
        }

       
    }

    public override void ProcessInput(CameraInput input)
    {
        this.input = input;
        if (GameSettings.Instance.lockOnSettings == InputSettings.Hold && !input.LockOn)  
        {
            playerCamera.SetState(new DefaultCameraState(playerCamera));
            return;
        } 
        else if (GameSettings.Instance.lockOnSettings == InputSettings.Toggle && input.LockOn)
        {
            playerCamera.SetState(new DefaultCameraState(playerCamera));
            return;
        } 
    }

    public override void Exit() 
    {
        Vector3 euler = playerCamera.transform.rotation.eulerAngles;

        // Convert Unity 0..360 into -180..180
        if (euler.x > 180f) euler.x -= 360f;
        if (euler.y > 180f) euler.y -= 360f;

        playerCamera._eulerAngles = euler;
    }
    public override void BeforeCameraUpdate()
    {
        if (target == null)
        {
            playerCamera.SetState(new DefaultCameraState(playerCamera));
            return;
        }
    }
    public override void CameraUpdate()
    {
       

        Vector3 direction = target.position - playerCamera.transform.position;

       
       

        Quaternion targetRotation = Quaternion.LookRotation(direction.normalized);


        
        playerCamera.transform.rotation = Quaternion.Slerp(playerCamera.transform.rotation,targetRotation, playerCamera.rotationSpeed * Time.deltaTime);
    }


}
