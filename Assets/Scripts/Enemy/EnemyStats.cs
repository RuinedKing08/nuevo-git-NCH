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

        if (!NavAgent)
            NavAgent = GetComponent<NavMeshAgent>();
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