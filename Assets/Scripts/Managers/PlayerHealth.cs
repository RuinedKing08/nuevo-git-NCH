using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class PlayerHealth : HealthController
{
    public static PlayerHealth Instance { get; private set; }
    private PlayerCombatSystem _combatSystem;    
    [SerializeField] private Image lifeBar;
    [SerializeField] private CameraShake cameraShake;
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
        DJVFX.Instance.PlayHeal();
        ScorePopUp.Instance.CreateHealLifePopUp(heal);
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

        
        if (cameraShake != null)
            cameraShake.Shake();

        ScorePopUp.Instance.CreateLostLifePopUp(finalDamage);
        SetHealthHUD();
        if (health <= 0) Die();

    }

   public void TakeDamage(int damage)
    {       
        ApplyDamage(damage);
        LostDamageBuff();
        ResetCombo();
        AudioManager.Instance.Play(_combatSystem.DamageSound, transform.position);
    }
    void LostDamageBuff()
    {
        if (UniqueUpgrades.ginBuff)
        {
            switch (UniqueUpgrades.ginLevel)
            {
                case 1:
                    if(health <= 90 * maxHealth / 100)
                    {
                        DJVFX.Instance.StopDmg();
                        UniqueUpgrades.ginBuff = false;
                        UniqueUpgrades.Instance.ChangeDamage();
                    }
                    break;
                case 2:
                    if (health <= 70 * maxHealth / 100)
                    {
                        DJVFX.Instance.StopDmg();
                        UniqueUpgrades.ginBuff = false;
                        UniqueUpgrades.Instance.ChangeDamage();
                    }
                    break;
                case 3:
                    if (health <= 50 * maxHealth / 100)
                    {
                        DJVFX.Instance.StopDmg();
                        UniqueUpgrades.ginBuff = false;
                        UniqueUpgrades.Instance.ChangeDamage();
                    }
                    break;
            }
        }
        else
        {
            DJVFX.Instance.StopDmg();
        }
        if (UniqueUpgrades.whiskeyBuff)
        {
            switch (UniqueUpgrades.whiskeyLevel)
            {
                case 1:
                    if (health <= 90 * maxHealth / 100)
                    {
                        UniqueUpgrades.whiskeyBuff = false;
                    }
                    break;
                case 2:
                    if (health <= 70 * maxHealth / 100)
                    {
                        UniqueUpgrades.whiskeyBuff = false;
                    }
                    break;
                case 3:
                    if (health <= 50 * maxHealth / 100)
                    {
                        UniqueUpgrades.whiskeyBuff = false;
                    }
                    break;
            }
        }
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
    bool RevivalBuff()
    {
        if (UniqueUpgrades.brandyBuff)
        {
            switch (UniqueUpgrades.brandyLevel)
            {
                case 1:
                    GetLife(UniqueUpgrades.brandyAmount * maxHealth / 100);
                    UniqueUpgrades.brandyAmount -= UniqueUpgrades.brandyAmount;
                    break;
                case 2:
                    GetLife(UniqueUpgrades.brandyAmount * maxHealth / 100);
                    UniqueUpgrades.brandyAmount -= UniqueUpgrades.brandyAmount;
                    break;
                case 3:
                    GetLife(UniqueUpgrades.brandyAmount * maxHealth / 100);
                    UniqueUpgrades.brandyAmount -= UniqueUpgrades.brandyAmount;
                    break;
            }
            UniqueUpgrades.brandyBuff = false;
            return true;
        }
        else return false;
    }
    void Die()
    {
        if (RevivalBuff()) return;
        ChangeCards.cardsOpened = false;
        Exp.Instance.ChangeExp(-100);
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
        _damageResistance = Mathf.Clamp(_damageResistance, 0,0.5f);
    }
   
    public int GetDamageAfterResistance(int damage)
    {
        float damageAfterResistance = damage * (1f - _damageResistance);
        return Mathf.RoundToInt(damageAfterResistance);
    }
}
