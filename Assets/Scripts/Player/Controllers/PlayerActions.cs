using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using Unity.Cinemachine;
public class PlayerActions : MonoBehaviour
{
    [Header("Rotation Variables")]
    [SerializeField] float rotateSpeed;
    [SerializeField] LayerMask layer;
    [Header("Blocl Variables")]
    [SerializeField] bool startBlocking;
    public static bool blocking;
    [SerializeField] float timerBlocking;
    void Start()
    {
        InputsParent.Instance.BlockInput().performed += Block;
        InputsParent.Instance.BlockInput().canceled += UnBlock;
    }

    void Update()
    {

    }
    private void FixedUpdate()
    {
        LookAtMouse();
        TimerStartBlocking();
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
}
