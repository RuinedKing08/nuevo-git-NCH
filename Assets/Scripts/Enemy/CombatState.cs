using UnityEngine;
using System.Collections;

public class CombatState : IEnemyState
{
    readonly EnemyController   _e;
    readonly EnemyStateMachine _sm;

    EnemyStateMachine _subSM;

    public CombatIdleSubState    IdleSubState;
    public AttackSubState        AttackSubState;
    public DefendSubState        DefendSubState;
    public ExchangeSubState      ExchangeSubState;
    public ChooseSpecialtySubState ChooseSpecialtySubState;

    public CombatState(EnemyController e, EnemyStateMachine sm)
    {
        _e  = e;
        _sm = sm;

        _subSM = new EnemyStateMachine();

        IdleSubState          = new CombatIdleSubState(e, _subSM, this);
        AttackSubState        = new AttackSubState(e, _subSM, this);
        DefendSubState        = new DefendSubState(e, _subSM, this);
        ExchangeSubState      = new ExchangeSubState(e, _subSM, this);
        ChooseSpecialtySubState = new ChooseSpecialtySubState(e, _subSM, this);
    }

    // ── IEnemyState ───────────────────────────────────────────────────────

    public void EnterState()
    {
        _e.Animator.SetBool(EnemyController.Hash_InCombat, true);

        // Si no tiene especialidad → elegirla primero (máxima prioridad)
        if (_e.Stats.Specialty == null)
            _subSM.Initialize(ChooseSpecialtySubState);
        else
            _subSM.Initialize(IdleSubState);
    }

    public void UpdateState()
    {
        _subSM.CurrentState.UpdateState();

        // Blend 2D de movimiento en combate (permite moverse mientras actúa)
        UpdateLocomotionBlend();
    }

    public void FixedUpdateState() => _subSM.CurrentState.FixedUpdateState();

    public void ExitState()
    {
        _e.Animator.SetBool(EnemyController.Hash_InCombat, false);
    }

    // ── API interna ───────────────────────────────────────────────────────

    public void OnHitInterrupt()
    {
        // Si estaba atacando, se interrumpe y vuelve a idle de combate
        if (_subSM.CurrentState == AttackSubState)
            _subSM.ChangeState(IdleSubState);
    }

    /// <summary>
    /// Actualiza los parámetros del Blend Tree 2D que permite
    /// que el enemigo se mueva suavemente mientras está en combate.
    /// El Animator tiene un BlendTree 2D (MoveX, MoveY) con:
    ///   - Idle center
    ///   - Walk forward/back/left/right
    ///   - Strafe layers para rodear al jugador
    /// </summary>
    void UpdateLocomotionBlend()
    {
        Vector3 localVelocity = _e.transform.InverseTransformDirection(_e.Stats.NavAgent.velocity);
        float   speed         = _e.Stats.NavAgent.velocity.magnitude;

        float targetX = speed > 0.1f ? localVelocity.x / _e.Stats.NavAgent.speed : 0f;
        float targetY = speed > 0.1f ? localVelocity.z / _e.Stats.NavAgent.speed : 0f;

        _e.Animator.SetFloat(EnemyController.Hash_MoveX, targetX, 0.1f, Time.deltaTime);
        _e.Animator.SetFloat(EnemyController.Hash_MoveY, targetY, 0.1f, Time.deltaTime);
        _e.Animator.SetFloat(EnemyController.Hash_Speed,  speed,  0.1f, Time.deltaTime);
    }
}

// ═══════════════════════════════════════════════════════════════════════════
//  SUB-ESTADO: ChooseSpecialty  (se ejecuta una sola vez al entrar en combate)
// ═══════════════════════════════════════════════════════════════════════════
public class ChooseSpecialtySubState : IEnemyState
{
    readonly EnemyController  _e;
    readonly EnemyStateMachine _subSM;
    readonly CombatState       _combat;

    public ChooseSpecialtySubState(EnemyController e, EnemyStateMachine subSM, CombatState combat)
    { _e = e; _subSM = subSM; _combat = combat; }

    public void EnterState()
    {
        // Determinar especialidad según lo que haya en el área de peligro
        var specialty = SpecialtyFactory.Resolve(_e);
        _e.Stats.SetSpecialty(specialty);

        // Inmediatamente pasar a idle de combate
        _subSM.ChangeState(_combat.IdleSubState);
    }

    public void UpdateState()      { }
    public void FixedUpdateState() { }
    public void ExitState()        { }
}

// ═══════════════════════════════════════════════════════════════════════════
//  SUB-ESTADO: CombatIdle  –  espera instrucciones del grupo
// ═══════════════════════════════════════════════════════════════════════════
public class CombatIdleSubState : IEnemyState
{
    readonly EnemyController   _e;
    readonly EnemyStateMachine _subSM;
    readonly CombatState       _combat;

