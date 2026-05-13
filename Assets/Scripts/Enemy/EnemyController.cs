using UnityEngine;
using System.Collections.Generic;


[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(EnemyStats))]
[RequireComponent(typeof(EnemyDetectionArea))]
public class EnemyController : MonoBehaviour
{
    [Header("Debug Info")]
    [SerializeField] private string _currentStateName; 
    [SerializeField] private string _currentSubStateName;
    
    [HideInInspector] public Animator Animator;
    [HideInInspector] public EnemyStats Stats;
    [HideInInspector] public EnemyDetectionArea Detection;
    [HideInInspector] public EnemyCombatGroup CombatGroup;

    [Header("Hit Interrupt")]
    [SerializeField] private float interruptDuration = 0.75f;
    [SerializeField] private Color interruptColor = Color.yellow;

    [Header("Attack Feedback")]
    [SerializeField] private Color attackColor = Color.red;

    private float _interruptTimer;
    private bool _isInterrupted;
    private bool _isAttackColorActive;
    private Material[] _interruptMaterials;
    private int[] _interruptMaterialPropertyIds;
    private Color[] _interruptOriginalColors;

    public EnemyStateMachine StateMachine { get; private set; }

    public PatrolState PatrolState { get; private set; }
    public RestState RestState { get; private set; }
    public AlertState AlertState { get; private set; }
    public CombatState CombatState { get; private set; }

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


    

    void Awake()
    {
        Animator  = GetComponent<Animator>();
        Stats     = GetComponent<EnemyStats>();
        Detection = GetComponent<EnemyDetectionArea>();

        StateMachine = new EnemyStateMachine();

        PatrolState = new PatrolState(this, StateMachine);
        RestState   = new RestState(this, StateMachine);
        AlertState  = new AlertState(this, StateMachine);
        CombatState = new CombatState(this, StateMachine);

        TryFindMyGroup();
        InitializeInterruptMaterials();
       
    }
    void Start()
    {
        IEnemyState initialState;
        
        if (Stats.StartsResting)
        {
            initialState = RestState;
        }
        else
        {            
            int random = Random.Range(0, 3);
            initialState = random switch
            {
                0 => PatrolState,
                1 => RestState,
                _ => AlertState
            };
        }
        
        StateMachine.Initialize(initialState);
        Stats.NavAgent.avoidancePriority = Random.Range(0, 100);
        //Debug.Log("Estado inicial: " + initialState.GetType().Name);
    }


    void Update()   
    {
        if (StateMachine.CurrentState == null) return;

        
        _currentStateName = StateMachine.CurrentState.GetType().Name;

       
        if (StateMachine.CurrentState is CombatState combat)
        {
            _currentSubStateName = combat.GetSubStateName();
        }
        else
        {
            _currentSubStateName = "-"; 
        }

        if (_interruptTimer > 0f)
        {
            _interruptTimer -= Time.deltaTime;
            if (_interruptTimer <= 0f)
                EndInterrupt();
            return;
        }

        StateMachine.CurrentState.UpdateState();
    }
    
    void FixedUpdate() => StateMachine.CurrentState.FixedUpdateState();

    public void TakeDamage(int amount, Vector3 hitDirection)
    {
        Stats.TakeDamage(amount);
        Interrupt();

        if (StateMachine.CurrentState == CombatState)
            CombatState.OnHitInterrupt();

        Animator.SetTrigger(Hash_IsHit);

        if (Stats.IsDead)
            Die();
    }

    public bool IsInterrupted => _isInterrupted;

    public void ApplyAttackColor()
    {
        if (_isInterrupted) return;
        _isAttackColorActive = true;
        SetMaterialColor(attackColor);
    }

    public void EndAttackColor()
    {
        _isAttackColorActive = false;
        if (!_isInterrupted)
            RestoreOriginalColor();
    }

    private void Interrupt()
    {
        _isInterrupted = true;
        _interruptTimer = interruptDuration;
        if (Stats.NavAgent != null && Stats.NavAgent.isOnNavMesh)
            Stats.NavAgent.isStopped = true;

        SetMaterialColor(interruptColor);
    }

    private void EndInterrupt()
    {
        _isInterrupted = false;
        _interruptTimer = 0f;
        if (Stats.NavAgent != null && Stats.NavAgent.isOnNavMesh)
            Stats.NavAgent.isStopped = false;

        if (!_isAttackColorActive)
            RestoreOriginalColor();
    }

