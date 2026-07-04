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
    
    [Header("Specialties")]
    private IEnemySpecialty _currentSpecialty;
    private bool _hasChangedSpecialty = false;
    
    [Header("Combat")]
    private float _attackTimer = 0f;
    private bool _isAttacking = false;
    private float _attackCooldown = 2.4f;

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

    private void Awake()
    {
        CurrentHealth = MaxHealth;
        IndividualSeed = Random.Range(0f, 1000f);

        Animator = GetComponentInChildren<Animator>();
        Stats = GetComponent<EnemyStats>();
        Detection = GetComponent<EnemyDetectionArea>();

        if (Stats == null)
            Debug.LogError($"[BOSS] {name} no tiene componente EnemyStats. Asígnalo en el Inspector.");
        
        if (Detection == null)
            Debug.LogError($"[BOSS] {name} no tiene componente EnemyDetectionArea. Asígnalo en el Inspector.");
        
        if (Animator == null)
            Debug.LogWarning($"[BOSS] {name} no tiene Animator. Busca en los hijos.");

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
            Debug.Log($"[BOSS] {name} - Asignando BareKnuckleSpecialty inicial");
            Stats.SetSpecialty(new BareKnuckleSpecialty());
            _currentSpecialty = Stats.Specialty;
        }
        
        if (CombatGroup != null)
        {
            CombatGroup.OnChangeCurrentMembers += OnCombatGroupMembersChanged;
        }
    }
    
    private void OnCombatGroupMembersChanged()
    {
        if (CombatGroup != null && CombatGroup.GetCurrentAttackers().Count > 0 && !_isAttacking)
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
            float targetAttackDistance = 2.5f; // Distancia para atacar cuerpo a cuerpo
            
            if (distanceToPlayer > targetAttackDistance)
            {
                // Acercarse rápido al jugador
                Stats.NavAgent.speed = 8f;
                Stats.NavAgent.SetDestination(playerPos);
            }
            else
            {
                // Ya está lo suficientemente cerca - detenerse
                Stats.NavAgent.velocity = Vector3.zero;
                Stats.NavAgent.isStopped = true;
            }
        }
        else
        {
            // No está atacando - volver a órbita
            Stats.NavAgent.isStopped = false;
            
            // Calcular posición en órbita (boss siempre es "último" en la órbita)
            int bossIndex = CombatGroup.GetCurrentMembers().Count - 1;
            float baseAngle = bossIndex * (360f / Mathf.Max(1, CombatGroup.GetCurrentMembers().Count));
            float rotationDir = (IndividualSeed > 500) ? 1 : -1;
            float individualRotation = Time.time * (15f + (IndividualSeed % 10f)) * rotationDir;
            float finalAngle = baseAngle + individualRotation;
            
            float radiusNoise = Mathf.PerlinNoise(Time.time * 0.2f, IndividualSeed + 100f);
            float dynamicRadius = Mathf.Lerp(4.5f, 7.5f, radiusNoise);
            
            Vector3 offset = new Vector3(Mathf.Cos(finalAngle * Mathf.Deg2Rad), 0, Mathf.Sin(finalAngle * Mathf.Deg2Rad)) * dynamicRadius;
            Vector3 orbitPos = playerPos + offset;
            
            // Mover hacia la posición de órbita
            Stats.NavAgent.speed = 3.5f + (Mathf.PingPong(Time.time, 1f));
            Stats.NavAgent.SetDestination(orbitPos);
        }
        
        // Rotar hacia el jugador
        Vector3 dir = (playerPos - transform.position).normalized;
        dir.y = 0;
        if (dir.sqrMagnitude > 0.01f)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 10f);
    }
    
    private void UpdateAttackTimer()
    {
        if (_isAttacking)
        {
            _attackTimer += Time.deltaTime;
            if (_attackTimer >= _attackCooldown)
            {
                _isAttacking = false;
                _attackTimer = 0f;
            }
        }
    }

   
    public void TakeDamage(int amount)
    {
        CurrentHealth -= amount;
        CurrentHealth = Mathf.Max(CurrentHealth, 0);
        
        Debug.Log($"[BOSS] {name} tomó {amount} daño. Salud: {CurrentHealth}/{MaxHealth}");

        if (Animator != null)
            Animator.SetTrigger(Hash_IsHit);

        
        if (CurrentHealth <= _specialtyChangeThreshold && !_hasChangedSpecialty)
        {
            ChangeSpecialty();
        }

        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    
    private void ChangeSpecialty()
    {
        _hasChangedSpecialty = true;
        Debug.Log($"[BOSS] {name} - Cambiando a especialidad de Juego Sucio");
        
        // Cambiar a Juego Sucio (si está disponible)       
        
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
            ExecuteAttack();
        }
    }
    
    private void ExecuteAttack()
    {
        if (Animator == null)
        {
            Debug.LogWarning($"[BOSS] {name} no tiene Animator asignado. Intenta buscar...");
            Animator = GetComponentInChildren<Animator>();
            if (Animator == null)
            {
                Debug.LogError($"[BOSS] {name} No se encontró Animator en el GameObject o sus hijos.");
                return;
            }
        }
        
        _isAttacking = true;
        _attackTimer = 0f;
        
        Debug.Log($"[BOSS] {name} ejecutando ataque.");
        
        // Boss ataque como enemigo normal: ejecutar animación
        int chosenAttack = Random.Range(0, 2);
        Animator.SetInteger("AttackIndex", chosenAttack);
        Animator.SetTrigger(Hash_Attack);
        
        // Activar colliders de ataque por 0.5 segundos (mientras hay animación o feedback)
        SetAttackDamage(15);
        BeginAttackColliderWindow(0.5f);
        
        Debug.Log($"[BOSS] {name} activó colliders de ataque. AttackIndex: {chosenAttack}");
    }

    
    private void RegisterInGroup()
    {
        if (CombatGroup == null)
            CombatGroup = FindFirstObjectByType<EnemyCombatGroup>();
        
        if (CombatGroup != null)
        {
            Debug.Log($"[BOSS] {name} registrado en el grupo de combate");
            CombatGroup.SetBoss(this);
        }
    }
   
    private void Die()
    {
        Debug.Log($"[BOSS] {name} derrotado. Fase/oleada terminada");
        
        if (Animator != null)
        {
            Animator.SetTrigger(Hash_IsDead);
            Animator.SetBool(Hash_DeadBool, true);
        }
        
        if (Stats != null && Stats.NavAgent != null)
            Stats.NavAgent.enabled = false;

        // Notificar que la fase terminó
        
        
        Destroy(gameObject, 2.5f);
    }

    public void SetAttackDamage(int dmg) { foreach (var h in GetComponentsInChildren<AttackColliderHandler>()) h.SetDamage(dmg); }
    public void BeginAttackColliderWindow(float duration = 0.5f) { StartCoroutine(AttackHitWindow(duration)); }
    private System.Collections.IEnumerator AttackHitWindow(float d) { EnableAttackColliders(); yield return new WaitForSeconds(d); DisableAttackColliders(); }
    public void CancelAttackColliderWindow() { StopAllCoroutines(); DisableAttackColliders(); }
    public void EnableAttackColliders() { foreach (var h in GetComponentsInChildren<AttackColliderHandler>()) h.OnAttackStart(); }
    public void DisableAttackColliders() { foreach (var h in GetComponentsInChildren<AttackColliderHandler>()) h.OnAttackEnd(); }
}
