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

        playerCamera._eulerAngles = playerCamera.transform.eulerAngles;
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

        Vector3 euler = targetRotation.eulerAngles;

        playerCamera._eulerAngles = euler;
        playerCamera.transform.rotation = Quaternion.Slerp(playerCamera.transform.rotation,targetRotation, playerCamera.rotationSpeed * Time.deltaTime);
    }


}
