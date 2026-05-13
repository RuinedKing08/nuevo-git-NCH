using UnityEngine;
using System.Collections.Generic;


public class AttackColliderHandler : MonoBehaviour
{
    [SerializeField] private Collider _attackCollider; 
    [SerializeField] private int _damageAmount = 10;
    
    private EnemyController _enemyController;
    private HashSet<Collider> _hitTargets = new HashSet<Collider>(); 
    private bool _isAttackActive = false;

    private void Awake()
    {
        _enemyController = GetComponentInParent<EnemyController>();
        
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

    
    public void OnAttackStart()
    {
        _isAttackActive = true;
        _hitTargets.Clear();
        
        if (_attackCollider != null)
        {
            _attackCollider.enabled = true;
        }

        Debug.Log($"[AttackCollider] Attack started on {gameObject.name}");
    }

    
    public void OnAttackEnd()
    {
        _isAttackActive = false;
        
        if (_attackCollider != null)
        {
            _attackCollider.enabled = false;
        }

        Debug.Log($"[AttackCollider] Attack ended on {gameObject.name}");
    }

    
    private void OnTriggerEnter(Collider other)
    {
        if (!_isAttackActive || _attackCollider == null)
            return;

        
        if (_hitTargets.Contains(other))
            return;

        _hitTargets.Add(other);

        
        var playerHealth = other.GetComponentInParent<PlayerHealth>();
        if (playerHealth != null)
        {
            //Debug.Log($"[AttackCollider] Hit PlayerHealth! Damage: {_damageAmount}");
            playerHealth.TakeDamage(_damageAmount);
            
            var playerController = other.GetComponentInParent<PlayerController>();
            if (playerController != null)
            {
                playerController.SendMessage("OnHit", transform.position, SendMessageOptions.DontRequireReceiver);
            }
            return;
        }
        
        var pc = other.GetComponentInParent<PlayerController>();
        if (pc != null)
        {
            //Debug.Log($"[AttackCollider] Hit PlayerController! Damage: {_damageAmount}");
            pc.gameObject.SendMessage("TakeDamage", _damageAmount, SendMessageOptions.DontRequireReceiver);
            pc.gameObject.SendMessage("OnHit", transform.position, SendMessageOptions.DontRequireReceiver);
        }
    }

    
    public void SetDamage(int amount)
    {
        _damageAmount = amount;
    }

    public void ResetHitTargets()
    {
        _hitTargets.Clear();
    }
}
