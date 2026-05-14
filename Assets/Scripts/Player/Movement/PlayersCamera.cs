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
    CinemachineInputAxisController axisController;
    CinemachineGroupFraming groupFraming;
    [SerializeField] CinemachineTargetGroup targetGroup;
    float currentDistance;
    float targetDistance;

    void Start()
    {
        orbitalFollow = GetComponent<CinemachineOrbitalFollow>();
        axisController = GetComponent<CinemachineInputAxisController>();
        groupFraming = GetComponent<CinemachineGroupFraming>();
        currentDistance = maxDistance;
        EnemyCombatGroup.Instance.OnChangeCurrentAttackers += FindTargetsGroup;
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

    void FindTargetsGroup()
    {
        if (!PlayerMove.isLockOn)
        {
            EnemyCombatGroup.Instance.GetCurrentAttackers();
            targetGroup.Targets.Clear();
            targetGroup.AddMember(target, 2f, 6f);
            for (int i = 0; i < EnemyCombatGroup.Instance.GetCurrentAttackers().Count; i++)
            {
                targetGroup.AddMember(EnemyCombatGroup.Instance.GetCurrentAttackers()[i].transform, 1f, 1f);
            }
            if(EnemyCombatGroup.Instance.GetCurrentAttackers().Count <= 1)
            {
                orbitalFollow.VerticalAxis.Recentering.Enabled = true;
                orbitalFollow.VerticalAxis.Recentering.Wait = 1f;
                orbitalFollow.VerticalAxis.Recentering.Time = 2f;
                groupFraming.enabled = false;
                SetOrbitYEnabled(true);
            }
            else
            {

                orbitalFollow.VerticalAxis.Recentering.Enabled = true;
                orbitalFollow.VerticalAxis.Recentering.Wait = 0f;
                orbitalFollow.VerticalAxis.Recentering.Time = 0f;
                groupFraming.enabled = true;
                SetOrbitYEnabled(false);
            }
        }
        else
        {
            orbitalFollow.VerticalAxis.Recentering.Enabled = true;
            orbitalFollow.VerticalAxis.Recentering.Wait = 0f;
            orbitalFollow.VerticalAxis.Recentering.Time = 0f;
            groupFraming.enabled = true;
            SetOrbitYEnabled(false);
        }
    }
    void SetOrbitYEnabled(bool isEnabled)
    {
        var orbitY = axisController.Controllers.Find(c => c.Name.ToUpper().Contains("Y"));
        if (orbitY != null)
        {
            orbitY.Enabled = isEnabled;
           // Debug.Log("Fnciona?");
        }
        else
        {
            foreach (var controller in axisController.Controllers)
            {
               // Debug.Log($"Ejes disponibles: {controller.Name}");
            }
        }
    }
    private void OnDestroy()
    {
        EnemyCombatGroup.Instance.OnChangeCurrentAttackers -= FindTargetsGroup;
    }
}
