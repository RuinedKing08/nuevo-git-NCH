using UnityEngine;

public abstract class HealthController : MonoBehaviour
{
    [SerializeField] protected int health;
    [SerializeField] protected int maxHealth;
    protected virtual void Start()
    {
        SetLife();
    }

    public void ApplyDamage(int damage)
    {
        GetDamage(damage);
    }
    protected abstract void SetLife();
    protected abstract void GetLife(int heal);
    protected abstract void GetMaxLife(int extraHealth);
    protected abstract void GetDamage(int damage);
}
