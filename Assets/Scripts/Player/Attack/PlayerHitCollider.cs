using UnityEngine;
using System.Collections.Generic;

public class PlayerHitCollider : MonoBehaviour
{
     [SerializeField] private Collider _attackCollider;
    private HashSet<Collider> _hitTargets = new HashSet<Collider>();
    private bool _isAttackActive = false;
    private int _currentDamage;

    private void Awake()
    {
        if (_attackCollider == null) _attackCollider = GetComponent<Collider>();
        if (_attackCollider != null)
        {
            _attackCollider.isTrigger = true;
            _attackCollider.enabled = false;
        }
    }

    public void OnAttackStart(int type = 0)
    {
        _isAttackActive = true;
        _hitTargets.Clear();
        if (_attackCollider != null) _attackCollider.enabled = true;
    }

    public void OnAttackEnd()
    {
        _isAttackActive = false;
        if (_attackCollider != null) _attackCollider.enabled = false;
    }

    public void SetDamage(int damage) => _currentDamage = damage;

    private void OnTriggerEnter(Collider other)
    {
        if (!_isAttackActive || _hitTargets.Contains(other)) return;

        var enemy = other.GetComponentInParent<EnemyController>();
        if (enemy != null)
        {
            _hitTargets.Add(other);
            Vector3 hitDir = (other.transform.position - transform.position).normalized;
            enemy.TakeDamage(_currentDamage, hitDir);
        }
    }
}
