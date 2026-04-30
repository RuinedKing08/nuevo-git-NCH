using UnityEngine;
using System.Collections.Generic;

// ═══════════════════════════════════════════════════════════════════════════
//  ENEMY DETECTION AREA
// ═══════════════════════════════════════════════════════════════════════════
public class EnemyDetectionArea : MonoBehaviour
{
    [Header("Radios")]
    public float InterestAreaRadius ;
    public float DangerAreaRadius   ;

    // ── Estado detectado ──────────────────────────────────────────────────
    public bool    PlayerInInterestArea { get; private set; }
    public bool    PlayerInDangerArea   { get; private set; }
    public Vector3 LastKnownPlayerPosition { get; private set; }
    private SphereCollider _interestTrigger;
    private SphereCollider _dangerTrigger;
    private float _interestRadiusSqr;
    private float _dangerRadiusSqr;
    Transform _player;

    void Awake()
    {        
        var p = GameObject.FindGameObjectWithTag("Player");
        if (p) _player = p.transform;
      
        _interestRadiusSqr = InterestAreaRadius * InterestAreaRadius;
        _dangerRadiusSqr   = DangerAreaRadius * DangerAreaRadius;

        SetupTriggers();
    }

    void SetupTriggers()
    {       
        _interestTrigger = GetComponent<SphereCollider>();
        if (_interestTrigger == null)
        {
            _interestTrigger = gameObject.AddComponent<SphereCollider>();
            _interestTrigger.isTrigger = true;
        }
        else
        {
            _interestTrigger.isTrigger = true;
        }
        _interestTrigger.radius = InterestAreaRadius;
        
        Transform dangerTransform = transform.Find("DangerTrigger");
        GameObject dangerObj;
        if (dangerTransform == null)
        {
            dangerObj = new GameObject("DangerTrigger");
            dangerObj.transform.SetParent(transform);
            dangerObj.transform.localPosition = Vector3.zero;
        }
        else
        {
            dangerObj = dangerTransform.gameObject;
        }
        _dangerTrigger = dangerObj.GetComponent<SphereCollider>();
        if (_dangerTrigger == null)
        {
            _dangerTrigger = dangerObj.AddComponent<SphereCollider>();
            _dangerTrigger.isTrigger = true;
        }
        else
        {
            _dangerTrigger.isTrigger = true;
        }
        _dangerTrigger.radius = DangerAreaRadius;
    }

    void Update()
    {
        if (_player == null) return;

        
        if (PlayerInInterestArea)
            LastKnownPlayerPosition = _player.position;
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.gameObject == _player?.gameObject)
                PlayerInDangerArea = true;
            
            float sqrDist = (transform.position - other.transform.position).sqrMagnitude;
            if (sqrDist <= _interestRadiusSqr)
                PlayerInInterestArea = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.gameObject == _player?.gameObject)
                PlayerInDangerArea = false;

            
            float sqrDist = (transform.position - other.transform.position).sqrMagnitude;
            if (sqrDist > _interestRadiusSqr)
                PlayerInInterestArea = false;
        }
    }

    

    public bool HasMeleeWeaponInDangerArea(out EnemyMeleeWeapon weapon)
    {
        weapon = null;
        var cols = Physics.OverlapSphere(transform.position, DangerAreaRadius);
        foreach (var c in cols)
        {
            var w = c.GetComponent<EnemyMeleeWeapon>();
            if (w != null) { weapon = w; return true; }
        }
        return false;
    }

    public bool HasThrowableObjectInDangerArea(out EnemyThrowableObject throwable)
    {
        throwable = null;
        var cols = Physics.OverlapSphere(transform.position, DangerAreaRadius);
        foreach (var c in cols)
        {
            var t = c.GetComponent<EnemyThrowableObject>();
            if (t != null) { throwable = t; return true; }
        }
        return false;
    }
    
    public bool IsCloserThanPlayer(Vector3 objectPos)
    {
        
        float sqrDistToObj    = (transform.position - objectPos).sqrMagnitude;
        float sqrDistToPlayer = (transform.position - LastKnownPlayerPosition).sqrMagnitude;
        return sqrDistToObj < sqrDistToPlayer;
    }

    public EnemyController GetNearbyRestingEnemy()
    {
        var cols = Physics.OverlapSphere(transform.position, InterestAreaRadius);
        foreach (var c in cols)
        {
            var e = c.GetComponent<EnemyController>();
            if (e != null && e != GetComponent<EnemyController>() &&
                e.StateMachine.CurrentState == e.RestState)
                return e;
        }
        return null;
    }

    public void AlertNearbyEnemies()
    {
        var cols = Physics.OverlapSphere(transform.position, InterestAreaRadius);

        foreach (var c in cols)
        {
            var e = c.GetComponent<EnemyController>();

            if (e == null) continue;
            if (e == GetComponent<EnemyController>()) continue;
            
            if (e.StateMachine.CurrentState == e.AlertState) continue;
            if (e.StateMachine.CurrentState == e.CombatState) continue;

            e.StateMachine.ChangeState(e.AlertState);
        }
    }

    public void PropagateEnterCombat()
    {
        var cols = Physics.OverlapSphere(transform.position, InterestAreaRadius);
        foreach (var c in cols)
        {
            var e = c.GetComponent<EnemyController>();
            if (e != null && e.StateMachine.CurrentState != e.CombatState)
                e.StateMachine.ChangeState(e.CombatState);
        }
    }

    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, InterestAreaRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, DangerAreaRadius);
    }
}
