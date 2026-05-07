using UnityEngine;
using Unity.Cinemachine;
public class PlayersCamera : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] LayerMask layers;
    [SerializeField] float wallOffset;
    [SerializeField] float smoothingIn;
    [SerializeField] float smoothingOut;
    [SerializeField] float maxDistance;
    [SerializeField] float sphereRadius;

    CinemachineOrbitalFollow orbitalFollow;
    float currentDistance;
    float targetDistance;

    void Start()
    {
        orbitalFollow = GetComponent<CinemachineOrbitalFollow>();
        currentDistance = maxDistance;
    }
    void LateUpdate()
    {
        RayPoints();
    }

    void RayPoints()
    {
        if (target == null || orbitalFollow == null) return;

        Vector3 desiredCameraPos = Camera.main.transform.position;
        Vector3 dir = (desiredCameraPos - target.position).normalized;
        RaycastHit hit;
        if (Physics.SphereCast(target.position, sphereRadius, dir, out hit, maxDistance, layers))
        {
            targetDistance = hit.distance - wallOffset;
        }
        else
        {
            targetDistance = maxDistance;
        }
        float lerSpeed = (targetDistance < currentDistance) ? smoothingIn : smoothingOut;
        currentDistance = Mathf.Lerp(currentDistance, targetDistance, Time.deltaTime * lerSpeed);
        orbitalFollow.Radius = Mathf.Max(0.1f, currentDistance);
    }
}
