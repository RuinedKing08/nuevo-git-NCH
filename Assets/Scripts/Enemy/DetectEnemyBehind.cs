using UnityEngine;
using UnityEngine.UI;

public class DetectEnemyBehind : MonoBehaviour
{
    GameObject alertSymbol;
    [SerializeField] float radius;
    [SerializeField] float distanceToCamera;
    Camera mainCam;
    Transform player;
    bool startBehind, finishBehind;
    Image[] alertSymbols;
    EnemyController thisEnemy;
    void Start()
    {
        alertSymbol = GameObject.Find("Canvas").transform.Find("AlertSymbol").gameObject;
        alertSymbols = alertSymbol.GetComponentsInChildren<Image>();
        player = GameObject.Find("PlayerGo").transform;
        thisEnemy = GetComponent<EnemyController>();
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
                if (EnemyCombatGroup.Instance.GetCurrentAttackers().Contains(thisEnemy))
                {
                    for(int i = 0; i < alertSymbols.Length; i++)
                    {
                        alertSymbols[i].color = Color.red;
                    }
                }
                else
                {
                    EvaluateOtherEnemies();
                }
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
                //EvaluateOtherEnemies();
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
    bool ExistCurrentAttackersBehind()
    {
        var allEnemies = EnemyCombatGroup.Instance.GetCurrentMembers();
        var currentAttackers = EnemyCombatGroup.Instance.GetCurrentAttackers();
        foreach(var enemy in allEnemies)
        {
            if (enemy == thisEnemy) continue;
            Vector3 viewportPos = mainCam.WorldToViewportPoint(transform.position);
            if(Vector3.Distance(transform.position, player.position) <= radius && viewportPos.z <= distanceToCamera && currentAttackers.Contains(enemy))
            {
                return true;
            }
        }
        return false;
    }
    void EvaluateOtherEnemies()
    {
        if (ExistCurrentAttackersBehind())
        {
            for (int i = 0; i < alertSymbols.Length; i++)
            {
                alertSymbols[i].color = Color.red;
            }
        }
        else
        {
            for (int i = 0; i < alertSymbols.Length; i++)
            {
                alertSymbols[i].color = Color.yellow;
            }
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
