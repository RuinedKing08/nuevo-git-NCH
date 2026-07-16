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
    EnemyDetectionArea thisEnemy;
     bool attackBehind;
    void Start()
    {
        alertSymbol = GameObject.Find("Canvas").transform.Find("AlertSymbol").gameObject;
        alertSymbols = alertSymbol.GetComponentsInChildren<Image>();
        player = GameObject.Find("PlayerGo").transform;
        thisEnemy = GetComponentInParent<EnemyDetectionArea>();
        mainCam = Camera.main;
        EnemyBehindHUD.Instance.OnChangeEnemyBehindCount += ChangeAlertSymbol;
    }

    void Update()
    {
        ActivateAlertSymbol();
    }
    public void ToEventStartAttack()
    {
        if (TutorialBool.tutorial) Tutorial();
        attackBehind = true;
        finishBehind = false;
        startBehind = false;
    }
    void Tutorial()
    {
        Time.timeScale = 0f;
        DialoguesTutorial.Instance.ActivePanel();
        DialoguesTutorial.Instance.StartDialogue();

    }
    public void ToEventFinishtAttack()
    {
        if (startBehind && !finishBehind){ EnemyBehindHUD.Instance.ChangeEnemiesBehindCount(-1); startBehind = false; finishBehind = true; }
        attackBehind = false;
    }
    public void ActivateAlertSymbol()
    {
        Vector3 viewportPos = mainCam.WorldToViewportPoint(transform.position);
        /*if(viewportPos.z <= 0)
        {
            Debug.Log($"Enemigo en la espalda. Distancia exacta a la cámara (Z): {Mathf.Abs(viewportPos.z)}");
        }*/
        if (!startBehind)
        {
            if (Vector3.Distance(thisEnemy.transform.position, player.position) <= radius && viewportPos.z <= distanceToCamera)
            {
                if (attackBehind)
                {
                    EnemyBehindHUD.Instance.ChangeEnemiesBehindCount(1);
                    startBehind = true;
                    finishBehind = false;
                }
            }
        }
        else if (!finishBehind)
        {
            if(viewportPos.z > distanceToCamera)
            {
                EnemyBehindHUD.Instance.ChangeEnemiesBehindCount(-1);
                CleanBehind();
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
            else
            {

            }
        }

    }
    void CleanBehind()
    {
        if (startBehind && !finishBehind)
        {
            startBehind = false;
            finishBehind = true;
            attackBehind = false;
        }
    }
    public void OnDestroy()
    {
        if(startBehind && !finishBehind) EnemyBehindHUD.Instance.ChangeEnemiesBehindCount(-1);
        CleanBehind();
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
    void SetSymbolActive(bool active)
    {
        if (alertSymbol == null) return;

        alertSymbol.SetActive(active);
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
