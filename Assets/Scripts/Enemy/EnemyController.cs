using UnityEngine;


[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(EnemyStats))]
[RequireComponent(typeof(EnemyDetectionArea))]
public class EnemyController : MonoBehaviour
{
    [HideInInspector] public Animator Animator;
    [HideInInspector] public EnemyStats Stats;
    [HideInInspector] public EnemyDetectionArea Detection;
    [HideInInspector] public EnemyCombatGroup CombatGroup;

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

        TryRegisterToNearestCombatGroup();
    }
    void Start()
    {
        var initialState = Stats.StartsResting ? (IEnemyState)RestState : PatrolState;
        StateMachine.Initialize(initialState);
    }


    void Update()   => StateMachine.CurrentState.UpdateState();
    void FixedUpdate() => StateMachine.CurrentState.FixedUpdateState();

    public void TakeDamage(int amount, Vector3 hitDirection)
    {
        Stats.TakeDamage(amount);
        if (StateMachine.CurrentState == CombatState)
            CombatState.OnHitInterrupt();

        Animator.SetTrigger(Hash_IsHit);

        if (Stats.IsDead)
            Die();
    }

    void Die()
    {
        Animator.SetTrigger(Hash_IsDead);
        Stats.Specialty?.OnDefeated(this);
        CombatGroup?.RemoveEnemy(this);
        enabled = false;
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
}
