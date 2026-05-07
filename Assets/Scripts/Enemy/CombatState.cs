using UnityEngine;
using System.Collections;

public class CombatState : IEnemyState
{
    readonly EnemyController   _e;
    readonly EnemyStateMachine _sm;
    float _exitCombatTimer = 0f;
    const float TimeToExitCombat = 3f;
    private float _orbitAngle;
    private float _currentOrbitAngle;

    EnemyStateMachine _subSM;

    public CombatIdleSubState    IdleSubState;
    public AttackSubState        AttackSubState;
    public DefendSubState        DefendSubState;
    public ChooseSpecialtySubState ChooseSpecialtySubState;

    public CombatState(EnemyController e, EnemyStateMachine sm)
    {
        _e  = e;
        _sm = sm;

        _subSM = new EnemyStateMachine();

        IdleSubState          = new CombatIdleSubState(e, _subSM, this);
        AttackSubState        = new AttackSubState(e, _subSM, this);
        DefendSubState        = new DefendSubState(e, _subSM, this);
        ChooseSpecialtySubState = new ChooseSpecialtySubState(e, _subSM, this);
    }

    // ── IEnemyState ───────────────────────────────────────────────────────

    public void EnterState()
    {
        _e.Animator.SetBool(EnemyController.Hash_InCombat, true);
        
        if (_e.CombatGroup != null)   
        {
            _e.CombatGroup.AddEnemy(_e);
        }
        
        if (_e.Stats.Specialty == null)
            _subSM.Initialize(ChooseSpecialtySubState);
        else
            _subSM.Initialize(IdleSubState);
    }
    public string GetSubStateName()
    {
        if (_subSM != null && _subSM.CurrentState != null)
        {
            return _subSM.CurrentState.GetType().Name;
        }
        return "None";
    }
    public void DoOrbit(float radius, float speedMultiplier = 1f)
    {
       
        if (_e.CombatGroup == null) return;

        
        float baseAngle = _e.CombatGroup.GetTargetAngleForMember(_e);
        
        
        float globalOffset = Time.time * 20f * speedMultiplier; 
        float targetAngle = (baseAngle + globalOffset) % 360f;

        
        _currentOrbitAngle = Mathf.LerpAngle(_currentOrbitAngle, targetAngle, Time.deltaTime * 2f);

        
        float radians = _currentOrbitAngle * Mathf.Deg2Rad;
        Vector3 offset = new Vector3(Mathf.Cos(radians), 0, Mathf.Sin(radians)) * radius;
        Vector3 targetPos = _e.Detection.LastKnownPlayerPosition + offset;

        
        if (_e.Stats.NavAgent.isOnNavMesh)
        {
            _e.Stats.NavAgent.SetDestination(targetPos);
        }
        
        Vector3 dirToPlayer = (_e.Detection.LastKnownPlayerPosition - _e.transform.position).normalized;
        dirToPlayer.y = 0;
        if (dirToPlayer != Vector3.zero)
        {
            _e.transform.rotation = Quaternion.Slerp(_e.transform.rotation, Quaternion.LookRotation(dirToPlayer), Time.deltaTime * 8f);
        }
    }
        

    public void UpdateState()
    {
        _subSM.CurrentState.UpdateState();

        
        UpdateLocomotionBlend();

        if (!_e.Detection.PlayerInInterestArea)
        {
            _exitCombatTimer += Time.deltaTime;
            if (_exitCombatTimer >= TimeToExitCombat)
            {
                _sm.ChangeState(_e.AlertState); 
            }
        }
        else
        {
            _exitCombatTimer = 0f; 
        }
    }

    public void FixedUpdateState() => _subSM.CurrentState.FixedUpdateState();

    public void ExitState()
    {
        _e.Animator.SetBool(EnemyController.Hash_InCombat, false);
        
        if (_e.CombatGroup != null)
        {
            _e.CombatGroup.RemoveEnemy(_e);
        }
    }

    // ── API interna ───────────────────────────────────────────────────────

