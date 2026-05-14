using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using Unity.Cinemachine;
using System.Collections.Generic;
public class PlayerMove : MonoBehaviour
{
    public enum MovementType { freeMove, lockMove }
    public MovementType moveType;
    Rigidbody rb;
    [Header("Input Values")]
    [HideInInspector] public float moveXfloat;
    [HideInInspector] public float moveZfloat;

    [Header("Movement Variables")]
    [SerializeField] private float moveSpeed;
    [Header("Rotationt Variables")]
    [SerializeField] float rotateSpeed;
    [SerializeField] Transform cam;
    [Header("SideStep Variables")]
    [SerializeField] float sideStepForce;
    [SerializeField] float sideStepDistanceToWall;
    [SerializeField] float sideStepDuration;
    [SerializeField] LayerMask layers;
    bool sideStepActivated, sideStepStart;
    [SerializeField] float timerSideStep;
    [SerializeField] float maxTimerSideStep;
    [Header("LockOn Variables")]
    [SerializeField] CinemachineTargetGroup targetGroup;
    [SerializeField] float lockOnRadius;
    [SerializeField] LayerMask lockOnLayer;
    GameObject cameraTarget;
    public static bool isLockOn;
    Transform currentEnemy;
    [Header("Block Variables")]
    public static bool blocking;
    Transform currentEnemyBlocking;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        InputsParent.Instance.SideStepInput().started += SideStep;
        InputsParent.Instance.LockOnInput().performed += LockOn;
        InputsParent.Instance.LockOnInput().canceled += UnLockOn;
        InputsParent.Instance.BlockInput().performed += Block;
        InputsParent.Instance.BlockInput().canceled += UnBlock;
        StartCoroutine(SideStepOn());
        cameraTarget = transform.Find("CameraTarget").gameObject;
    }
    void Update()
    {
        GetInput();
        SpeedLimit();
        TimerSideStep();
    }
    private void FixedUpdate()
    {
        if (!sideStepActivated)
        {
            ChangeMovement();
        }
        if(blocking)Blocking();
    }
    private void ChangeMovement()
    {
        if (blocking) return;
        if (isLockOn)
        {
            moveType = MovementType.lockMove;
        }
        else
        {
            moveType = MovementType.freeMove;
        }
        switch (moveType)
        {
            case MovementType.freeMove:
                Movement();
                break;
            case MovementType.lockMove:
                LockOnMovement();
                break;
            default:
                Movement();
                break;
        }
    }
    private void Movement()
    {
        Vector3 direction = new Vector3(moveXfloat, 0, moveZfloat).normalized;
        Vector3 vel = Vector3.zero;
        if (direction.magnitude >= 0.1f)
        {
            Vector3 camForward = cam.forward;
            Vector3 camRight = cam.right;
            camForward.y = 0;
            camForward.Normalize();
            camRight.y = 0;
            camRight.Normalize();
            Vector3 moveDirection = (camForward * direction.z + camRight * direction.x).normalized;
            if (camForward != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, rotateSpeed * Time.fixedDeltaTime));
            }
            vel = moveDirection * moveSpeed;
        }

        vel.y = rb.linearVelocity.y;
        rb.linearVelocity = vel;
    }
    private void SpeedLimit()
    {
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        if (flatVel.magnitude >= moveSpeed)
        {
            Vector3 limitVel = flatVel.normalized * moveSpeed;
            rb.linearVelocity = new Vector3(limitVel.x, rb.linearVelocity.y, limitVel.z);
            //rb.linearVelocity = Vector3.ClampMagnitude(rb.linearVelocity, maxSpeed);
        }
    }

    void SideStep(InputAction.CallbackContext context)
    {
        if (!sideStepStart)
        {
            sideStepActivated = true;
            sideStepStart = true;
        }
    }
    IEnumerator SideStepOn()
    {
        while (true)
        {
            yield return null;
            while (sideStepActivated)
            {
                Vector3 direction = Vector3.zero;
                switch (moveType)
                {
                    case MovementType.freeMove:
                        direction = NormalSideStep();
                        break;
                    case MovementType.lockMove:
                        direction = LockOnSideStep();
                        break;
                    default:
                        direction = NormalSideStep();
                        break;
                }


                float startTime = Time.time;
                rb.linearVelocity = Vector3.zero;
                if (Physics.Raycast(transform.position, direction, out RaycastHit hit, sideStepDistanceToWall, layers))
                {
                    float safeDis = hit.distance - 0.5f;
                    if (safeDis < 0) safeDis = 0;
                    while (Time.time < startTime + sideStepDuration)
                    {
                        rb.linearVelocity = direction * safeDis;
                        yield return null;
                    }
                }
                else
                {
                    while (Time.time < startTime + sideStepDuration)
                    {
                        rb.linearVelocity = direction * sideStepForce;
                        yield return null;
                    }
                    rb.linearVelocity = Vector3.zero;
                }
            }
        }
    }
    Vector3 NormalSideStep()
    {
        Vector3 direction = transform.forward;
        return direction;
    }
    Vector3 LockOnSideStep()
    {
        Vector3 input = new Vector3(moveXfloat, 0, moveZfloat).normalized;
        Vector3 direction = Vector3.zero;
        if (input.magnitude >= 0.1f)
        {
            direction = (transform.forward * moveZfloat + transform.right * moveXfloat).normalized;
        }
        else
        {
            direction = transform.forward;
        }
        return direction;
    }
    void TimerSideStep()
    {
        if (sideStepStart)
        {
            timerSideStep += Time.deltaTime;
            if (timerSideStep >= sideStepDuration)
            {
                sideStepActivated = false;
            }
            if (timerSideStep >= maxTimerSideStep)
            {
                sideStepStart = false;
                timerSideStep = 0;

            }
        }
    }
    void LockOnMovement()
    {
        if (isLockOn && currentEnemy != null)
        {
            LockRotation(currentEnemy);
            Vector3 moveVec = (transform.forward * moveZfloat + transform.right * moveXfloat).normalized;
            rb.linearVelocity = new Vector3(moveVec.x * moveSpeed, rb.linearVelocity.y, moveVec.z * moveSpeed);
        }
    }
    void LockRotation(Transform currentEnemy)
    {
        Vector3 directionToEnemy = (currentEnemy.position - transform.position).normalized;
        directionToEnemy.y = 0;
        Quaternion targetRot = Quaternion.LookRotation(directionToEnemy);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotateSpeed * Time.fixedDeltaTime);
    }
    void Block(InputAction.CallbackContext context)
    {
        blocking = true;
        rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
        currentEnemyBlocking = GetClosestEnemy();
        /*Debug.Log("Bloqueando");
        Debug.Log(blocking);*/
        
    }
    void UnBlock(InputAction.CallbackContext context)
    {
        blocking = false;
    }
    void Blocking()
    {
        if (!sideStepActivated)
        {
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
        }
        
        if (currentEnemyBlocking != null)
        {
            LockRotation(currentEnemyBlocking);
        }
    }
    void LockOn(InputAction.CallbackContext context)
    {
        //Debug.Log("Input LockOn");
        currentEnemy = GetClosestEnemy();
        if (currentEnemy == null) return;
        isLockOn = true;
        //Debug.Log("Lockeando");
        targetGroup.Targets.Clear();
        targetGroup.AddMember(cameraTarget.transform, 2f, 6f);
        targetGroup.AddMember(currentEnemy, 1f, 3f);
        if (EnemyCombatGroup.Instance.GetCurrentAttackers().Count > 0) EnemyCombatGroup.Instance.InvokeChangeCurrentAttackers();
    }
    void UnLockOn(InputAction.CallbackContext context)
    {
        isLockOn = false;
        currentEnemy = null;
        targetGroup.Targets.Clear();
        targetGroup.AddMember(cameraTarget.transform, 2f, 6f);
        if(EnemyCombatGroup.Instance.GetCurrentAttackers().Count > 0) EnemyCombatGroup.Instance.InvokeChangeCurrentAttackers();
    }
    Transform GetClosestEnemy()
    {
        //Debug.Log("gettingClosestEnemy");
        Collider[] hits = Physics.OverlapSphere(transform.position, lockOnRadius, lockOnLayer);
        Transform closest = null;
        float closestDis = Mathf.Infinity;
        Camera main = Camera.main;

        foreach(Collider hit in hits)
        {
            float dis = Vector3.Distance(transform.position, hit.transform.position);
            if(dis < closestDis)
            {
                closestDis = dis;
                closest = hit.transform;
            }
        }
        return closest;
    }
    private void GetInput()
    {
        moveXfloat = InputsParent.Instance.MoveXInput().ReadValue<float>();
        moveZfloat = InputsParent.Instance.MoveZInput().ReadValue<float>();
    }
}
