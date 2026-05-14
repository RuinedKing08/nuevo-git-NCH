using UnityEngine;
using System.Collections.Generic;

public class PlayerHitCollider : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private Collider _attackCollider;
    [SerializeField] private int _lightDamage = 10;
    [SerializeField] private int _heavyDamage = 20;

    [Header("Solape al activar (temporal)")]
    [Tooltip("Si el hitbox ya está dentro del enemigo al encenderse, OnTriggerEnter no siempre dispara; esto aplica daño a solapes iniciales.")]
    [SerializeField] private bool _probeOverlapsOnAttackStart = true;
    [SerializeField] private int _overlapProbeMax = 16;

    private HashSet<Collider> _hitTargets = new HashSet<Collider>();
    private bool _isAttackActive = false;
    private int _currentDamage;
    private Collider[] _overlapBuffer;

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

        _overlapBuffer = new Collider[_overlapProbeMax];
    }

    
    public void OnAttackStart(int damageType = 0)
    {
        _isAttackActive = true;
        _hitTargets.Clear();

        int presetFromCombat = _currentDamage;
        int fromColliderFields = damageType == 1 ? _heavyDamage : _lightDamage;
        _currentDamage = fromColliderFields > 0 ? fromColliderFields : presetFromCombat;

        if (_attackCollider != null)
        {
            _attackCollider.enabled = true;
            Physics.SyncTransforms();
            if (_probeOverlapsOnAttackStart)
                ProbeOverlappingDamage();
        }

        Debug.Log($"[PlayerAttack] Attack started - Damage: {_currentDamage}");
    }

    /// <summary>
    /// Daño a enemigos que ya solapan el hitbox al encenderlo (OnTriggerEnter no siempre corre en ese caso).
    /// </summary>
    private void ProbeOverlappingDamage()
    {
        if (_attackCollider == null || !_isAttackActive) return;

        Bounds b = _attackCollider.bounds;
        int count = Physics.OverlapBoxNonAlloc(
            b.center,
            b.extents,
            _overlapBuffer,
            Quaternion.identity,
            ~0,
            QueryTriggerInteraction.Collide);

        for (int i = 0; i < count; i++)
        {
            Collider c = _overlapBuffer[i];
            if (c == null || c == _attackCollider) continue;
            TryApplyHit(c);
        }
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

        TryApplyHit(other);
    }

    private void TryApplyHit(Collider other)
    {
        if (!_isAttackActive || _attackCollider == null || other == null)
            return;

        if (_hitTargets.Contains(other))
            return;

        _hitTargets.Add(other);

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
