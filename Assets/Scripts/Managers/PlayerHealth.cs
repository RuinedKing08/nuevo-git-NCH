using UnityEngine;
using UnityEditor;


public class PlayerHealth : HealthController
{
    public static PlayerHealth Instance { get; private set; }
    private PlayerCombatSystem _combatSystem;    

    protected override void Start()
    {
        base.Start();
        Instance = this;
        _combatSystem = GetComponent<PlayerCombatSystem>();
    }

    
    protected override void SetLife()
    {
        if (maxHealth <= 0) maxHealth = 100;
        health = maxHealth;
    }

    protected override void GetLife(int heal) => health = Mathf.Min(maxHealth, health + heal);
    protected override void GetMaxLife(int extra) => maxHealth += extra;

    
    protected override void GetDamage(int damage)
    {
       if (PlayerActions.blocking)
        {
            _combatSystem.PlayBlockHit();
            return;
        }

        health = Mathf.Max(0, health - damage);
        _combatSystem.PlayTakeHit();
        gameObject.SendMessage("OnHit", SendMessageOptions.DontRequireReceiver);

        if (health <= 0) Die();
    }

    public void TakeDamage(int damage) => ApplyDamage(damage);
    private void Die() => gameObject.SendMessage("OnPlayerDeath", SendMessageOptions.DontRequireReceiver);
}
