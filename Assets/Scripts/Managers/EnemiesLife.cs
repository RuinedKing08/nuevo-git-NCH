using UnityEngine;

public class EnemiesLife : MonoBehaviour
{
    EnemyController _enemyController;
    bool _damageEnter;

    void Awake()
    {
        _enemyController = GetComponent<EnemyController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DamageToEnemies"))
        {
           
                if (_enemyController != null)
                {
                    Vector3 hitDir = (_enemyController.transform.position - other.transform.position).normalized;
                    _enemyController.TakeDamage(10, hitDir);
                }
                _damageEnter = true;
            
        }   
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("DamageToEnemies"))
        {
            _damageEnter = false;
        }
    }
}