    private void InitializeInterruptMaterials()
    {
        var renderers = GetComponentsInChildren<Renderer>(includeInactive: true);
        var materials = new List<Material>();
        var propertyIds = new List<int>();
        var originalColors = new List<Color>();

        foreach (var renderer in renderers)
        {
            var mats = renderer.materials;
            for (int i = 0; i < mats.Length; i++)
            {
                var mat = mats[i];
                if (mat.HasProperty(Shader.PropertyToID("_Color")))
                {
                    materials.Add(mat);
                    propertyIds.Add(Shader.PropertyToID("_Color"));
                    originalColors.Add(mat.GetColor("_Color"));
                }
                else if (mat.HasProperty(Shader.PropertyToID("_BaseColor")))
                {
                    materials.Add(mat);
                    propertyIds.Add(Shader.PropertyToID("_BaseColor"));
                    originalColors.Add(mat.GetColor("_BaseColor"));
                }
            }
        }

        _interruptMaterials = materials.ToArray();
        _interruptMaterialPropertyIds = propertyIds.ToArray();
        _interruptOriginalColors = originalColors.ToArray();
    }

    private void SetMaterialColor(Color color)
    {
        if (_interruptMaterials == null) return;

        for (int i = 0; i < _interruptMaterials.Length; i++)
        {
            _interruptMaterials[i].SetColor(_interruptMaterialPropertyIds[i], color);
        }
    }

    private void RestoreOriginalColor()
    {
        if (_interruptMaterials == null) return;

        for (int i = 0; i < _interruptMaterials.Length; i++)
        {
            _interruptMaterials[i].SetColor(_interruptMaterialPropertyIds[i], _interruptOriginalColors[i]);
        }
    }

    void Die()
    {
        Animator.SetTrigger(Hash_IsDead);
        Stats.Specialty?.OnDefeated(this);
        CombatGroup?.RemoveEnemy(this);
        enabled = false;
        Destroy(gameObject, 2f);
    }

    
    public void ApplyMeleeDamage(Vector3 center, float radius, int damage)
    {
       // Debug.Log($"ApplyMeleeDamage: center={center} radius={radius} damage={damage}");
        var cols = Physics.OverlapSphere(center, radius);
       // Debug.Log($"ApplyMeleeDamage: OverlapSphere found {cols.Length} colliders");
        foreach (var c in cols)
        {
            //Debug.Log($" ApplyMeleeDamage collider: {c.name} layer={LayerMask.LayerToName(c.gameObject.layer)}");
            var ph = c.GetComponentInParent<PlayerHealth>();
            if (ph != null)
            {
               // Debug.Log($" ApplyMeleeDamage -> applying {damage} to PlayerHealth on {ph.gameObject.name}");
                ph.TakeDamage(damage);
                var pc = c.GetComponentInParent<PlayerController>();
                pc?.SendMessage("OnHit", transform.position, SendMessageOptions.DontRequireReceiver);
                return;
            }

            var pcFallback = c.GetComponentInParent<PlayerController>();
            if (pcFallback != null)
            {
                //Debug.Log($" ApplyMeleeDamage -> sending TakeDamage to PlayerController on {pcFallback.gameObject.name}");
                pcFallback.gameObject.SendMessage("TakeDamage", damage, SendMessageOptions.DontRequireReceiver);
                pcFallback.gameObject.SendMessage("OnHit", transform.position, SendMessageOptions.DontRequireReceiver);
                return;
            }
        }
    }
    void TryRegisterToNearestCombatGroup()
    {
        var groups = FindObjectsOfType<EnemyCombatGroup>();
        if (groups == null || groups.Length == 0) return;

        EnemyCombatGroup nearest = null;
        float bestDist = float.MaxValue;
        foreach (var g in groups)
        {
            float d = Vector3.Distance(transform.position, g.transform.position);
            if (d < bestDist)
            {
                bestDist = d;
                nearest = g;
            }
        }

        if (nearest != null)
            nearest.AddEnemy(this);
    }
    void TryFindMyGroup()
    {   
        var groups = FindObjectsOfType<EnemyCombatGroup>();
        EnemyCombatGroup nearest = null;
        float bestDist = float.MaxValue;

        foreach (var g in groups)
        {
            float d = Vector3.Distance(transform.position, g.transform.position);
            if (d < bestDist) { bestDist = d; nearest = g; }
        }
        
        if (nearest != null)
            this.CombatGroup = nearest;
    }

    
    public void SetAttackDamage(int damage)
    {
        var handlers = GetComponentsInChildren<AttackColliderHandler>();
        foreach (var handler in handlers)
        {
            handler.SetDamage(damage);
        }
    }

    
    public void EnableAttackColliders()
    {
        var handlers = GetComponentsInChildren<AttackColliderHandler>();
        foreach (var handler in handlers)
        {
            handler.OnAttackStart();
        }
    }

    
    public void DisableAttackColliders()
    {
        var handlers = GetComponentsInChildren<AttackColliderHandler>();
        foreach (var handler in handlers)
        {
            handler.OnAttackEnd();
        }
    }
}
