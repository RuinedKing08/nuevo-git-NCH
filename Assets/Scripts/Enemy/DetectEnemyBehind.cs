using UnityEngine;

public class DetectEnemyBehind : MonoBehaviour
{
    GameObject alertSymbol;
    [SerializeField] float radius;
    [SerializeField] float distanceToCamera;
    Camera mainCam;
    Transform player;
    bool startBehind, finishBehind;
    void Start()
    {
        alertSymbol = GameObject.Find("Canvas").transform.Find("AlertSymbol").gameObject;
        player = GameObject.Find("PlayerGo").transform;
        mainCam = Camera.main;
        EnemyBehindHUD.Instance.OnChangeEnemyBehindCount += ChangeAlertSymbol;
    }

    void Update()
    {
        ActivateAlertSymbol();
    }

    void ActivateAlertSymbol()
    {
        Vector3 viewportPos = mainCam.WorldToViewportPoint(transform.position);
        if (!startBehind)
        {
            if (Vector3.Distance(transform.position, player.position) <= radius && viewportPos.z <= distanceToCamera)
            {
                EnemyBehindHUD.Instance.ChangeEnemiesBehindCount(1);
                startBehind = true;
                finishBehind = false;
            }
        }
        else if (!finishBehind)
        {
            if(viewportPos.z > 0)
            {
                EnemyBehindHUD.Instance.ChangeEnemiesBehindCount(-1);
                startBehind = false;
                finishBehind = true;
            }
        }
    }
    void ChangeAlertSymbol()
    {
        if (EnemyBehindHUD.Instance.GetEnemiesBehindCount() >= 1)
        {
            alertSymbol.SetActive(true);
        }
        else
        {
            alertSymbol.SetActive(false);
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
