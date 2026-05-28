using UnityEngine;
using System.Collections;

public class CombatState : IEnemyState
{
   readonly EnemyController _e;
    readonly EnemyStateMachine _sm;
    public EnemyStateMachine SubSM { get; private set; }
    public CombatIdleSubState IdleSubState;
    public AttackSubState AttackSubState;
    public DefendSubState DefendSubState;

    public CombatState(EnemyController e, EnemyStateMachine sm)
    {
        _e = e; _sm = sm;
        SubSM = new EnemyStateMachine();
        IdleSubState = new CombatIdleSubState(e, SubSM, this);
        AttackSubState = new AttackSubState(e, SubSM, this);
        DefendSubState = new DefendSubState(e, SubSM, this);
    }

    public void EnterState()
    {
        _e.Animator.SetBool(EnemyController.Hash_InCombat, true);
        SubSM.Initialize(IdleSubState);
    }

    public void UpdateState()
    {
        SubSM.CurrentState.UpdateState();
        RotateTowardsPlayer();        

        if (!_e.Detection.PlayerInInterestArea) _sm.ChangeState(_e.AlertState);
    }

    public void FixedUpdateState() => SubSM.CurrentState.FixedUpdateState();

    void RotateTowardsPlayer()
    {
        Vector3 dir = (_e.Detection.LastKnownPlayerPosition - _e.transform.position).normalized;
        dir.y = 0;
        if (dir.sqrMagnitude > 0.01f)
            _e.transform.rotation = Quaternion.Slerp(_e.transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 10f);
    }   

    public void ExitState() => _e.Animator.SetBool(EnemyController.Hash_InCombat, false);
}

// --- SUBESTADOS ---

public class CombatIdleSubState : IEnemyState
{
    readonly EnemyController _e;
    readonly EnemyStateMachine _subSM;
    readonly CombatState _combat;

    public CombatIdleSubState(EnemyController e, EnemyStateMachine subSM, CombatState combat)
    { _e = e; _subSM = subSM; _combat = combat; }

    public void EnterState() { }
    public void UpdateState()
    {
        if (_e.CombatGroup != null)   
        {        
            _e.Stats.NavAgent.speed = 3.5f + (Mathf.PingPong(Time.time, 1f)); 
            _e.Stats.NavAgent.SetDestination(_e.CombatGroup.GetOrbitPosition(_e, _e.Detection.LastKnownPlayerPosition));
            _e.CombatGroup.NotifyExchangeReady(_e);
        }
    }
    public void FixedUpdateState() { }
    public void ExitState() { }
    public void AssignAttack() => _subSM.ChangeState(_combat.AttackSubState);
    public void AssignDefend() => _subSM.ChangeState(_combat.DefendSubState);
}

public class AttackSubState : IEnemyState
{
    readonly EnemyController _e;
    readonly EnemyStateMachine _subSM;
    readonly CombatState _combat;
    float _timer;
    bool _hasAttacked;

    public AttackSubState(EnemyController e, EnemyStateMachine subSM, CombatState combat)
    { _e = e; _subSM = subSM; _combat = combat; }

    public void EnterState() 
    { 
        _timer = 0; 
        _hasAttacked = false; 
        _e.Stats.Specialty?.BeginAttackPhase(_e);
        
        _e.Stats.NavAgent.stoppingDistance = 0f; 
        _e.Stats.NavAgent.speed = 5.5f;
        _e.Stats.NavAgent.isStopped = false;
    }
    public void UpdateState()
    {
            _timer += Time.deltaTime;
        Vector3 playerPos = _e.Detection.LastKnownPlayerPosition;
        float dist = Vector3.Distance(_e.transform.position, playerPos);
        
        
        float targetAttackDist = 2.7f; 

       
         if (dist > targetAttackDist && !_hasAttacked)
        {            
            _e.Stats.NavAgent.SetDestination(playerPos);
            _e.Stats.NavAgent.isStopped = false;
            _e.Stats.NavAgent.acceleration = 20f; 
            _e.Stats.NavAgent.speed = 8f; 
        }
        else 
        {            
            if (!_hasAttacked)
            {            
                _e.Stats.NavAgent.velocity = Vector3.zero;
                _e.Stats.NavAgent.isStopped = true;
                
            
                Vector3 lookDir = playerPos - _e.transform.position;
                lookDir.y = 0;
                if(lookDir.sqrMagnitude > 0.01f) 
                    _e.transform.rotation = Quaternion.LookRotation(lookDir);

                
                if (_e.Stats.Specialty != null)
                {
                    _e.Stats.Specialty.TickAttack(_e, _timer);
                    _hasAttacked = true;
                }
            }
        }
        
        
        if (_timer >= 2.4f) 
        {
            _e.CombatGroup?.ReportAttackFinished(_e);
            _subSM.ChangeState(_combat.IdleSubState);
        }

    }
    public void FixedUpdateState() { }
    public void ExitState() 
    { 
        _e.Stats.NavAgent.stoppingDistance = 2.5f; 
        _e.Stats.NavAgent.isStopped = false;
    }
}

public class DefendSubState : IEnemyState
{
    readonly EnemyController _e;
    readonly EnemyStateMachine _subSM;
    readonly CombatState _combat;

    public DefendSubState(EnemyController e, EnemyStateMachine subSM, CombatState combat)
    { _e = e; _subSM = subSM; _combat = combat; }

    public void EnterState() { _e.Animator.SetBool(EnemyController.Hash_IsDefending, true); }
    public void UpdateState()
    {
        if (_e.CombatGroup != null)
        {
            _e.Stats.NavAgent.speed = 2.0f;
            
            Vector3 orbitPos = _e.CombatGroup.GetOrbitPosition(_e, _e.Detection.LastKnownPlayerPosition);
            _e.Stats.NavAgent.SetDestination(orbitPos);
        }
    }
           
    public void FixedUpdateState() { }
    public void ExitState() => _e.Animator.SetBool(EnemyController.Hash_IsDefending, false);
    public void ForceToExchange() => _subSM.ChangeState(_combat.IdleSubState);

}
