using UnityEngine;
using UnityEngine.AI;

public class BossController : MonoBehaviour
{
    [Header("Stats")]
    public int MaxHealth = 10;
    public int CurrentHealth;
    private int _specialtyChangeThreshold = 4;

    [Header("References")]
    public Animator Animator;
    public EnemyStats Stats;
    public EnemyDetectionArea Detection;
    public EnemyCombatGroup CombatGroup;
   
    public event Dead OnDead;
    public delegate void Dead();

    [Header("Specialties")]
    private IEnemySpecialty _currentSpecialty;
    private bool _hasChangedSpecialty = false;
    
    [Header("Combat")]
    private float _attackTimer = 0f;
    [SerializeField] private bool _isAttacking = false;
    private bool _attackExecuted = false;
    private float _attackCooldown = 2.4f;
    private float _postAttackCooldown = 0.6f;
    
    [Header("Auto Attack")]
    [SerializeField] private float _autoAttackInterval = 5f;
    private float _autoAttackTimer = 0f;

    [Header("Ragdoll")]
    private bool _ragdollActive = false;
    [SerializeField] private Collider mainCollider;
    [SerializeField] private Rigidbody mainRigidbody;
    [SerializeField] private float ragdollPushForce = 30f;
    private Rigidbody[] ragdollRigidbodies;
    private Collider[] ragdollColliders;

    public static readonly int Hash_Speed = Animator.StringToHash("Speed");
    public static readonly int Hash_IsAlert = Animator.StringToHash("IsAlert");
    public static readonly int Hash_InCombat = Animator.StringToHash("InCombat");
    public static readonly int Hash_AttackIndex = Animator.StringToHash("AttackIndex");
    public static readonly int Hash_IsDefending = Animator.StringToHash("IsDefending");
    public static readonly int Hash_IsHit = Animator.StringToHash("IsHit");
    public static readonly int Hash_IsDead = Animator.StringToHash("IsDead");
    public static readonly int Hash_MoveX = Animator.StringToHash("MoveX");
    public static readonly int Hash_MoveY = Animator.StringToHash("MoveY");
    public static readonly int Hash_Attack = Animator.StringToHash("Attack");
    public static readonly int Hash_DeadBool = Animator.StringToHash("Dead");

    public float IndividualSeed { get; private set; }
    [SerializeField] float score;

    private void Awake()
    {
        CurrentHealth = MaxHealth;
        IndividualSeed = Random.Range(0f, 1000f);

        mainCollider = GetComponent<Collider>();
        Animator = GetComponentInChildren<Animator>();
        Stats = GetComponent<EnemyStats>();
        Detection = GetComponent<EnemyDetectionArea>();

        if (Stats != null && Stats.NavAgent != null)
        {
            Stats.NavAgent.updateRotation = false;
            Stats.NavAgent.autoBraking = false;
            Stats.NavAgent.obstacleAvoidanceType = ObstacleAvoidanceType.MedQualityObstacleAvoidance;
            Stats.NavAgent.radius = 0.35f;
        }

        // Initialize ragdoll
        mainCollider = GetComponent<Collider>();
        mainRigidbody = GetComponent<Rigidbody>();
        ragdollRigidbodies = GetComponentsInChildren<Rigidbody>();
        
        Transform modelRoot = transform.Find("bigbad");
        if (modelRoot != null)
            ragdollColliders = modelRoot.GetComponentsInChildren<Collider>();
        else
            ragdollColliders = GetComponentsInChildren<Collider>();
        SetRagdoll(false);
    }

    private void Start()
    {
        RegisterInGroup();
        
        if (Stats != null && Stats.Specialty == null)
        {
            Stats.SetSpecialty(new BareKnuckleSpecialty());
            _currentSpecialty = Stats.Specialty;
        }
        
        if (CombatGroup != null)
        {
            CombatGroup.OnChangeCurrentMembers += OnCombatGroupMembersChanged;
        }
        OnDead += Die;
    }
    public void InvokeOnDead() { OnDead?.Invoke(); }

    private void OnCombatGroupMembersChanged()
    {
        if (CombatGroup != null && CombatGroup.GetCurrentAttackers().Count > 0 && !_isAttacking && CombatGroup.ShouldBossAttack())
        {
            OnAlliesAttacking();
        }
    }
    
