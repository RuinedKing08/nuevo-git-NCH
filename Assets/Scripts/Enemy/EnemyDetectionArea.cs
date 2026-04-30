using UnityEngine;
using System.Collections.Generic;

// ═══════════════════════════════════════════════════════════════════════════
//  ENEMY DETECTION AREA
// ═══════════════════════════════════════════════════════════════════════════
public class EnemyDetectionArea : MonoBehaviour
{
    [Header("Radios")]
    public float InterestAreaRadius = 10f;
    public float DangerAreaRadius = 4f;

    [Header("Estado detectado")]
    [SerializeField] private bool _playerInInterestArea;
    [SerializeField] private bool _playerInDangerArea;
    
    public bool PlayerInInterestArea => _playerInInterestArea;
    public bool PlayerInDangerArea => _playerInDangerArea;
    public Vector3 LastKnownPlayerPosition { get; private set; }

    private float _interestRadiusSqr;
    private float _dangerRadiusSqr;
    private Transform _player;

    void Awake()
    {        
        var p = GameObject.FindGameObjectWithTag("Player");
        if (p) _player = p.transform;      
        
        _interestRadiusSqr = InterestAreaRadius * InterestAreaRadius;
        _dangerRadiusSqr   = DangerAreaRadius * DangerAreaRadius;
    }

    void Update()
    {
        if (_player == null) return;

        float sqrDistance = (transform.position - _player.position).sqrMagnitude;


        _playerInInterestArea = sqrDistance <= _interestRadiusSqr;
        _playerInDangerArea   = sqrDistance <= _dangerRadiusSqr;

        if (_playerInInterestArea)
        {
            LastKnownPlayerPosition = _player.position;
        }
    }

   

    public bool HasMeleeWeaponInDangerArea(out EnemyMeleeWeapon weapon)
    {
        weapon = null;
        var cols = Physics.OverlapSphere(transform.position, DangerAreaRadius);
        foreach (var c in cols)
        {
            if (c.TryGetComponent(out EnemyMeleeWeapon w)) { weapon = w; return true; }
        }
        return false;
    }

    public bool HasThrowableObjectInDangerArea(out EnemyThrowableObject throwable)
    {
        throwable = null;
        var cols = Physics.OverlapSphere(transform.position, DangerAreaRadius);
        foreach (var c in cols)
        {
            if (c.TryGetComponent(out EnemyThrowableObject t)) { throwable = t; return true; }
        }
        return false;
    }
    
    public bool IsCloserThanPlayer(Vector3 objectPos)
    {
        float sqrDistToObj = (transform.position - objectPos).sqrMagnitude;
        float sqrDistToPlayer = (transform.position - _player.position).sqrMagnitude;
        return sqrDistToObj < sqrDistToPlayer;
    }

    public void AlertNearbyEnemies()
    {
        var cols = Physics.OverlapSphere(transform.position, InterestAreaRadius);
        foreach (var c in cols)
        {
            if (c.TryGetComponent(out EnemyController e))
            {
                if (e.gameObject == this.gameObject) continue;
                if (e.StateMachine.CurrentState == e.AlertState || e.StateMachine.CurrentState == e.CombatState) continue;
                e.StateMachine.ChangeState(e.AlertState);
            }
        }
    }

    public void PropagateEnterCombat()
    {
        var cols = Physics.OverlapSphere(transform.position, InterestAreaRadius);
        foreach (var c in cols)
        {
            if (c.TryGetComponent(out EnemyController e))
            {
                if (e.StateMachine.CurrentState != e.CombatState)
                    e.StateMachine.ChangeState(e.CombatState);
            }
        }
    }

    public EnemyController GetNearbyRestingEnemy()
    {
        var cols = Physics.OverlapSphere(transform.position, InterestAreaRadius);
        foreach (var c in cols)
        {
            if (c.TryGetComponent(out EnemyController e))
            {
                if (e != GetComponent<EnemyController>() && e.StateMachine.CurrentState == e.RestState)
                    return e;
            }
        }
        return null;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, InterestAreaRadius);
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, DangerAreaRadius);
        
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, LastKnownPlayerPosition);
        
        Gizmos.DrawSphere(LastKnownPlayerPosition, 0.3f); 
    }
}
