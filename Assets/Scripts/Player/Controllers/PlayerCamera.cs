using System;
using Unity.Cinemachine;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public Transform _CMcamera;
    public Camera MainCamera;

    [Header("Runtime Settings")]
    public float[] _gain = new float[2];
    [NonSerialized]public float _sensibility = 1f;
    [NonSerialized]public Vector3 _eulerAngles;

    public float rotationSpeed;
    public float maxLockOnDistance;
    public LayerMask lockOnLayer;

    private PlayerState tempstate;
    public PlayerState TempState => tempstate;

    [SerializeField] public Transform lockOnTarget;

    ICameraBehaviourState currentState;

    public bool inSight;

    

    public void Initialize(Transform Target)
    {
        transform.position = Target.position;
        transform.rotation = Target.rotation;
        transform.eulerAngles = _eulerAngles = Target.eulerAngles;

        SetState(new DefaultCameraState(this));
      

    }

    public void ProcessInput(CameraInput input)
    {
        currentState?.ProcessInput(input);

    }

    public void BeforeCameraUpdate()
    {
        currentState?.BeforeCameraUpdate();
    }

    public void CameraUpdate()
    {
        currentState.CameraUpdate();
    }
 
    public void UpdatePosition(Transform Target)
    {
        transform.position = Target.position;
    }

    public void SetState(ICameraBehaviourState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState.Enter();
        Debug.Log("Settign state " + tempstate.cameraState);
    }

    public bool TrylockOnTarget()
    {
        Collider[] candidates = Physics.OverlapSphere
           (
            transform.position,
            maxLockOnDistance,
            lockOnLayer
           );

        if (candidates.Length == 0)
        {
            lockOnTarget = null;
            return false;
        }
        float bestScore = float.MaxValue;
        Transform bestTarget = null;

        Vector2 screenCenter = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);

        foreach (Collider target in candidates)
        {
            Vector3 direction = target.transform.position - transform.position;

            if (Vector3.Dot(transform.forward, direction) <= 0f) continue;

            float distance = Vector3.Distance(transform.position, target.transform.position);

            if (distance > maxLockOnDistance)
                continue;

            Vector3 screenPos = MainCamera.WorldToScreenPoint(target.transform.position);

            if (screenPos.x <= 0 || screenPos.y < 0 || screenPos.z < 0 || screenPos.x > Screen.width || screenPos.y > Screen.height)
            {
                continue;
            }

            float screenDistance = Vector2.Distance(new Vector2(screenPos.x, screenPos.y), screenCenter);

            float normalizedScreen = screenDistance / (Screen.width * 0.5f);

            float normalizedDistance = distance / maxLockOnDistance;

            float screenWeight = 0.7f;
            float distanceWeight = 0.3f;

            float score = normalizedScreen * screenWeight +
                          normalizedDistance * distanceWeight;

            if (score < bestScore)
            {
                bestScore = score;
                bestTarget = target.transform;
            }
        }

        lockOnTarget = bestTarget;
        return lockOnTarget != null;
       
    } 
}
