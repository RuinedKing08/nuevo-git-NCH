using UnityEngine;

public abstract class HealthController : MonoBehaviour
{
    [SerializeField] protected float health;
    [SerializeField] protected float maxHealth;
    protected virtual void Start()
    {
        SetLife();
    }

    public void ApplyDamage(int damage)
    {
        GetDamage(damage);
    }
    protected abstract void SetLife();
    public abstract void GetLife(float heal);
    public abstract void GetMaxLife(float extraHealth);
    protected abstract void GetDamage(int damage);
}