    private void OnDestroy()
    {
        if (CombatGroup != null)
        {
            CombatGroup.OnChangeCurrentMembers -= OnCombatGroupMembersChanged;
        }
    }

    public void Update()
    {
        UpdateAttackTimer();
        UpdateAutoAttackTimer();
        UpdateMovement();
        UpdateAnimationParameters();
    }
    
    private void UpdateAnimationParameters()
    {
        if (Animator == null || Stats == null || Stats.NavAgent == null)
            return;
        
        Vector3 localVelocity = transform.InverseTransformDirection(Stats.NavAgent.velocity);
        float maxSpeed = Stats.NavAgent.speed > 0 ? Stats.NavAgent.speed : 1f;

        float targetX = localVelocity.x / maxSpeed;
        float targetY = localVelocity.z / maxSpeed;

        Animator.SetFloat("MoveX", targetX, 0.2f, Time.deltaTime);
        Animator.SetFloat("MoveY", targetY, 0.2f, Time.deltaTime);
        Animator.SetFloat("Speed", Stats.NavAgent.velocity.magnitude, 0.1f, Time.deltaTime);
    }
    
    private void UpdateMovement()
    {
        if (Detection == null || Stats == null || Stats.NavAgent == null || CombatGroup == null)
            return;
        
        Vector3 playerPos = Detection.LastKnownPlayerPosition;
        
        if (_isAttacking)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, playerPos);
            float targetAttackDistance = 2.7f;
            
            if (distanceToPlayer > targetAttackDistance)
            {
                Stats.NavAgent.stoppingDistance = 0f;
                Stats.NavAgent.speed = 8f;
                Stats.NavAgent.acceleration = 20f;
                Stats.NavAgent.SetDestination(playerPos);
                Stats.NavAgent.isStopped = false;
            }
            else
            {
                Stats.NavAgent.velocity = Vector3.zero;
                Stats.NavAgent.isStopped = true;
                
                if (!_attackExecuted)
                {
                    ExecuteAttack();
                    _attackExecuted = true;
                    _attackTimer = _attackCooldown - _postAttackCooldown;
                }
            }
            
