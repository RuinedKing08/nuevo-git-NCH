using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class PlayerHealth : HealthController
{
    public static PlayerHealth Instance { get; private set; }
    private PlayerCombatSystem _combatSystem;    
    [SerializeField] private Image lifeBar;
    private string thisScene;
    private float _damageResistance = 0f; // 0 = 0%, 1 = 100%
    public float CurrentHealth => health;
    public float MaxHealth => maxHealth;

    protected override void Start()
    {
        base.Start();
        Instance = this;
        thisScene = SceneManager.GetActiveScene().name;
        _combatSystem = GetComponent<PlayerCombatSystem>();
    }

    
    protected override void SetLife()
    {
        if (maxHealth <= 0) maxHealth = 100;
        health = maxHealth;
        SetHealthHUD();
    }

    public override void GetLife(float heal)
    {
        health += heal;
        SetHealthHUD();
        if (health > maxHealth) health = maxHealth;
    }
     public override void GetMaxLife(float extraHealth)
    {
        maxHealth += extraHealth;
        SetHealthHUD();
        if (maxHealth < 1) maxHealth = 1;
        if (health > maxHealth) health = maxHealth;
    }

    
    protected override void GetDamage(int damage)
    {
       if (PlayerActions.blocking)
        {
            _combatSystem.PlayBlockHit();
        }

        
        float damageAfterResistance = damage * (1f - _damageResistance);
        int finalDamage = Mathf.RoundToInt(damageAfterResistance);
        
        health = Mathf.Max(0, health - finalDamage);
        _combatSystem.PlayTakeHit();
        gameObject.SendMessage("OnHit", SendMessageOptions.DontRequireReceiver);

        if (health <= 0) Die();
        SetHealthHUD();

    }

   public void TakeDamage(int damage)
    {       
        ApplyDamage(damage);
        ResetCombo();
    }

    public void ResetCombo()
    {
        ChangeCombo.Instance.Combo(true);
        HitBlur.Instance.Play();
    }

    void SetHealthHUD()
    {
        lifeBar.fillAmount = health / maxHealth;
    }
    void Die()
    {
       
        SceneManager.LoadScene(thisScene);
        gameObject.SendMessage("OnPlayerDeath", SendMessageOptions.DontRequireReceiver);
    }

   
    public void SetDamageResistance(float resistance)
    {
        _damageResistance = Mathf.Clamp01(resistance);
    }

    
    public float GetDamageResistance() => _damageResistance;

   
    public void ModifyDamageResistance(float amount)
    {
        _damageResistance = Mathf.Clamp01(_damageResistance + amount);
    }
   
    public int GetDamageAfterResistance(int damage)
    {
        float damageAfterResistance = damage * (1f - _damageResistance);
        return Mathf.RoundToInt(damageAfterResistance);
    }
}
