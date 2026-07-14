using UnityEngine;
using System.Collections.Generic;

public class PlayerHitCollider : MonoBehaviour
{
    [SerializeField] private Collider _attackColliderRight;
    [SerializeField] private Collider _attackColliderLeft;
    private HashSet<Collider> _hitTargets = new HashSet<Collider>();
    private bool _isAttackActive = false;
    private int _currentDamage = 10;

    public PlayerCombatSystem _combatsys;

    public static PlayerHitCollider Instance;
    private void Awake()
    {
        Instance = this;
        
        // Si no está asignado, intentar obtener de este GameObject
        if (_attackColliderRight == null) _attackColliderRight = GetComponent<Collider>();
        
        // Configurar ambos colliders
        if (_attackColliderRight != null)
        {
            _attackColliderRight.isTrigger = true;
            _attackColliderRight.enabled = false;
        }
        if (_attackColliderLeft != null)
        {
            _attackColliderLeft.isTrigger = true;
            _attackColliderLeft.enabled = false;
        }
    }

    public void OnAttackStart(int type = 0)
    {
        _isAttackActive = true;
        _hitTargets.Clear();
        if (_attackColliderRight != null) _attackColliderRight.enabled = true;
        if (_attackColliderLeft != null) _attackColliderLeft.enabled = true;
        AudioManager.Instance.Play(_combatsys.AirSound, transform.position);
    }

    public void OnAttackEnd()
    {
        _isAttackActive = false;
        if (_attackColliderRight != null) _attackColliderRight.enabled = false;
        if (_attackColliderLeft != null) _attackColliderLeft.enabled = false;
    }

    public void SetDamage(int damage) => _currentDamage = damage;

    
    public int GetDamage() => _currentDamage;
    
    public void ModifyDamage(int amount)
    {
        _currentDamage += amount;
        _currentDamage = Mathf.Max(0, _currentDamage);
    }
   
    public void SetDamageMultiplier(float multiplier)
    {
        _currentDamage = Mathf.Max(1, Mathf.RoundToInt(_currentDamage * multiplier));
    }
    float a, b, c; 

    private void OnTriggerEnter(Collider other)
    {
        if (!_isAttackActive || _hitTargets.Contains(other)) return;
            
        
        if (other == null || _hitTargets.Contains(other)) return;
        if (other.gameObject.CompareTag("EnemyDetect"))
        {
            /*a++;
            b++;
            Debug.Log(a);
            if (a >= 2)
            {
                a = 0;
                return;
            }
            if (b >= 3)
            {
                a = 0;
                b = 0;
                return;
            }*/

            var enemy = other.GetComponentInParent<EnemyController>();
            var boss = other.GetComponentInParent<BossController>();
            if (enemy != null)
            {
                Debug.Log($"Hit enemy: {enemy.name} with damage: {_currentDamage}");
                _hitTargets.Add(other);
                Vector3 hitDir = (other.transform.position - transform.position).normalized;
                AudioManager.Instance.Play(_combatsys.HitSound, transform.position);
                enemy.TakeDamage(_currentDamage, hitDir);
                DJVFX.Instance.PlayHit1();
            }
            else if (boss != null)
            {
                _hitTargets.Add(other);
                Vector3 hitDir = (other.transform.position - transform.position).normalized;
                AudioManager.Instance.Play(_combatsys.HitSound, transform.position);
                boss.TakeDamage(_currentDamage);
                DJVFX.Instance.PlayHit1();
            }
        }
        
    }
    
}
