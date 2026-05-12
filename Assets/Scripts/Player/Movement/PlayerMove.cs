using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;
public class PlayerMove : MonoBehaviour
{
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
    [SerializeField] float sideStepDistance;
    [SerializeField] float sideStepDuration;
    [SerializeField] LayerMask layers;
    bool sideStepActivated;
    [SerializeField] float timerSideStep;
    [SerializeField] float maxTimerSideStep;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        InputsParent.Instance.SideStepInput().performed += SideStep;
        StartCoroutine(SideStepOn());
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
            Movement();

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
            if(camForward != Vector3.zero)
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
        if (!sideStepActivated)
        {
            sideStepActivated = true;
            /*Vector3 direction = transform.forward;
            direction.y = 0;
            direction.Normalize();
            if (Physics.Raycast(transform.position, direction, out RaycastHit hit, sideStepDistance, layers))
            {
                float safeDis = hit.distance - 0.5f;
                if (safeDis < 0) safeDis = 0;
                rb.AddForce(direction * safeDis, ForceMode.Impulse);
            }
            else
            {
                rb.AddForce(direction * sideStepForce, ForceMode.Impulse);
                sideStepActivated = true;
            }    */
        }
    }
    IEnumerator SideStepOn()
    {
        while (true)
        {
            yield return null;
            while (sideStepActivated)
            {
                Vector3 direction = transform.forward;
                float startTime = Time.time;
                rb.linearVelocity = Vector3.zero;
                if (Physics.Raycast(transform.position, direction, out RaycastHit hit, sideStepDistance, layers))
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
    void TimerSideStep()
    {
        if (sideStepActivated)
        {
            timerSideStep += Time.deltaTime;
            if(timerSideStep >= maxTimerSideStep)
            {
                sideStepActivated = false;
                timerSideStep = 0;
            }
        }
    }
    private void GetInput()
    {
        moveXfloat = InputsParent.Instance.MoveXInput().ReadValue<float>();
        moveZfloat = InputsParent.Instance.MoveZInput().ReadValue<float>();
    }
}