            Vector3 dir = (playerPos - transform.position).normalized;
            dir.y = 0;
            if (dir.sqrMagnitude > 0.01f)
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 10f);
        }
        else
        {
            Stats.NavAgent.speed = 6f;
            Stats.NavAgent.SetDestination(CombatGroup.GetBossOrbitPosition(playerPos));
            
            Vector3 dir = (playerPos - transform.position).normalized;
            dir.y = 0;
            if (dir.sqrMagnitude > 0.01f)
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 10f);
        }
    }
    
    private void UpdateAttackTimer()
    {
        if (_isAttacking || _attackExecuted)
        {
            _attackTimer += Time.deltaTime;
            if (_attackTimer >= _attackCooldown)
            {
                _isAttacking = false;
                _attackExecuted = false;
                _attackTimer = 0f;
                if (Stats != null && Stats.NavAgent != null)
                {
                    Stats.NavAgent.isStopped = false;
                    Stats.NavAgent.stoppingDistance = 2.5f;
                }
            }
        }
    }

    private void UpdateAutoAttackTimer()
    {
        if (CombatGroup != null && CombatGroup.GetCurrentAttackers().Count == 0 && !_isAttacking)
        {
            _autoAttackTimer += Time.deltaTime;
            if (_autoAttackTimer >= _autoAttackInterval)
            {
                _autoAttackTimer = 0f;
                OnAlliesAttacking();
            }
        }
        else
        {
            _autoAttackTimer = 0f;
        }
    }

   
    public void TakeDamage(int amount)
    {
        if (_ragdollActive) return;
        
        CurrentHealth -= amount;
        CurrentHealth = Mathf.Max(CurrentHealth, 0);
        Debug.Log($"Boss took {amount} damage. Current health: {CurrentHealth}/{MaxHealth}");

        if (Animator != null)
            Animator.SetTrigger(Hash_IsHit);

        if (CurrentHealth <= _specialtyChangeThreshold && !_hasChangedSpecialty)
        {
            ChangeSpecialty();
        }

        if (CurrentHealth <= 0)
        {
            if (mainCollider != null)
                mainCollider.enabled = false;
            InvokeOnDead();
        }
    }

    
    private void ChangeSpecialty()
    {
        _hasChangedSpecialty = true;
    }
    
    public void NotifyExchangeReady()
    {
        if (CombatGroup != null)
        {
            CombatGroup.NotifyExchangeReady(null); 
        }
    }

    
    public void ReportAttackFinished()
    {
        if (CombatGroup != null)
        {
            CombatGroup.ReportAttackFinished(null);
        }
    }

    
    public void OnAlliesAttacking()
    {
        if (!_isAttacking)
        {
            _isAttacking = true;
            _attackExecuted = false;
            _attackTimer = 0f;
        }
    }
    
    private void ExecuteAttack()
    {
        if (Animator == null)
        {
            Animator = GetComponentInChildren<Animator>();
            if (Animator == null)
                return;
        }
        
        int chosenAttack = Random.Range(0, 2);
        Animator.SetInteger("AttackIndex", chosenAttack);
        Animator.SetTrigger(Hash_Attack);
        
        SetAttackDamage(15);

    }

    
    private void RegisterInGroup()
    {
        if (CombatGroup == null)
            CombatGroup = FindFirstObjectByType<EnemyCombatGroup>();
        
        if (CombatGroup != null)
        {
            CombatGroup.SetBoss(this);
        }
    }
   
    private void Die()
    {
        // Activate ragdoll
        SetRagdoll(true);
        PushRagdoll(Vector3.zero, ragdollPushForce);
        
        if (Animator != null)
        {
            Animator.SetTrigger(Hash_IsDead);
            Animator.SetBool(Hash_DeadBool, true);
        }
        
        if (Stats != null && Stats.NavAgent != null)
            Stats.NavAgent.enabled = false;
        Currency.Instance.ChangeMoney(0.15f, score);
        Debug.Log("EnemyDie");
        Combo();
        ChangeExp(20);
        KillToHeal();
        Waves.Instance.ChangeWave();
        Destroy(gameObject, 2.5f);
    }

    public void Combo()
    {
        ChangeCombo.Instance.Combo(false);
    }
    public void ChangeExp(float exp)
    {
        Exp.Instance.ChangeExp(exp);
    }
    public void KillToHeal()
    {
        if (UniqueUpgrades.ronBuff)
        {
            switch (UniqueUpgrades.ronLevel)
            {
                case 1:
                    PlayerHealth.Instance.GetLife(UniqueUpgrades.ronAmount * PlayerHealth.Instance.MaxHealth / 100);
                    break;
                case 2:
                    PlayerHealth.Instance.GetLife(UniqueUpgrades.ronAmount * PlayerHealth.Instance.MaxHealth / 100);
                    break;
                case 3:
                    PlayerHealth.Instance.GetLife(UniqueUpgrades.ronAmount * PlayerHealth.Instance.MaxHealth / 100);
                    break;
            }
        }
    }
    public void SetAttackDamage(int dmg) { foreach (var h in GetComponentsInChildren<AttackColliderHandler>()) h.SetDamage(dmg); }
    public void BeginAttackColliderWindow(float duration = 0.5f) { StartCoroutine(AttackHitWindow(duration)); }
    private System.Collections.IEnumerator AttackHitWindow(float d) { EnableAttackColliders(); yield return new WaitForSeconds(d); DisableAttackColliders(); }
    public void CancelAttackColliderWindow() { StopAllCoroutines(); DisableAttackColliders(); }
    public void EnableAttackColliders() { foreach (var h in GetComponentsInChildren<AttackColliderHandler>()) h.OnAttackStart(); }
    public void DisableAttackColliders() { foreach (var h in GetComponentsInChildren<AttackColliderHandler>()) h.OnAttackEnd(); }

    
    public void SetRagdoll(bool active)
    {
        _ragdollActive = active;
        
        if (Animator != null)
            Animator.enabled = !active;

        if (mainCollider != null)
            mainCollider.enabled = !active;

        if (mainRigidbody != null)
            mainRigidbody.isKinematic = !active;

        foreach (Rigidbody rb in ragdollRigidbodies)
        {
            if (rb == mainRigidbody) continue;
            rb.isKinematic = !active;
        }

        foreach (Collider col in ragdollColliders)
        {
            if (col == mainCollider) continue;
            col.enabled = active;
        }
    }

    
    public void PushRagdoll(Vector3 direction, float force)
    {
        direction.Normalize();
        
        foreach (Rigidbody rb in ragdollRigidbodies)
        {
            if (rb == mainRigidbody) continue;
            rb.linearVelocity = direction * force;
        }
    }
}