    public void OnHitInterrupt()
    {
        
        if (_subSM.CurrentState == AttackSubState)
            _subSM.ChangeState(IdleSubState);
    }

    
    void UpdateLocomotionBlend()
    {
        
        Vector3 localVelocity = _e.transform.InverseTransformDirection(_e.Stats.NavAgent.velocity);
        float maxSpeed = _e.Stats.NavAgent.speed;

        
        float targetX = localVelocity.x / maxSpeed;
        float targetY = localVelocity.z / maxSpeed;

        
        _e.Animator.SetFloat(EnemyController.Hash_MoveX, targetX, 0.1f, Time.deltaTime);
        _e.Animator.SetFloat(EnemyController.Hash_MoveY, targetY, 0.1f, Time.deltaTime);
        
        _e.Animator.SetFloat(EnemyController.Hash_Speed, _e.Stats.NavAgent.velocity.magnitude, 0.1f, Time.deltaTime);
    }
}


public class ChooseSpecialtySubState : IEnemyState
{
    readonly EnemyController  _e;
    readonly EnemyStateMachine _subSM;
    readonly CombatState       _combat;

    public ChooseSpecialtySubState(EnemyController e, EnemyStateMachine subSM, CombatState combat)
    { _e = e; _subSM = subSM; _combat = combat; }

    public void EnterState()
    {

        var specialty = SpecialtyFactory.Resolve(_e);
        if (specialty != null)
        {
            _e.Stats.SetSpecialty(specialty);
        }
        else
        {
            Debug.LogWarning($"{_e.name}: SpecialtyF actory devolvió null, usando comportamiento por defecto.");
        }

        _subSM.ChangeState(_combat.IdleSubState);
    }

    public void UpdateState()      { }
    public void FixedUpdateState() { }
    public void ExitState()        { }
}


public class CombatIdleSubState : IEnemyState
{
    
    readonly EnemyController   _e;
    readonly EnemyStateMachine _subSM;
    readonly CombatState       _combat;
    
    private float _postAttackSpeedMultiplier = 1f;
    private float _postAttackTimer = 0f;
    private const float PostAttackSpeedDuration = 2f;
    private const float PostAttackSpeedBoost = 1.5f;

    public CombatIdleSubState(EnemyController e, EnemyStateMachine subSM, CombatState combat)
    { _e = e; _subSM = subSM; _combat = combat; }

    public void EnterState()   
    { 
        Debug.Log("IdleSubstate"); 
        _e.CombatGroup?.NotifyExchangeReady(_e);
        
        
        _postAttackSpeedMultiplier = PostAttackSpeedBoost;
        _postAttackTimer = PostAttackSpeedDuration;
    }

    public void UpdateState()
    {
        
        if (_postAttackTimer > 0f)
        {
            _postAttackTimer -= Time.deltaTime;
            _postAttackSpeedMultiplier = Mathf.Lerp(PostAttackSpeedBoost, 1f, 1f - (_postAttackTimer / PostAttackSpeedDuration));
        }
        else
        {
            _postAttackSpeedMultiplier = 1f;
        }

        if (_e.Stats.Specialty != null)
        {
            _e.Stats.Specialty.UpdatePassiveBehavior(_e);
        }
        else
        {
            _combat.DoOrbit(radius: 5.5f, speedMultiplier: 1f * _postAttackSpeedMultiplier);
        }
    }

    public void FixedUpdateState() { }
    public void ExitState()        { }

   
    public void AssignAttack()  => _subSM.ChangeState(_combat.AttackSubState);
    public void AssignDefend()  => _subSM.ChangeState(_combat.DefendSubState);
}


public class AttackSubState : IEnemyState
{
    readonly EnemyController   _e;
    readonly EnemyStateMachine _subSM;
    readonly CombatState       _combat;

    float _attackWindowTimer;
    const float AttackWindowDuration = 5f;

    public AttackSubState(EnemyController e, EnemyStateMachine subSM, CombatState combat)
    { _e = e; _subSM = subSM; _combat = combat; }

