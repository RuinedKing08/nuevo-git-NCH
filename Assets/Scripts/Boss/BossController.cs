using UnityEngine;

public class BossController : MonoBehaviour
{
    public int MaxHealth = 200;
    public int CurrentHealth;
    public bool IsPhaseTwo;
    public bool CanDeflectProjectiles = true;
    public bool IsVulnerable;
    public bool HasWeaponStuckInWall;
    public BossPhase PhaseOne;
    public BossPhase PhaseTwo;
    public BossPhase CurrentPhase;
    public PlayerController Player;

    public void Awake()
    {
        CurrentHealth = MaxHealth;
    }

    public void Start()
    {
        PhaseOne = new BossPhaseOne(this);
        PhaseTwo = new BossPhaseTwo(this);
        CurrentPhase = PhaseOne;
        CurrentPhase.Enter();
    }

    public void Update()
    {
        CurrentPhase?.Update();
    }

    public void TakeDamage(int damage)
    {
        if (!IsVulnerable)
        {
            return;
        }
        CurrentPhase?.OnHit(damage);
    }

    public void ApplyDamage(int damage)
    {
        CurrentHealth -= damage;
        CurrentHealth = Mathf.Max(CurrentHealth, 0);
        if (!IsPhaseTwo && CurrentHealth <= MaxHealth / 2)
        {
            SetPhaseTwo();
        }
        if (CurrentHealth == 0)
        {
            Die();
        }
    }

    public void SetPhaseTwo()
    {
        CurrentPhase?.Exit();
        IsPhaseTwo = true;
        IsVulnerable = false;
        HasWeaponStuckInWall = false;
        CurrentPhase = PhaseTwo;
        CurrentPhase.Enter();
        HealPlayerOnPhaseChange();
    }

    public void HealPlayerOnPhaseChange()
    {
        if (Player != null)
        {
            Player.SendMessage("HealMissingHealthPercent", 0.3f, SendMessageOptions.DontRequireReceiver);
        }
    }

    public void DamagePlayer(int damage)
    {
        if (Player == null)
        {
            return;
        }
        Player.SendMessage("TakeDamage", damage, SendMessageOptions.DontRequireReceiver);
    }

    public void TriggerSpecialAttack()
    {
        if (Player == null)
        {
            return;
        }
        Player.SendMessage("TakeDamagePercent", 0.4f, SendMessageOptions.DontRequireReceiver);
        Player.SendMessage("LockOnCombo", null, SendMessageOptions.DontRequireReceiver);
        Player.SendMessage("TryExecuteIfBelow", 20, SendMessageOptions.DontRequireReceiver);
    }

    public void SetVulnerable(bool value)
    {
        IsVulnerable = value;
    }

    public void OnPlayerDeath()
    {
        CurrentPhase?.Enter();
    }

    public void Die()
    {
        CurrentPhase?.Exit();
        Destroy(gameObject, 2f);
    }
}
