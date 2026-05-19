using UnityEngine;

public class EnemyLayer : MonoBehaviour
{
    AttackColliderHandler AttackColliderHandler;
    public int layerNorth;
    public int layerSouth;
    public int layerEast;
    public int layerWest;
    void Start()
    {
        AttackColliderHandler = GetComponent<AttackColliderHandler>();
        AttackColliderHandler.OnAttack += ChangeLayer;
    }

    void ChangeLayer()
    {
        Camera mainCam = Camera.main;
        if (mainCam == null) return;

        Vector3 viewportPos = mainCam.WorldToViewportPoint(transform.position);
        if (viewportPos.z < 0) return;
        float x = viewportPos.x - 0.5f;
        float y = viewportPos.y - 0.5f;
        if(Mathf.Abs(y) > Mathf.Abs(x))
        {
            if (y > 0) gameObject.layer = layerNorth;
            else gameObject.layer = layerSouth;
        }
        else
        {
            if (x > 0) gameObject.layer = layerEast;
            else gameObject.layer = layerWest;
        }
    }
}