    public CombatIdleSubState(EnemyController e, EnemyStateMachine subSM, CombatState combat)
    { _e = e; _subSM = subSM; _combat = combat; }

    public void EnterState()   { }

    public void UpdateState()
    {
        // El EnemyCombatGroup asignará el sub-estado correcto
        // a través de AssignRole()
        _e.Stats.Specialty?.UpdatePassiveBehavior(_e);   // ej: rodear, mantener distancia
    }

    public void FixedUpdateState() { }
    public void ExitState()        { }

    // Llamado por EnemyCombatGroup
    public void AssignAttack()  => _subSM.ChangeState(_combat.AttackSubState);
    public void AssignDefend()  => _subSM.ChangeState(_combat.DefendSubState);
}

// ═══════════════════════════════════════════════════════════════════════════
//  SUB-ESTADO: Attack  (Prioridad 7)
// ═══════════════════════════════════════════════════════════════════════════
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
        _attackWindowTimer = 0f;
        // La especialidad decide cuándo y qué ataque ejecutar
        _e.Stats.Specialty?.BeginAttackPhase(_e);
    }

    public void UpdateState()
    {
        _attackWindowTimer += Time.deltaTime;

        // Ejecutar ataque en el momento que la especialidad decida (dentro de la ventana)
        _e.Stats.Specialty?.TickAttack(_e, _attackWindowTimer);

        if (_attackWindowTimer >= AttackWindowDuration)
            _subSM.ChangeState(_combat.ExchangeSubState);
    }

    public void FixedUpdateState() { }

    public void ExitState()
    {
        _e.Animator.ResetTrigger(EnemyController.Hash_AttackIndex);
        _e.Stats.Specialty?.EndAttackPhase(_e);
    }
}

// ═══════════════════════════════════════════════════════════════════════════
//  SUB-ESTADO: Defend  (Prioridad 6)
// ═══════════════════════════════════════════════════════════════════════════
public class DefendSubState : IEnemyState
{
    readonly EnemyController   _e;
    readonly EnemyStateMachine _subSM;
    readonly CombatState       _combat;

    // Decisión aleatoria: bloquear o retroceder
    bool _willBlock;

    public DefendSubState(EnemyController e, EnemyStateMachine subSM, CombatState combat)
    { _e = e; _subSM = subSM; _combat = combat; }

    public void EnterState()
    {
        _willBlock = _e.Stats.Specialty?.PrefersBlock(_e) ?? (Random.value > 0.5f);
        _e.Animator.SetBool(EnemyController.Hash_IsDefending, true);
        _e.Stats.IsBlocking = _willBlock;
    }

    public void UpdateState()
    {
        if (!_willBlock)
        {
            // Alejarse del jugador
            Vector3 awayDir = (_e.transform.position - _e.Detection.LastKnownPlayerPosition).normalized;
            _e.Stats.NavAgent.SetDestination(_e.transform.position + awayDir * 2f);
        }
        // Si está bloqueando → el sistema de daño en EnemyController maneja la reducción
    }

    public void FixedUpdateState() { }

    public void ExitState()
    {
        _e.Animator.SetBool(EnemyController.Hash_IsDefending, false);
        _e.Stats.IsBlocking = false;
    }

    // Llamado por EnemyCombatGroup cuando el turno de ataque termina
    public void ForceToExchange() => _subSM.ChangeState(_combat.ExchangeSubState);
}

// ═══════════════════════════════════════════════════════════════════════════
//  SUB-ESTADO: Exchange  –  transición entre atacar/defender (1-2 seg)
// ═══════════════════════════════════════════════════════════════════════════
public class ExchangeSubState : IEnemyState
{
    readonly EnemyController   _e;
    readonly EnemyStateMachine _subSM;
    readonly CombatState       _combat;

    float _timer;
    float _duration;

    public ExchangeSubState(EnemyController e, EnemyStateMachine subSM, CombatState combat)
    { _e = e; _subSM = subSM; _combat = combat; }

    public void EnterState()
    {
        _timer    = 0f;
        _duration = Random.Range(1f, 2f);
        // Notificar al grupo que este enemigo está disponible
        _e.CombatGroup?.NotifyExchangeReady(_e);
    }

    public void UpdateState()
    {
        _timer += Time.deltaTime;
        if (_timer >= _duration)
            _subSM.ChangeState(_combat.IdleSubState);  // el grupo reasignará el rol
    }

    public void FixedUpdateState() { }
    public void ExitState()        { }
}
