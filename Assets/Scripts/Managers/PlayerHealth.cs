using UnityEngine;
using UnityEditor;


public class PlayerHealth : HealthController
{
    public static PlayerHealth Instance { get; private set; }
    private PlayerCombatSystem _combatSystem;

    [Header("Debug")]
    [SerializeField] private bool _debugDamage = true;

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
        if (_debugDamage)
            //Debug.Log($"[PlayerHealth] Daño recibido: {damage}. Bloqueando: {PlayerActions.blocking}");

        
        if (PlayerActions.blocking)
        {
            if (_combatSystem != null) _combatSystem.PlayBlockHit();
             
            return; 
        }

       
        health -= damage;
        health = Mathf.Max(0, health);

        if (_combatSystem != null) 
            _combatSystem.PlayTakeHit();

        
        gameObject.SendMessage("OnHit", SendMessageOptions.DontRequireReceiver);

        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
       // Debug.Log("Jugador ha muerto");
        gameObject.SendMessage("OnPlayerDeath", SendMessageOptions.DontRequireReceiver);
    }

    
    public void TakeDamage(int amount) => ApplyDamage(amount);
    
    public int CurrentHealth => health;
    public int MaxHealth => maxHealth;
}
