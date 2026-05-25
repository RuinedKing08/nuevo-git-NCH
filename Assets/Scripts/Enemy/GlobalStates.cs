using UnityEngine;
using System.Collections;


public class AlertState : IEnemyState
{
   readonly EnemyController _e;
    readonly EnemyStateMachine _sm;

    public AlertState(EnemyController e, EnemyStateMachine sm) { _e = e; _sm = sm; }

    public void EnterState() => _e.Animator.SetBool(EnemyController.Hash_IsAlert, true);

    public void UpdateState()
    {
        if (_e.Detection.PlayerInDangerArea) { _sm.ChangeState(_e.CombatState); return; }

        _e.Stats.NavAgent.SetDestination(_e.Detection.LastKnownPlayerPosition);

        if (_e.Stats.NavAgent.velocity.sqrMagnitude > 0.1f)
        {
            Quaternion targetRot = Quaternion.LookRotation(_e.Stats.NavAgent.velocity.normalized);
            _e.transform.rotation = Quaternion.Slerp(_e.transform.rotation, targetRot, Time.deltaTime * 5f);
        }
    }
        

    public void FixedUpdateState() { }
    public void ExitState() { }
}

public class PatrolState : IEnemyState
{
    readonly EnemyController _e;
    readonly EnemyStateMachine _sm;

    public PatrolState(EnemyController e, EnemyStateMachine sm) { _e = e; _sm = sm; }

    public void EnterState() { }

    public void UpdateState()
    {
        if (_e.Detection.PlayerInInterestArea) { _sm.ChangeState(_e.AlertState); return; }

        _e.Stats.PatrolPath.Tick(_e);

        if (_e.Stats.NavAgent.velocity.sqrMagnitude > 0.1f)
        {
            Quaternion targetRot = Quaternion.LookRotation(_e.Stats.NavAgent.velocity.normalized);
            _e.transform.rotation = Quaternion.Slerp(_e.transform.rotation, targetRot, Time.deltaTime * 5f);
        }
    }

    public void FixedUpdateState() { }
    public void ExitState() { }
}

public class RestState : IEnemyState
{
    readonly EnemyController _e;
    readonly EnemyStateMachine _sm;

    public RestState(EnemyController e, EnemyStateMachine sm) { _e = e; _sm = sm; }

    public void EnterState() 
    {
        
    }

    public void UpdateState()
    {
        if (_e.Detection.PlayerInInterestArea) _sm.ChangeState(_e.AlertState);
    }

    public void FixedUpdateState() { }
    public void ExitState() { }
}