using System.Collections;
using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class EnemyController : MonoBehaviour
{
    public EnemyStateMachine StateMachine { get; private set; }
    public PatrolState PatrolState { get; private set; }
    public RestState RestState { get; private set; }
    public AlertState AlertState { get; private set; }
    public CombatState CombatState { get; private set; }

    public Animator Animator;
    public EnemyStats Stats;
    public EnemyDetectionArea Detection;
    public EnemyCombatGroup CombatGroup;
    public event Dead OnDead;
    public delegate void Dead();
    // Hashes
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
    
    // Ragdoll components
    [SerializeField] private Collider _mainCollider;
    [SerializeField] private Rigidbody _mainRigidbody;
    [SerializeField] private Avatar _ragdollAvatar; 
    private Avatar _originalAvatar; 
    [SerializeField] private Transform[] _excludeFromRagdoll; 
    [SerializeField] private Rigidbody[] _ragdollRigidbodies;
    [SerializeField] private Collider[] _ragdollColliders;
    [SerializeField] private bool _isRagdollActive = false;
    [SerializeField] private float _ragdollPushForce = 20f;
    
    private void Awake()
    {

        Animator = GetComponentInChildren<Animator>();
        Stats = GetComponent<EnemyStats>();
        Detection = GetComponent<EnemyDetectionArea>();
        
        StateMachine = new EnemyStateMachine();
        PatrolState = new PatrolState(this, StateMachine);
        RestState = new RestState(this, StateMachine);
        AlertState = new AlertState(this, StateMachine);
        CombatState = new CombatState(this, StateMachine);

        CombatGroup = Object.FindFirstObjectByType<EnemyCombatGroup>();

        if (Stats.NavAgent != null)
        {
            Stats.NavAgent.updateRotation = false; 
        }

        IndividualSeed = UnityEngine.Random.Range(0f, 1000f);
    }

    void Start()
    {
        RegisterInGroup();
        StateMachine.Initialize(PatrolState);
        if (Stats.Specialty == null)
        {
            Debug.Log($"[SISTEMA] Asignando BareKnuckleSpecialty a {name}");
            Stats.SetSpecialty(new BareKnuckleSpecialty());
        }
        if (Stats.NavAgent != null)
        {            
            Stats.NavAgent.obstacleAvoidanceType = UnityEngine.AI.ObstacleAvoidanceType.MedQualityObstacleAvoidance;
            
            Stats.NavAgent.radius = 0.35f; 
        }
        
        // Inicializar ragdoll
        InitializeRagdoll();
        OnDead += Die;
    }
    
    private void InitializeRagdoll()
    {
        _mainCollider = GetComponent<Collider>();
        _mainRigidbody = GetComponent<Rigidbody>();
        
        
        List<Rigidbody> rbList = new List<Rigidbody>();
        List<Collider> colList = new List<Collider>();
        
        foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>())
        {
            if (!IsExcluded(rb.transform))
                rbList.Add(rb);
        }
        
        foreach (Collider col in GetComponentsInChildren<Collider>())
        {
            if (!IsExcluded(col.transform))
                colList.Add(col);
        }
        
        _ragdollRigidbodies = rbList.ToArray();
        _ragdollColliders = colList.ToArray();
        
        // Guardar el avatar original
        if (Animator != null)
            _originalAvatar = Animator.avatar;
        
        
        SetRagdoll(false);
    }
    
    private bool IsExcluded(Transform bone)
    {
        if (_excludeFromRagdoll == null || _excludeFromRagdoll.Length == 0)
            return false;
        
        foreach (Transform excluded in _excludeFromRagdoll)
        {
            if (bone == excluded)
                return true;
        }
        return false;
    }

    void Update()
    {
        StateMachine.CurrentState.UpdateState();
        UpdateAnimationParameters(); 
    }
    void UpdateAnimationParameters()
    {
        if (Stats.NavAgent == null) return;
        
        Vector3 localVelocity = transform.InverseTransformDirection(Stats.NavAgent.velocity);
        float maxSpeed = Stats.NavAgent.speed > 0 ? Stats.NavAgent.speed : 1f;

        float targetX = localVelocity.x / maxSpeed;
        float targetY = localVelocity.z / maxSpeed;

        Animator.SetFloat(Hash_MoveX, targetX, 0.2f, Time.deltaTime);
        Animator.SetFloat(Hash_MoveY, targetY, 0.2f, Time.deltaTime);
        
        Animator.SetFloat(Hash_Speed, Stats.NavAgent.velocity.magnitude, 0.1f, Time.deltaTime);
    }

    public void TakeDamage(int amount, Vector3 hitDir)
    {
        Stats.TakeDamage(amount);
        Animator.SetTrigger(Hash_IsHit);
        if (Stats.IsDead)
        {
            OnDead?.Invoke();
            // Aplicar empuje al ragdoll después de la muerte
            StartCoroutine(PushRagdollAfterDeath(hitDir));
        }
    }
    
    private IEnumerator PushRagdollAfterDeath(Vector3 hitDir)
    {
        yield return null;
        if (_isRagdollActive)
            PushRagdoll(hitDir);
    }

    void Die()
    {
        if (_isRagdollActive) return; // Prevenir múltiples activaciones
        
        // Mostrar puntuación y dinero
        TMP_Text scoreView = transform.Find("Cyanide").Find("CanvasEnemy").Find("ScorePopUpTMP").GetComponent<TMP_Text>();
        scoreView.text = $"{score * Currency.Instance.GetScoreMultiplier()}";
        Currency.Instance.ChangeMoney(0.15f, score);
        Debug.Log("EnemyDie");
        Combo();
        
        
        SetRagdoll(true);
        
        
        Destroy(gameObject, 3f);
    }
    
    
    public void PushRagdoll(Vector3 direction, float force = -1)
    {
        if (!_isRagdollActive) return;
        
        float pushForce = force > 0 ? force : _ragdollPushForce;
        direction.Normalize();
        
        // Aplicar fuerza a todos los rigidbodies del ragdoll
        foreach (Rigidbody rb in _ragdollRigidbodies)
        {
            if (rb == _mainRigidbody) continue;
            rb.linearVelocity = direction * pushForce;
        }
    }
    
    public void SetRagdollPushForce(float force)
    {
        _ragdollPushForce = Mathf.Max(0, force);
    }
    
    
    public void SetRagdoll(bool active)
    {
        if (active == _isRagdollActive) return;
        _isRagdollActive = active;
        
        
        if (Animator != null)
        {
            Animator.enabled = !active;
            
          
            if (active && _ragdollAvatar != null)
                Animator.avatar = _ragdollAvatar;
            else if (!active && _originalAvatar != null)
                Animator.avatar = _originalAvatar;
        }
        
        
        if (_mainCollider != null)
            _mainCollider.enabled = !active;
        
        
        if (_mainRigidbody != null)
            _mainRigidbody.isKinematic = active;
        
        
        foreach (Rigidbody rb in _ragdollRigidbodies)
        {
            if (rb == _mainRigidbody) continue;
            rb.isKinematic = !active;
        }
        
        
        foreach (Collider col in _ragdollColliders)
        {
            if (col == _mainCollider) continue;
            col.enabled = active;
        }
        
        
        if (Stats.NavAgent != null && active)
            Stats.NavAgent.enabled = false;
    }
    
     public void Combo()
    {
        ChangeCombo.Instance.Combo(false);
    }

    public void EnableAttackColliders() { foreach (var h in GetComponentsInChildren<AttackColliderHandler>()) h.OnAttackStart(); }
    public void DisableAttackColliders() { foreach (var h in GetComponentsInChildren<AttackColliderHandler>()) h.OnAttackEnd(); }
    
    public void BeginAttackColliderWindow(float duration = 0.5f) { StartCoroutine(AttackHitWindow(duration)); }
    IEnumerator AttackHitWindow(float d) { EnableAttackColliders(); yield return new WaitForSeconds(d); DisableAttackColliders(); }
    public void CancelAttackColliderWindow() { StopAllCoroutines(); DisableAttackColliders(); }
    public void SetAttackDamage(int dmg) { foreach (var h in GetComponentsInChildren<AttackColliderHandler>()) h.SetDamage(dmg); }
    public void ApplyAttackColor() { }
    public void EndAttackColor() { }

    void RegisterInGroup()
    {        
        if (CombatGroup == null) CombatGroup = Object.FindFirstObjectByType<EnemyCombatGroup>();
        if (CombatGroup != null) CombatGroup.AddEnemy(this);
    }

    private void OnDestroy()
    {
        if (CombatGroup != null) CombatGroup.RemoveEnemy(this);
    }

    public void AE_EnemyAttackStart()
    {
        EnableAttackColliders();
    }

    public void AE_EnemyAttackEnd()
    {
        DisableAttackColliders();
    }

}
