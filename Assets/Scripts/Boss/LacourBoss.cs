using UnityEngine;

public class LacourBoss : BossController
{
    [Header("Lacour Specific")]
    [SerializeField] private float attackCooldown = 2f;
    private float _attackTimer = 0f;

    private void Start()
    {
        
    }

    private void Update()
    {
        base.Update();
        
        
        UpdateAttackCooldown();
    }

    private void UpdateAttackCooldown()
    {
        if (_attackTimer > 0)
        {
            _attackTimer -= Time.deltaTime;
        }
    }

    
    public void ExecuteFollowUpAttack()
    {
        if (_attackTimer <= 0)
        {
            //Debug.Log($"[BOSS] {name} ejecutando ataque de seguimiento");  
            _attackTimer = attackCooldown;
        }
    }
}

