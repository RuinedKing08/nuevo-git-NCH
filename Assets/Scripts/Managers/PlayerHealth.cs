using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class PlayerHealth : HealthController
{
    public static PlayerHealth Instance { get; private set; }

    [Header("Debug vida")]
    [SerializeField] private bool _debugDamage = true;

    public int CurrentHealth => health;
    public int MaxHealth => maxHealth;

    protected override void Start()
    {
        base.Start();
        Instance = this;
    }

    protected override void SetLife()
    {
        if (maxHealth <= 0) maxHealth = 100;
        health = maxHealth;
#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
#endif
    }

    protected override void GetLife(int heal)
    {
        health += heal;
        if (health > maxHealth) health = maxHealth;
#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
#endif
    }

    protected override void GetMaxLife(int extraHealth)
    {
        maxHealth += extraHealth;
        if (maxHealth < 1) maxHealth = 1;
        if (health > maxHealth) health = maxHealth;
#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
#endif
    }

    protected override void GetDamage(int damage)
    {
        if (_debugDamage)
            Debug.Log($"[PlayerHpDbg] GetDamage recibido={damage} healthAntes={health} max={maxHealth} go='{gameObject.name}'");

        if (damage <= 0)
            Debug.LogWarning($"[PlayerHpDbg] Daño recibido es {damage} — la vida no bajará (revisa ataque enemigo / multiplicadores).");

        health -= damage;
        health = Mathf.Max(0, health);
#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
#endif

        if (_debugDamage)
            Debug.Log($"[PlayerHpDbg] GetDamage después health={health}");

        gameObject.SendMessage("OnHit", SendMessageOptions.DontRequireReceiver);
        if (health == 0)
            Die();
    }

    void Die()
    {
       
        gameObject.SendMessage("OnPlayerDeath", SendMessageOptions.DontRequireReceiver);
    }

    
    public void TakeDamage(int damage)
    {
        if (_debugDamage)
            Debug.Log($"[PlayerHpDbg] TakeDamage llamado amount={damage} → ApplyDamage");
        ApplyDamage(damage);
    }

    public void TakeDamagePercent(float percent)
    {
        int dmg = Mathf.CeilToInt(maxHealth * percent);
        ApplyDamage(dmg);
    }

    public void HealMissingHealthPercent(float percent)
    {
        int heal = Mathf.CeilToInt(maxHealth * percent);
        GetLife(heal);
    }

    
    public void TryExecuteIfBelow(int threshold)
    {
        if (health <= threshold)
            gameObject.SendMessage("OnExecuteAvailable", SendMessageOptions.DontRequireReceiver);
    }
}
