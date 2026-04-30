using UnityEngine;
using UnityEngine.AI;

public class EnemyStats : MonoBehaviour
{
    [Header("Health")]
    public int MaxHealth = 100;
    public int CurrentHealth;

    [Header("State")]
    public bool StartsResting = false;

    [Header("Navigation")]
    public NavMeshAgent NavAgent;
    public float FollowStopDistance = 1.5f;
    public Vector3 InitialPosition;

    [Header("Patrol")]
    public EnemyPatrolPath PatrolPath;

    [Header("Optional")]
    public IEnemySpecialty Specialty { get; private set; } 

    public bool IsDead => CurrentHealth <= 0;

    public int Damage = 10;
    public bool WasKilledByFinisher;
    public bool IsBlocking;

    void Awake()
    {
        CurrentHealth = MaxHealth;
        InitialPosition = transform.position;

        if (!NavAgent)
            NavAgent = GetComponent<NavMeshAgent>();

        if (NavAgent != null)
        {
            NavAgent.autoBraking = true;
            NavAgent.stoppingDistance = FollowStopDistance;
        }
    }

    public Vector3 GetSafeDestination(Vector3 target)
    {
        Vector3 direction = target - transform.position;
        float distance = direction.magnitude;
        if (distance <= FollowStopDistance || distance <= 0.001f)
            return transform.position;

        return target - direction.normalized * FollowStopDistance;
    }

    public void TakeDamage(int amount)
    {
        CurrentHealth -= amount;
        CurrentHealth = Mathf.Max(CurrentHealth, 0);
    }

    public void SetSpecialty(IEnemySpecialty specialty)
    {
        Specialty = specialty;
        specialty.OnAssigned(GetComponent<EnemyController>());
    }

    // ========================
    // WEAPONS / THROWABLES
    // ========================

    EnemyMeleeWeapon _equippedWeapon;
    EnemyThrowableObject _equippedThrowable;

    public bool HasWeapon => _equippedWeapon != null;
    public bool HasThrowable => _equippedThrowable != null;

    public void EquipWeapon(EnemyMeleeWeapon weapon)
    {
        _equippedWeapon = weapon;
        weapon?.Pickup();
    }

    public void DropWeapon()
    {
        _equippedWeapon?.Drop();
        _equippedWeapon = null;
    }

    public void ThrowWeapon(Vector3 target)
    {
        _equippedWeapon?.Throw(target);
        _equippedWeapon = null;
    }

    public void EquipThrowable(EnemyThrowableObject obj)
    {
        _equippedThrowable = obj;
        obj?.Pickup();
    }

    public void DropThrowable()
    {
        _equippedThrowable?.Drop();
        _equippedThrowable = null;
    }

    public void ThrowObject(Vector3 target)
    {
        _equippedThrowable?.Throw(target);
        _equippedThrowable = null;
    }
    
    public void ReloadThrowable()
    {
        // lógica opcional (puedes dejar vacío por ahora)
    }
}