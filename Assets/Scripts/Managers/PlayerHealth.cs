using UnityEngine;

public class PlayerHealth : HealthController
{
    public static PlayerHealth Instance { get; private set; }

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
    }

    protected override void GetLife(int heal)
    {
        health += heal;
        if (health > maxHealth) health = maxHealth;
    }

    protected override void GetMaxLife(int extraHealth)
    {
        maxHealth += extraHealth;
        if (maxHealth < 1) maxHealth = 1;
        if (health > maxHealth) health = maxHealth;
    }

    protected override void GetDamage(int damage)
    {
        health -= damage;
        health = Mathf.Max(0, health);
        
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
