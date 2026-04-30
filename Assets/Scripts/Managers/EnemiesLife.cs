using UnityEngine;

public class EnemiesLife : HealthController
{
    protected override void Start()
    {
        base.Start();
    }
    protected override void SetLife()
    {
        health = maxHealth;
    }
    protected override void GetMaxLife(int extraHealth)
    {
        maxHealth += extraHealth;
    }
    protected override void GetLife(int heal)
    {
        health += heal;
    }
    protected override void GetDamage(int damage)
    {
        health -= damage;
        if(health <= 0)
        {
            //gameObject.SetActive(false);
            Destroy(gameObject, 2f);
        }
    }
    bool damageEnter;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DamageToEnemies"))
        {
            if (!damageEnter)
            {
                GetDamage(2);
                damageEnter = true;
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("DamageToEnemies"))
        {
            damageEnter = false;
        }
    }
}
