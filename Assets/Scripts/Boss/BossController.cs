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
    private bool _attackExecuted = false; // Para no repetir el ataque múltiples veces
    private float _attackCooldown = 2.4f;
    private float _postAttackCooldown = 0.6f; // Tiempo que se queda parado después de atacar

    // Hashes para Animator
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
    }

    private void Start()
    {
        RegisterInGroup();
        
        // Asignar especialidad inicial (Arma Blanca)
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
        UpdateMovement();
        UpdateAnimationParameters();
    }
    
    private void UpdateAnimationParameters()
    {
        if (Animator == null || Stats == null || Stats.NavAgent == null)
            return;
        
        // Calcular velocidad local (relativa al modelo)
        Vector3 localVelocity = transform.InverseTransformDirection(Stats.NavAgent.velocity);
        float maxSpeed = Stats.NavAgent.speed > 0 ? Stats.NavAgent.speed : 1f;

        float targetX = localVelocity.x / maxSpeed;
        float targetY = localVelocity.z / maxSpeed;

        // Enviar parámetros de movimiento al Animator (suavizado)
        Animator.SetFloat("MoveX", targetX, 0.2f, Time.deltaTime);
        Animator.SetFloat("MoveY", targetY, 0.2f, Time.deltaTime);
        Animator.SetFloat("Speed", Stats.NavAgent.velocity.magnitude, 0.1f, Time.deltaTime);
    }
    
    private void UpdateMovement()
    {
        // Boss se mueve en órbita alrededor del jugador como los enemigos
        if (Detection == null || Stats == null || Stats.NavAgent == null || CombatGroup == null)
            return;
        
        Vector3 playerPos = Detection.LastKnownPlayerPosition;
        
        // Si está atacando, se acerca al jugador para golpear
        if (_isAttacking)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, playerPos);
            float targetAttackDistance = 2.7f; // Misma distancia que EnemyController
            
            if (distanceToPlayer > targetAttackDistance)
            {
                // Acercarse rápido al jugador (igual que enemigo)
                Stats.NavAgent.stoppingDistance = 0f;
                Stats.NavAgent.speed = 8f;
                Stats.NavAgent.acceleration = 20f;
                Stats.NavAgent.SetDestination(playerPos);
                Stats.NavAgent.isStopped = false;
            }
            else
            {
                // Ya está lo suficientemente cerca - detener y atacar
                Stats.NavAgent.velocity = Vector3.zero;
                Stats.NavAgent.isStopped = true;
                
                // Ejecutar ataque solo una vez durante este ciclo
                if (!_attackExecuted)
                {
                    ExecuteAttack();
                    _attackExecuted = true;
                    // Iniciar cooldown de post-ataque (se queda parado antes de retroceder)
                    _attackTimer = _attackCooldown - _postAttackCooldown;
                    // No cambiar _isAttacking aún, esperar el cooldown
                }
            }
            
            // Rotación hacia el jugador mientras ataca (como en CombatState)
            Vector3 dir = (playerPos - transform.position).normalized;
            dir.y = 0;
            if (dir.sqrMagnitude > 0.01f)
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 10f);
        }
        else
        {
            // Cuando no está atacando, se mueve en órbita más rápido
            Stats.NavAgent.speed = 6f;
            Stats.NavAgent.SetDestination(CombatGroup.GetBossOrbitPosition(playerPos));
            
            // También rotación hacia jugador en órbita
            Vector3 dir = (playerPos - transform.position).normalized;
            dir.y = 0;
            if (dir.sqrMagnitude > 0.01f)
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 10f);
        }
    }
    
    private void UpdateAttackTimer()
    {
        // El timer continúa incluso después de atacar para completar el cooldown
        if (_isAttacking || _attackExecuted)
        {
            _attackTimer += Time.deltaTime;
            if (_attackTimer >= _attackCooldown)
            {
                // Cooldown completado, resetear todo y volver a órbita
                _isAttacking = false;
                _attackExecuted = false;
                _attackTimer = 0f;
                // Reiniciar el NavAgent para que vuelva a moverse en órbita
                if (Stats != null && Stats.NavAgent != null)
                {
                    Stats.NavAgent.isStopped = false;
                    Stats.NavAgent.stoppingDistance = 2.5f;
                }
            }
        }
    }

   
    public void TakeDamage(int amount)
    {
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
        // Solo marcar que debe atacar - el ataque se ejecutará cuando esté en rango
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
        
        // Boss ataca: ejecutar animación
        int chosenAttack = Random.Range(0, 2);
        Animator.SetInteger("AttackIndex", chosenAttack);
        Animator.SetTrigger(Hash_Attack);
        
        // Activar colliders de ataque por 0.5 segundos
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
}
