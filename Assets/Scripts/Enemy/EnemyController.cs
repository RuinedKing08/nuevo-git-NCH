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
    bool ragdoll;
    // Ragdoll components
    [SerializeField] private Collider mainCollider;
    [SerializeField] private Rigidbody mainRigidbody;
    [SerializeField] private GameObject scorePopUp;
    [SerializeField] private float ragdollPushForce = 30f;
    private Rigidbody[] ragdollRigidbodies;
    private Collider[] ragdollColliders;
    
    
    private AudioSource movementSoundSource;
    private bool wasMoving = false;
    
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
        
        // Inicializar ragdoll
        mainCollider = GetComponent<Collider>();
        mainRigidbody = GetComponent<Rigidbody>();
        ragdollRigidbodies = GetComponentsInChildren<Rigidbody>();
        ragdollColliders = GetComponentsInChildren<Collider>();
        SetRagdoll(false);

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
        
        OnDead += Die;
    }


    void Update()
    {
        StateMachine.CurrentState.UpdateState();
        UpdateAnimationParameters();
        
        
        bool isMoving = Stats.NavAgent.velocity.magnitude >= 0.01f;
        
        if (isMoving && !wasMoving)
        {
            
            movementSoundSource = AudioManager.Instance.Play(Stats.MovementSound,transform.position);
        }
        else if (!isMoving && wasMoving)
        {
            
            AudioManager.Instance.Stop(movementSoundSource);
        }
        
        wasMoving = isMoving;
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
        if (ragdoll) return;
        Stats.TakeDamage(amount);
        Animator.SetTrigger(Hash_IsHit);        
        if (Stats.IsDead)
        {
            OnDead?.Invoke();
            // Activar ragdoll y aplicar fuerza cuando muere
            SetRagdoll(true);
            PushRagdoll(hitDir, ragdollPushForce);
            AudioManager.Instance.Play(Stats.HitSound);
        }
    }

    void Die()
    {
        // Mostrar puntuación y dinero
        TMP_Text scoreView = transform.Find("Cyanide").Find("CanvasEnemy").Find("ScorePopUpTMP").GetComponent<TMP_Text>();
        scoreView.text = $"{score * Currency.Instance.GetScoreMultiplier()}";
        Currency.Instance.ChangeMoney(0.15f, score);
        Debug.Log("EnemyDie");
        Combo();
        ChangeExp(20);
        KillToHeal();
        Waves.Instance.ChangeWave();
        Destroy(gameObject, 3f);
    }
    
    public void SetRagdoll(bool active)
    {
        Animator.enabled = !active;
        ragdoll = active;
        if (mainCollider != null)
            mainCollider.enabled = !active;

        if (mainRigidbody != null)
            mainRigidbody.isKinematic = active;

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
        
        // Desactivar NavAgent cuando ragdoll está activo
        if (Stats.NavAgent != null)
            Stats.NavAgent.enabled = !active;
        scorePopUp.SetActive(active);
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
