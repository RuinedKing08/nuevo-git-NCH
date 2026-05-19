using UnityEngine;

public class PlayerRotate : MonoBehaviour
{
    [SerializeField] float rotateSpeed;
    [SerializeField] LayerMask layer;
    void Start()
    {
        
    }

    void Update()
    {
        
    }
    private void FixedUpdate()
    {
        LookAtMouse();
    }

    void LookAtMouse()
    {
        Camera mainCam = Camera.main;
        if (mainCam == null) return;
        Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if(Physics.Raycast(ray,out hit, Mathf.Infinity, layer))
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
}
