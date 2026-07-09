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
     bool attackBehind;
    void Start()
    {
        alertSymbol = GameObject.Find("Canvas").transform.Find("AlertSymbol").gameObject;
        alertSymbols = alertSymbol.GetComponentsInChildren<Image>();
        player = GameObject.Find("PlayerGo").transform;
        thisEnemy = GetComponentInParent<EnemyController>();
        mainCam = Camera.main;
        EnemyBehindHUD.Instance.OnChangeEnemyBehindCount += ChangeAlertSymbol;
    }

    void Update()
    {
        ActivateAlertSymbol();
    }
    public void ToEventStartAttack()
    {
        attackBehind = true;
        UpdateAlertColor();
    }
    public void ToEventFinishtAttack()
    {
        attackBehind = false;
        UpdateAlertColor();
    }
    public void ActivateAlertSymbol()
    {
        Vector3 viewportPos = mainCam.WorldToViewportPoint(transform.position);
        if (!startBehind)
        {
            if (Vector3.Distance(thisEnemy.transform.position, player.position) <= radius && viewportPos.z <= distanceToCamera)
            {
                EnemyBehindHUD.Instance.ChangeEnemiesBehindCount(1);
                startBehind = true;
                finishBehind = false;
                UpdateAlertColor();
            }
        }
        else if (!finishBehind)
        {
            if(viewportPos.z > 0.75f)
            {
                EnemyBehindHUD.Instance.ChangeEnemiesBehindCount(-1);
                startBehind = false;
                finishBehind = true;
                attackBehind = false;
                EvaluateOtherEnemies();
            }
        }
    }
    public void UpdateAlertColor()
    {       
        bool enemyAttackingBehind = false;
        var allEnemies = EnemyCombatGroup.Instance.GetCurrentMembers();
        foreach (var enemy in allEnemies)
        {
            if (enemy.GetComponentInChildren<DetectEnemyBehind>().attackBehind && !enemy.GetComponentInChildren<DetectEnemyBehind>().finishBehind &&
                enemy.GetComponentInChildren<DetectEnemyBehind>().startBehind)
            {
                enemyAttackingBehind = true;
                break;
            }
        }
        foreach (var enemy in allEnemies)
        {
            if(enemy.GetComponentInChildren<DetectEnemyBehind>().startBehind && !enemy.GetComponentInChildren<DetectEnemyBehind>().finishBehind)
            {
                if (enemyAttackingBehind)
                {
                    SetAlertColor(Color.red);
                }
                else
                {
                    SetAlertColor(Color.yellow);
                }
            }
        }

    }
    public void SetAlertColor(Color color)
    {
        for (int i = 0; i < alertSymbols.Length; i++)
        {
            alertSymbols[i].color = color;
        }
    }
    public void ChangeAlertSymbol()
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
    public bool ExistCurrentAttackersBehind()
    {
        var allEnemies = EnemyCombatGroup.Instance.GetCurrentMembers();
        var currentAttackers = EnemyCombatGroup.Instance.GetCurrentAttackers();
        foreach(var enemy in allEnemies)
        {
            if (enemy == thisEnemy) continue;
            Vector3 viewportPos = mainCam.WorldToViewportPoint(enemy.transform.position);
            if(Vector3.Distance(transform.position, player.position) <= radius && viewportPos.z <= distanceToCamera && enemy.GetComponentInChildren<DetectEnemyBehind>().attackBehind)
            {
                return true;
            }
        }
        return false;
    }
    public void EvaluateOtherEnemies()
    {
        var allEnemies = EnemyCombatGroup.Instance.GetCurrentMembers();
        foreach (var enemy in allEnemies)
        { 
            if (enemy.GetComponentInChildren<DetectEnemyBehind>().attackBehind && !enemy.GetComponentInChildren<DetectEnemyBehind>().finishBehind)
            {
                enemy.GetComponentInChildren<DetectEnemyBehind>().UpdateAlertColor();
            }
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
