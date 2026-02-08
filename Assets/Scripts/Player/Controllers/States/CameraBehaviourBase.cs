using UnityEngine;
using UnityEngine.Windows;

public abstract class CameraBehaviourBase : ICameraBehaviourState
{
   protected PlayerCamera playerCamera;

    protected CameraInput input; 

    protected CameraBehaviourBase(PlayerCamera playerCamera)
    {
        this.playerCamera = playerCamera;
    }

    public virtual void BeforeCameraUpdate()
    {
        
    }

    public virtual void CameraUpdate()
    {
       
    }

    public virtual void Enter()
    {
        
    }

    public virtual void Exit()
    {
      
    }

    public virtual void ProcessInput(CameraInput input)
    {
       this.input = input;
    }
}
