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

        if (Stats.NavAgent != null)
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
        if (Stats.Specialty == null)
        {
            Debug.Log($"[BOSS] {name} - Asignando BareKnuckleSpecialty inicial");
            Stats.SetSpecialty(new BareKnuckleSpecialty());
        }
        _currentSpecialty = Stats.Specialty;
    }

    public void Update()
    {
        
    }

   
    public void TakeDamage(int amount)
    {
        CurrentHealth -= amount;
        CurrentHealth = Mathf.Max(CurrentHealth, 0);
        
        Debug.Log($"[BOSS] {name} tomó {amount} daño. Salud: {CurrentHealth}/{MaxHealth}");

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
        Debug.Log($"[BOSS] {name} - Aliados atacando. Boss preparándose para contraataque.");
       
        
    }

    
    private void RegisterInGroup()
    {
        if (CombatGroup == null)
            CombatGroup = FindFirstObjectByType<EnemyCombatGroup>();
        
        if (CombatGroup != null)
            CombatGroup.AddEnemy(null); 
    }
   
    private void Die()
    {
        Debug.Log($"[BOSS] {name} derrotado. Fase/oleada terminada");
        
        Animator.SetTrigger(Hash_IsDead);
        Animator.SetBool(Hash_DeadBool, true);
        
        if (Stats.NavAgent != null)
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

    private void OnDestroy()
    {
        if (CombatGroup != null) 
            CombatGroup.RemoveEnemy(null); 
    }
}
