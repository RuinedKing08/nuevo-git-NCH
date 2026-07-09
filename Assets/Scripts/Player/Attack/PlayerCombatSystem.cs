using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerCombatSystem : MonoBehaviour
{
   [SerializeField] private Animator _animator;
   [SerializeField] private PlayerHitCollider _hitCollider;

    public AudioData HitSound;
    public AudioData AirSound;    
    public AudioData DodgeSound;
    public AudioData DamageSound;
    
    private int _comboIndex = 0;
    private bool _isAttacking = false;
    private float _lastAttackTime;
    private float _resetTime = 1.2f;

    public void PerformLightAttack()
    {
        if (_isAttacking || PlayerActions.blocking) return;

        _isAttacking = true;
        _lastAttackTime = Time.time;

        _animator.SetInteger("ComboIndex", _comboIndex);
        _animator.SetTrigger("Attack");
    }

    private void Update()
    {
        if (!_isAttacking && Time.time - _lastAttackTime > _resetTime)
            _comboIndex = 0;
    }

    // EVENTOS DE ANIMACIÓN
    public void AE_EnableHitbox() => _hitCollider.OnAttackStart();
    public void AE_DisableHitbox() => _hitCollider.OnAttackEnd();
    public void AE_AttackEnd()
    {
        _isAttacking = false;
        _comboIndex = (_comboIndex + 1) % 3; 
    }

    public void SetBlockingState(bool state) => _animator.SetBool("Blocking", state);
    public void PlayBlockHit() => _animator.SetTrigger("BlockHit");
    public void PlayTakeHit() => _animator.SetTrigger("Hit");
    public void PlayDodge() => _animator.SetTrigger("Dodge");
    public void PlayPush() => _animator.SetTrigger("Push");
    
}
