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
    public float CurrentHealth => health;
    public float MaxHealth => maxHealth;

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
        SetHealthHUD();
    }

    protected override void GetLife(int heal)
    {
        health += heal;
        SetHealthHUD();
        if (health > maxHealth) health = maxHealth;
    }
     protected override void GetMaxLife(int extraHealth)
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
            return;
        }

        health = Mathf.Max(0, health - damage);
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
}
