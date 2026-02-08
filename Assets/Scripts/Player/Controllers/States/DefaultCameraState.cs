using System;
using UnityEngine;

public class DefaultCameraState : CameraBehaviourBase
{
    
    public DefaultCameraState(PlayerCamera playerCamera) : base(playerCamera)
    {
        
    }
    public override void Enter()
    {
        playerCamera._eulerAngles = playerCamera.transform.eulerAngles;
    }
    public override void ProcessInput(CameraInput input)
    {
        this.input = input;

            if(input.LockOn)
            {
                if(playerCamera.TrylockOnTarget()) playerCamera.SetState(new LockOnCameraState(playerCamera) );
            }
        
    }

    public override void CameraUpdate()
    {
        
        playerCamera._eulerAngles += new Vector3(-input.Look.y * playerCamera._gain[0], input.Look.x * playerCamera._gain[1]) * playerCamera._sensibility;
        playerCamera._eulerAngles.x = Mathf.Clamp(playerCamera._eulerAngles.x, -50f, 50f);
        playerCamera.transform.eulerAngles = playerCamera._eulerAngles;
    }
}
