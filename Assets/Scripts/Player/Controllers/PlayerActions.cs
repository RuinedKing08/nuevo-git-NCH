using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using Unity.Cinemachine;
public class PlayerActions : MonoBehaviour
{
    [Header("Input Values")]
    [HideInInspector] public float moveXfloat;
    [HideInInspector] public float moveZfloat;
    [Header("Rotation Variables")]
    [SerializeField] float rotateSpeed;
    [SerializeField] LayerMask layer;
    [Header("Block Variables")]
    [SerializeField] bool startBlocking;
    public static bool blocking;
    [SerializeField] float timerBlocking;
    public enum EvadeState { up,down,left,right}
    [Header("Evade Variables")]
    public static EvadeState evadeState;
    public static bool canEvading;
    public static bool evading;
    [SerializeField] float timerEvading;
    void Start()
    {
        evadeState = EvadeState.up;
        canEvading = true;
        InputsParent.Instance.BlockInput().performed += Block;
        InputsParent.Instance.BlockInput().canceled += UnBlock;
        InputsParent.Instance.SideStepInput().started += Evade;
    }

    void Update()
    {
        GetInput();
    }
    private void FixedUpdate()
    {
        LookAtMouse();
        TimerStartBlocking();
        TimerStartEvading();
    }

    void LookAtMouse()
    {
        if (startBlocking) return;
        Camera mainCam = Camera.main;
        if (mainCam == null) return;
        Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layer))
        {
            Vector3 lookDirection = hit.point - transform.position;
            lookDirection.y = 0;
            if (lookDirection.magnitude > 0.1f)
            {
                Quaternion targetRot = Quaternion.LookRotation(lookDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotateSpeed * Time.fixedDeltaTime);
            }
        }
    }

    void Block(InputAction.CallbackContext context)
    {
        if (evading) return;
        blocking = true;
        startBlocking = true;
        timerBlocking = 0;
    }

    void UnBlock(InputAction.CallbackContext context)
    {
        blocking = false;
        startBlocking = false;
    }

    void TimerStartBlocking()
    {
        if (startBlocking)
        {
            timerBlocking += Time.fixedDeltaTime;
            if (timerBlocking >= 1)
            {
                startBlocking = false;
            }
        }
        
    }
    void TimerStartEvading()
    {
        if (!canEvading)
        {
            timerEvading += Time.fixedDeltaTime;
            if (timerEvading >= 0.5f)
            {
                evading = false;
            }
            if (timerEvading >= 1)
            {
                canEvading = true;
            }
        }
        
    }

    void Evade(InputAction.CallbackContext context)
    {
        if (blocking) return;
        if (!canEvading) return;
        evading = true;
        timerEvading = 0;
        Vector3 direction = new Vector3(moveXfloat, 0, moveZfloat).normalized;
        switch (direction.x,direction.y,direction.z)
        {
            case (1,0,0):
                evadeState = EvadeState.right;
                break;
            case (-1,0,0):
                evadeState = EvadeState.left;
                break;
            case (0,0,1):
                evadeState = EvadeState.up;
                break;
            case (0,0,-1):
                evadeState = EvadeState.down;
                break;
            default:
                evadeState = EvadeState.up;
                break;
        }
        canEvading = false;
    }
    private void GetInput()
    {
        moveXfloat = InputsParent.Instance.MoveXInput().ReadValue<float>();
        moveZfloat = InputsParent.Instance.MoveZInput().ReadValue<float>();
    }
}
