using UnityEngine;
public class TransparencyWithCamera : MonoBehaviour
{
    [SerializeField] GameObject player;

    [SerializeField] Material baseColor;
    [SerializeField] Color myColor;
    [SerializeField]
    [Range(0f, 1f)]
    float myAlpha = 1f;

    [SerializeField] float initialDistance;
    void Start()
    {
        baseColor = player.GetComponent<MeshRenderer>().material;
        baseColor.color = myColor;
        
    }
    bool start;
    void Update()
    {
        ChangeBaseColor();
    }

    [SerializeField] float dis;
    [SerializeField] float distanceToGoZero, distanceToStartTransparency;
    void ChangeBaseColor()
    {
        if (!start)
        {
            initialDistance = Vector3.Distance(gameObject.transform.position, player.transform.position);
            start = true;
        }
        dis = Vector3.Distance(gameObject.transform.position, player.transform.position);
        if(Vector3.Distance(gameObject.transform.position, player.transform.position) < initialDistance / distanceToGoZero)
        {
            myColor.a = 0;
            baseColor.SetColor("_Base_Color", myColor);
        }
        else if (Vector3.Distance(gameObject.transform.position, player.transform.position) < initialDistance / distanceToStartTransparency)
        {
            myAlpha = Vector3.Distance(gameObject.transform.position , player.transform.position) / initialDistance;
            myColor.a = myAlpha;
            baseColor.SetColor("_Base_Color", myColor);
        }
        else
        {
            myColor.a = 1;
            baseColor.SetColor("_Base_Color", myColor);
        }
        
    }
}
