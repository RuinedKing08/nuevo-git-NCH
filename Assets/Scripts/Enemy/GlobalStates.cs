using UnityEngine;
using System.Collections;

// ═══════════════════════════════════════════════════════════════════════════
//  PATROL STATE  (Prioridad 1)
// ═══════════════════════════════════════════════════════════════════════════
public class PatrolState : IEnemyState
{
    readonly EnemyController _e;
    readonly EnemyStateMachine _sm;

    int  _loopsNearRestingEnemy;
    bool _switchedWithResting;

    public PatrolState(EnemyController e, EnemyStateMachine sm) { _e = e; _sm = sm; }

    public void EnterState()
    {
        _loopsNearRestingEnemy = 0;
        _e.Animator.SetBool(EnemyController.Hash_InCombat, false);
        _e.Animator.SetBool(EnemyController.Hash_IsAlert,  false);
        // Animator: blend tree de caminar activo
    }

    public void UpdateState()
    {
        
        if (_e.Detection.PlayerInInterestArea)
        {
            _sm.ChangeState(_e.AlertState);
            Debug.Log("Creo que vi algo");
            return;
        }

        
        EnemyController nearbyResting = _e.Detection.GetNearbyRestingEnemy();
        if (nearbyResting != null)
        {
            _loopsNearRestingEnemy++;
            if (_loopsNearRestingEnemy >= 3)
            {
                SwapWithRestingEnemy(nearbyResting);
                return;
            }
        }
        else
        {
            _loopsNearRestingEnemy = 0;
        }

        
        _e.Stats.PatrolPath.Tick(_e);
        Debug.Log("Patrullando");

        float speed = _e.Stats.PatrolPath.CurrentSpeed;
        _e.Animator.SetFloat(EnemyController.Hash_Speed, speed, 0.1f, Time.deltaTime);
    }

    public void FixedUpdateState() { }

    public void ExitState() { }

    void SwapWithRestingEnemy(EnemyController restingEnemy)
    {
        restingEnemy.StateMachine.ChangeState(restingEnemy.PatrolState);
        _sm.ChangeState(_e.RestState);
    }
}

// ═══════════════════════════════════════════════════════════════════════════
//  REST STATE  (Prioridad 0)
// ═══════════════════════════════════════════════════════════════════════════
public class RestState : IEnemyState
{
    readonly EnemyController   _e;
    readonly EnemyStateMachine _sm;

    public RestState(EnemyController e, EnemyStateMachine sm) { _e = e; _sm = sm; }

    public void EnterState()
    {
        _e.Animator.SetFloat(EnemyController.Hash_Speed, 0f);
        _e.Animator.SetBool(EnemyController.Hash_IsAlert, false);
    }

    public void UpdateState()
    {
        if (_e.Detection.PlayerInInterestArea)
            _sm.ChangeState(_e.AlertState);
    }

    public void FixedUpdateState() { }
    public void ExitState() { }
}

// ═══════════════════════════════════════════════════════════════════════════
//  ALERT STATE  (Prioridad 5)
// ═══════════════════════════════════════════════════════════════════════════
public class AlertState : IEnemyState
{
    readonly EnemyController   _e;
    readonly EnemyStateMachine _sm;

    Vector3 _lastKnownPlayerPos;
    float   _timePlayerVisible;
    bool    _returningToOrigin;
    Vector3 _originPosition;

    const float TimeToEnterCombat = 5f;

    public AlertState(EnemyController e, EnemyStateMachine sm) { _e = e; _sm = sm; }

    public void EnterState()
    {
        _returningToOrigin  = false;
        _timePlayerVisible  = 0f;
        _originPosition     = _e.transform.position;
        _lastKnownPlayerPos = _e.Detection.LastKnownPlayerPosition;

        _e.Animator.SetBool(EnemyController.Hash_IsAlert, true);

        // Avisar a enemigos cercanos
        _e.Detection.AlertNearbyEnemies();
    }

    public void UpdateState()
    {
        // ── Si el jugador entra en área de PELIGRO → combate inmediato ───
        if (_e.Detection.PlayerInDangerArea)
        {
            EnterCombat();
            return;
        }

        if (!_returningToOrigin)
        {
            // ── El jugador sigue visible en área de interés ──────────────
            if (_e.Detection.PlayerInInterestArea)
            {
                _timePlayerVisible += Time.deltaTime;
                _lastKnownPlayerPos = _e.Detection.LastKnownPlayerPosition;

                if (_timePlayerVisible >= TimeToEnterCombat)
                {
                    EnterCombat();
                    return;
                }
            }
            else
            {
                // ── Ir a la última posición conocida ─────────────────────
                MoveTowards(_lastKnownPlayerPos);

                if (ReachedDestination(_lastKnownPlayerPos))
                    StartReturnToOrigin();
            }
        }
        else
        {
            // ── Volviendo al origen ──────────────────────────────────────
            if (_e.Detection.PlayerInInterestArea)
            {
                _returningToOrigin = false;
                _timePlayerVisible = 0f;
                return;
            }

            MoveTowards(_originPosition);

            if (ReachedDestination(_originPosition))
                _sm.ChangeState(_e.PatrolState);   // o RestState según la prioridad original
        }

        float speed = (_returningToOrigin || !_e.Detection.PlayerInInterestArea) ? 1f : 1.5f;
        _e.Animator.SetFloat(EnemyController.Hash_Speed, speed, 0.1f, Time.deltaTime);
    }

    public void FixedUpdateState() { }

    public void ExitState()
    {
        _e.Animator.SetBool(EnemyController.Hash_IsAlert, false);
    }

    void EnterCombat()
    {
        // Propagar cambio de prioridad a todos los enemigos en el área de detección
        _e.Detection.PropagateEnterCombat();
        _sm.ChangeState(_e.CombatState);
        Debug.Log("Esta ahi");
    }

    void MoveTowards(Vector3 target)
    {
        // Conectar aquí con NavMeshAgent o CharacterController
        _e.Stats.NavAgent.SetDestination(target);
    }

    bool ReachedDestination(Vector3 target) =>
        Vector3.Distance(_e.transform.position, target) < 0.5f;

    void StartReturnToOrigin() => _returningToOrigin = true;
}