    public void EnterState()
    {
        Debug.Log("attackSubstate");
        _attackWindowTimer = 0f;
        


        _e.Stats.Specialty?.BeginAttackPhase(_e);
    }

    public void UpdateState()
    {
        _attackWindowTimer += Time.deltaTime;
        
        float distanceToPlayer = Vector3.Distance(_e.transform.position, _e.Detection.LastKnownPlayerPosition);
        float attackRange = 1.8f; 

        
        if (_attackWindowTimer < 1.5f) 
        {            
            _combat.DoOrbit(radius: 5.5f, speedMultiplier: 1.2f);
        }
        
        else 
        {
            if (distanceToPlayer > attackRange)
            {                
                _e.Stats.NavAgent.isStopped = false;
                _e.Stats.NavAgent.SetDestination(_e.Detection.LastKnownPlayerPosition);
                
                
                _e.Stats.NavAgent.speed = 5.5f; 

                
                Vector3 dir = (_e.Detection.LastKnownPlayerPosition - _e.transform.position).normalized;
                dir.y = 0;
                if (dir != Vector3.zero)
                {
                    _e.transform.rotation = Quaternion.Slerp(_e.transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 15f);
                }
            }
            else
            {         
                _e.Stats.NavAgent.isStopped = true;            
                
                
                _e.Stats.Specialty?.TickAttack(_e, _attackWindowTimer);
                
                
                _e.transform.LookAt(new Vector3(_e.Detection.LastKnownPlayerPosition.x, _e.transform.position.y, _e.Detection.LastKnownPlayerPosition.z));
            }
        }

       
        if (_attackWindowTimer >= 5.0f) 
        {
            _e.CombatGroup?.ReportAttackFinished(_e);
            _subSM.ChangeState(_combat.IdleSubState); // Ir directamente a idle con movimiento rápido
        }
    }

    public void FixedUpdateState() { }

    public void ExitState()
    {
        _e.Stats.NavAgent.speed = 3.5f;
        _e.Animator.ResetTrigger(EnemyController.Hash_AttackIndex);
        _e.Stats.Specialty?.EndAttackPhase(_e);
    }
}


public class DefendSubState : IEnemyState
{
    readonly EnemyController   _e;
    readonly EnemyStateMachine _subSM;
    readonly CombatState       _combat;

    
    bool _willBlock;

    public DefendSubState(EnemyController e, EnemyStateMachine subSM, CombatState combat)
    { _e = e; _subSM = subSM; _combat = combat; }

    public void EnterState()
    {
        Debug.Log("defenceSubstate");
        _willBlock = _e.Stats.Specialty?.PrefersBlock(_e) ?? (Random.value > 0.5f);
        _e.Animator.SetBool(EnemyController.Hash_IsDefending, true);
        _e.Stats.IsBlocking = _willBlock;
    }

    public void UpdateState()
    {
        float distanceToPlayer = Vector3.Distance(_e.transform.position, _e.Detection.LastKnownPlayerPosition);
        float currentOrbitSpeed = _willBlock ? 0.4f : 0.8f;
        float targetRadius = 7.0f;

        if (distanceToPlayer < 3.5f)
        {    
            Vector3 awayDir = (_e.transform.position - _e.Detection.LastKnownPlayerPosition).normalized;
            _e.Stats.NavAgent.isStopped = false;
            _e.Stats.NavAgent.SetDestination(_e.transform.position + awayDir * 3f);
        }
        else if (distanceToPlayer > 8f)
        {        
            _e.Stats.NavAgent.isStopped = false;
            _e.Stats.NavAgent.SetDestination(_e.Detection.LastKnownPlayerPosition);
        }
        else
        {           
            _combat.DoOrbit(radius: targetRadius, speedMultiplier: currentOrbitSpeed);
        }
        
    }

    public void FixedUpdateState() { }

    public void ExitState()
    {
        _e.Animator.SetBool(EnemyController.Hash_IsDefending, false);
        _e.Stats.IsBlocking = false;
    }
    
    public void ForceToExchange() => _subSM.ChangeState(_combat.IdleSubState);
}
