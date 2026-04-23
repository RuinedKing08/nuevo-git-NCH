using UnityEngine;
using System.Collections.Generic;

// ═══════════════════════════════════════════════════════════════════════════
//  ENEMY DETECTION AREA
//  Gestiona las dos zonas: "Área de Interés" y "Área de Peligro"
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

    Transform _player;

    void Awake()
    {
        // Busca al jugador por tag; ajusta según tu proyecto
        var p = GameObject.FindGameObjectWithTag("Player");
        if (p) _player = p.transform;
    }

    void Update()
    {
        //Debug.Log("Player encontrado: " + (_player != null));
        if (_player == null) return;

        float dist = Vector3.Distance(transform.position, _player.position);

        PlayerInInterestArea = dist <= InterestAreaRadius;
        PlayerInDangerArea   = dist <= DangerAreaRadius;

        if (PlayerInInterestArea)
            LastKnownPlayerPosition = _player.position;
    }

    // ── Helpers de detección de objetos ──────────────────────────────────

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
        float distToObj    = Vector3.Distance(transform.position, objectPos);
        float distToPlayer = Vector3.Distance(transform.position, LastKnownPlayerPosition);
        return distToObj < distToPlayer;
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
