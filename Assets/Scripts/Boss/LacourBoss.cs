/*using UnityEngine;

public class LacourBoss : BossController
{
}

public class BossPhaseOne : BossPhase
{
    private float attackTimer;
    private const float FastThrustInterval = 1.2f;
    private const float StrongThrustInterval = 3f;
    private bool useStrongThrust;

    public BossPhaseOne(BossController boss) : base(boss)
    {
    }

    public override void Enter()
    {
        attackTimer = 0f;
        useStrongThrust = false;
        Boss.SetVulnerable(false);
        Boss.HasWeaponStuckInWall = false;
    }

    public override void Update()
    {
        attackTimer += Time.deltaTime;
        if (attackTimer >= StrongThrustInterval)
        {
            attackTimer = 0f;
            useStrongThrust = !useStrongThrust;
            if (useStrongThrust)
            {
                Boss.DamagePlayer(25);
                Boss.HasWeaponStuckInWall = true;
                Boss.SetVulnerable(true);
            }
            else
            {
                Boss.DamagePlayer(25);
            }
        }
    }

    public override void Exit()
    {
        Boss.SetVulnerable(false);
        Boss.HasWeaponStuckInWall = false;
    }

    public override void OnHit(int damage)
    {
        if (Boss.IsVulnerable)
        {
            Boss.ApplyDamage(damage);
            Boss.SetVulnerable(false);
            Boss.HasWeaponStuckInWall = false;
        }
    }
}

public class BossPhaseTwo : BossPhase
{
    private float attackTimer;
    private const float CommonAttackInterval = 1f;
    private const float SpecialAttackInterval = 6f;
    private bool specialReady;

    public BossPhaseTwo(BossController boss) : base(boss)
    {
    }

    public override void Enter()
    {
        attackTimer = 0f;
        specialReady = true;
        Boss.SetVulnerable(true);
    }

    public override void Update()
    {
        attackTimer += Time.deltaTime;
        if (attackTimer >= SpecialAttackInterval && specialReady)
        {
            attackTimer = 0f;
            specialReady = false;
            Boss.TriggerSpecialAttack();
            return;
        }
        if (attackTimer >= CommonAttackInterval)
        {
            attackTimer = 0f;
            Boss.DamagePlayer(15);
        }
    }

    public override void Exit()
    {
    }

    public override void OnHit(int damage)
    {
        Boss.ApplyDamage(damage);
    }
}

*/
