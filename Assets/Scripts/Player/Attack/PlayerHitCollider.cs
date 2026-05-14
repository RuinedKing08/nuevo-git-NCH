using UnityEngine;
using System.Collections.Generic;

public class PlayerHitCollider : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private Collider _attackCollider;
    [SerializeField] private int _lightDamage = 10;
    [SerializeField] private int _heavyDamage = 20;
    
    private HashSet<Collider> _hitTargets = new HashSet<Collider>();
    private bool _isAttackActive = false;
    private int _currentDamage;

    private void Awake()
    {
        if (_attackCollider == null)
        {
            _attackCollider = GetComponent<Collider>();
        }

        if (_attackCollider != null)
        {
            _attackCollider.isTrigger = true;
            _attackCollider.enabled = false;
        }
    }

    
    public void OnAttackStart(int damageType = 0)
    {
        _isAttackActive = true;
        _hitTargets.Clear();

        
        _currentDamage = damageType == 1 ? _heavyDamage : _lightDamage;

        if (_attackCollider != null)
        {
            _attackCollider.enabled = true;
        }

        Debug.Log($"[PlayerAttack] Attack started - Damage: {_currentDamage}");
    }

    
    public void OnAttackEnd()
    {
        _isAttackActive = false;

        if (_attackCollider != null)
        {
            _attackCollider.enabled = false;
        }

        Debug.Log($"[PlayerAttack] Attack ended");
    }

    /// <summary>
    /// Cambia el daño actual (útil para diferentes tipos de ataques).
    /// </summary>
    public void SetDamage(int damage)
    {
        _currentDamage = damage;
    }

    /// <summary>
    /// Detecta colisiones y aplica daño a enemigos.
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        if (!_isAttackActive || _attackCollider == null)
            return;

        // Evita hits múltiples al mismo enemigo en un ataque
        if (_hitTargets.Contains(other))
            return;

        _hitTargets.Add(other);

        // Busca EnemyHealth o EnemyStats en el objeto tocado
        var enemyController = other.GetComponentInParent<EnemyController>();
        if (enemyController != null)
        {
            Vector3 hitDirection = (other.transform.position - transform.position).normalized;
            enemyController.TakeDamage(_currentDamage, hitDirection);
            
            Debug.Log($"[PlayerAttack] Hit enemy {enemyController.name}! Damage: {_currentDamage}");
        }
    }

    /// <summary>
    /// Resetea los hits para el siguiente ataque.
    /// </summary>
    public void ResetHitTargets()
    {
        _hitTargets.Clear();
    }

    public void SetLightDamage(int damage) => _lightDamage = damage;
    public void SetHeavyDamage(int damage) => _heavyDamage = damage;
}
