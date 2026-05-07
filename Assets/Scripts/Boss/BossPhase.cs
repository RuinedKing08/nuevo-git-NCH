using UnityEngine;

public abstract class BossPhase
{
    protected BossController Boss;
    protected BossPhase(BossController boss)
    {
        Boss = boss;
    }

    public abstract void Enter();
    public abstract void Update();
    public abstract void Exit();
    public abstract void OnHit(int damage);
}
