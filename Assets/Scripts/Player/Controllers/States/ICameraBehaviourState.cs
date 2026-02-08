using UnityEngine;

public interface ICameraBehaviourState
{
    void Enter();
    void Exit();

    void ProcessInput(CameraInput input);
    void BeforeCameraUpdate();
    void CameraUpdate();
}
