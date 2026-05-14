using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerCombatSystem : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Animator _animator;
    [SerializeField] private PlayerHitCollider _hitCollider;
    [SerializeField] private PlayerMove _playerMove;

    [Header("Parámetros de Daño")]
    [SerializeField] private int _lightAttackDamage = 10;
    [SerializeField] private int _heavyAttackDamage = 20;
    [SerializeField] private int _grabDamage = 15;

    [Header("Tiempos de Combo")]
    [SerializeField] private float _comboResetTime = 1.0f;

    //temporal — sustituir por Animation Events
    [Header("Temporal — hit sin Animation Events")]
    [SerializeField] private bool _temporalManualHitWindow = true;
    [SerializeField] private float _temporalHitDelay = 0.12f;
    [SerializeField] private float _temporalHitActiveDuration = 0.25f;

    private int _currentComboIndex = 0;
    private float _lastAttackTime = 0f;
    private bool _isAttacking = false;
    private bool _canCombo = true;
    private Coroutine _temporalAttackHitRoutine;

    private static readonly int Hash_AttackType = Animator.StringToHash("AttackType");
    private static readonly int Hash_ComboIndex = Animator.StringToHash("ComboIndex");
    private static readonly int Hash_Attack = Animator.StringToHash("Attack");
    private static readonly int Hash_GrabType = Animator.StringToHash("GrabType");
    private static readonly int Hash_Grab = Animator.StringToHash("Grab");
    private static readonly int Hash_Block = Animator.StringToHash("Block");
    private static readonly int Hash_BlockEnd = Animator.StringToHash("BlockEnd");
    private static readonly int Hash_Hit = Animator.StringToHash("Hit");

    private void Awake()
    {
        if (_animator == null) _animator = GetComponent<Animator>();
        if (_hitCollider == null) _hitCollider = GetComponentInChildren<PlayerHitCollider>();
        if (_playerMove == null) _playerMove = GetComponent<PlayerMove>();
    }

    private void Update()
    {
        HandleComboReset();
    }
    
    public void PerformLightAttack()
    {
        if (_isAttacking || !_canCombo) return;

        
        if (_currentComboIndex == 0 || Time.time - _lastAttackTime > _comboResetTime)
        {
            
            _currentComboIndex = 0;
            ExecuteAttack(0); 
        }
        else if (_currentComboIndex == 1)
        {
            
            _currentComboIndex = 1;
            ExecuteAttack(0);
        }
        else if (_currentComboIndex == 2)
        {
            
            _currentComboIndex = 2;
            ExecuteAttack(0);
        }
        else if (_currentComboIndex > 2)
        {
            
            _currentComboIndex = 0;
            ExecuteAttack(0);
        }
    }

    
    public void PerformHeavyAttack()
    {
        if (_isAttacking || !_canCombo) return;

        
        if (_currentComboIndex == 0 || Time.time - _lastAttackTime > _comboResetTime)
        {            
            _currentComboIndex = 3; 
            ExecuteAttack(1); 
        }
        else if (_currentComboIndex == 3)
        {            
            _currentComboIndex = 4;
            ExecuteAttack(1);
        }
        else if (_currentComboIndex == 4)
        {            
            _currentComboIndex = 5;
            ExecuteAttack(1);
        }
        else if (_currentComboIndex > 5)
        {           
            _currentComboIndex = 3;
            ExecuteAttack(1);
        }
    }

    private void ExecuteAttack(int attackType)
    {
        _isAttacking = true;
        _lastAttackTime = Time.time;
        _canCombo = false;

        if (_animator != null)
        {
            _animator.SetInteger(Hash_AttackType, attackType);
            _animator.SetInteger(Hash_ComboIndex, _currentComboIndex);
            _animator.SetTrigger(Hash_Attack);
        }

        if (attackType == 0)
        {
            _hitCollider.SetDamage(_lightAttackDamage);
        }
        else
        {
            _hitCollider.SetDamage(_heavyAttackDamage);
        }

        //temporal — ventana de hit manual 
        if (_temporalManualHitWindow)
        {
            if (_temporalAttackHitRoutine != null)
                StopCoroutine(_temporalAttackHitRoutine);
            _temporalAttackHitRoutine = StartCoroutine(TemporalAttackHitWindow(attackType));
        }
    }

    //temporal
    private IEnumerator TemporalAttackHitWindow(int attackType)
    {
        yield return new WaitForSeconds(_temporalHitDelay);
        OnAttackHit(attackType);
        yield return new WaitForSeconds(_temporalHitActiveDuration);
        _temporalAttackHitRoutine = null;
        ApplyAttackEndCore();
    }

    public void OnAttackEnd()
    {
        //temporal — 
        if (_temporalAttackHitRoutine != null)
        {
            StopCoroutine(_temporalAttackHitRoutine);
            _temporalAttackHitRoutine = null;
        }

        ApplyAttackEndCore();
    }

    private void ApplyAttackEndCore()
    {
        _isAttacking = false;
        _canCombo = true;
        _currentComboIndex++;
        _hitCollider.OnAttackEnd();
    }

    public void OnAttackHit(int damageType = 0)
    {
        _hitCollider.OnAttackStart(damageType);
    }
    
    private void HandleComboReset()
    {
        if (!_isAttacking && _currentComboIndex > 0)
        {
            if (Time.time - _lastAttackTime > _comboResetTime)
            {
                _currentComboIndex = 0;
                Debug.Log("[Combat] Combo reset");
            }
        }
    }

    
    public void PerformGrab(int grabType = 0)
    {
        if (_isAttacking) return;

        
        _animator.SetInteger(Hash_GrabType, grabType);
        _animator.SetTrigger(Hash_Grab);
        _hitCollider.SetDamage(_grabDamage);

        Debug.Log($"[Combat] Grab attempt: Type={grabType}");
    }

   
    public void StartBlock()
    {
        _animator.SetBool(Hash_Block, true);
        PlayerMove.blocking = true;
    }

    
    public void EndBlock()
    {
        _animator.SetBool(Hash_Block, false);
        _animator.SetTrigger(Hash_BlockEnd);
        PlayerMove.blocking = false;
    }

    
    public void OnPlayerHit()
    {
        //temporal
        if (_temporalAttackHitRoutine != null)
        {
            StopCoroutine(_temporalAttackHitRoutine);
            _temporalAttackHitRoutine = null;
        }

        _hitCollider.OnAttackEnd();
        _isAttacking = false;
        _canCombo = true;
        _currentComboIndex = 0;

        if (_animator != null)
            _animator.SetTrigger(Hash_Hit);
    }

    
    public void PerformDash()
    {
        if (_isAttacking || _animator == null) return;
        
        _animator.SetTrigger(Animator.StringToHash("Dash"));
    }

    
    public bool IsAttacking => _isAttacking;
    public int CurrentComboIndex => _currentComboIndex;

    
    public void SetLightAttackDamage(int damage) => _lightAttackDamage = damage;
    public void SetHeavyAttackDamage(int damage) => _heavyAttackDamage = damage;
    public void SetComboResetTime(float time) => _comboResetTime = time;
}
