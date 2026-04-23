using UnityEngine;
using System.Collections;

public class MeleeWeapon : EquipableItemBase
{
    [Header("Settings")]
    public WeaponSettings settings;

    [Header("Attack")]
    public float attackRadius = 1.2f;
    public float attackDelay = 0.25f;      // tiempo hasta el impacto (seg)
    public float attackCooldown = 0.6f;    // duración total de la acción
    public LayerMask enemyLayerMask;       // asignar la layer "Enemy" en inspector

    bool _isBusy;
    Transform _weaponBone;
    PlayerController _playerController;
    Animator _playerAnimator;

    public override void OnEquip(PlayerActionController controller)
    {
        base.OnEquip(controller);
        _weaponBone = controller.weaponBone;
        _playerController = controller.GetComponentInParent<PlayerController>();
        if (_playerController != null)
            _playerAnimator = _playerController.GetComponentInChildren<Animator>();
    }

    public override void ProcessInput(ActionInput input)
    {
        if (input.Attack && !_isBusy)
        {
            controller.SetActionState(ActionState.ATTACK);
            controller.StartCoroutine(PerformAttack());
        }
    }

    protected virtual IEnumerator PerformAttack()
    {
        _isBusy = true;

        // Trigger de animación (asegúrate de que tu Animator tenga un trigger "Attack")
        if (_playerAnimator != null)
            _playerAnimator.SetTrigger("Attack");

        // Esperar hasta el frame de impacto
        yield return new WaitForSeconds(attackDelay);

        // Determinar centro del ataque (frente del arma)
        Vector3 center;
        if (_weaponBone != null)
            center = _weaponBone.position + _weaponBone.forward * (attackRadius * 0.5f);
        else
            center = controller.transform.position;

        // Detectar enemigos y aplicar daño
        var cols = Physics.OverlapSphere(center, attackRadius, enemyLayerMask);
        int dmg = Mathf.Max(1, Mathf.RoundToInt(settings != null ? settings.Damage : 10f));
        foreach (var c in cols)
        {
            var enemy = c.GetComponentInParent<EnemyController>();
            if (enemy != null)
            {
                Vector3 hitDir = (enemy.transform.position - center).normalized;
                enemy.TakeDamage(dmg, hitDir);
            }
        }

        // Esperar cooldown restante
        yield return new WaitForSeconds(Mathf.Max(0f, attackCooldown - attackDelay));

        _isBusy = false;
        controller.SetActionState(ActionState.NONE);
    }

    public override bool IsBusy() => _isBusy;

    // (Opcional) dibujar gizmo para depurar alcance del golpe
    void OnDrawGizmosSelected()
    {
        if (_weaponBone != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(_weaponBone.position + _weaponBone.forward * (attackRadius * 0.5f), attackRadius);
        }
    }
}

