using UnityEngine;

// ═══════════════════════════════════════════════════════════════════════════
//  INTERFAZ BASE DE ESPECIALIDAD
// ═══════════════════════════════════════════════════════════════════════════
public interface IEnemySpecialty
{
    void OnAssigned(EnemyController e);

    
    void UpdatePassiveBehavior(EnemyController e);

    
    void BeginAttackPhase(EnemyController e);
    void TickAttack(EnemyController e, float elapsed);
    void EndAttackPhase(EnemyController e);

    
    bool PrefersBlock(EnemyController e);

    
    void OnDefeated(EnemyController e);
}

// ═══════════════════════════════════════════════════════════════════════════
//  FACTORY  –  elige especialidad según contexto
// ═══════════════════════════════════════════════════════════════════════════
public static class SpecialtyFactory
{
    public static IEnemySpecialty Resolve(EnemyController e)
    {
        // Force BareKnuckle specialty for now (disable other specialties during debugging)
        //Debug.Log($"SpecialtyFactory.Resolve: forcing BareKnuckleSpecialty for {e.name}");
        return new BareKnuckleSpecialty();
   }
}

 
// ═══════════════════════════════════════════════════════════════════════════
public class BareKnuckleSpecialty : IEnemySpecialty
{
    const int Atk_Consecutive = 0;   
    const int Atk_Heavy       = 1;   
    const int Atk_Quick       = 2;   

    float _attackTimer;
    int   _chosenAttack;
    bool  _attackFired;
    float _prepDuration;

    public void OnAssigned(EnemyController e)
    {
        e.Stats.MaxHealth = 100;
        e.Stats.CurrentHealth = 100;
        e.Stats.Damage = 10;
    }

    public void UpdatePassiveBehavior(EnemyController e)
    {
        e.CombatState.DoOrbit(radius: 3.5f, speedMultiplier: 1.2f);

         if (e.CombatState.GetSubStateName() != "AttackSubState")
         {
              TryPickupWeapon(e);
         }
    }

    public void BeginAttackPhase(EnemyController e)
    {
         _chosenAttack = Atk_Consecutive; // Temporalmente desactivado combo - solo ataque básico
        _attackFired = false;
        _attackTimer = 0;
        
        // DEBUG: forzar impacto inmediato para verificar OverlapSphere
        _prepDuration = 0f;

        //Debug.Log($"BareKnuckleSpecialty.BeginAttackPhase: enemy={e.name} chosen={_chosenAttack} prepDuration={_prepDuration}");
    }

    public void TickAttack(EnemyController e, float elapsed)
    {
        //Debug.Log($"BareKnuckleSpecialty.TickAttack: enemy={e.name} elapsed={elapsed} prep={_prepDuration} attackFired={_attackFired}");
        if (elapsed >= _prepDuration && !_attackFired)
        {
            _attackFired = true;            
            
            if (_chosenAttack == Atk_Heavy) e.Stats.Damage = 20;
            else e.Stats.Damage = 10;

            e.Animator.SetInteger(EnemyController.Hash_AttackIndex, _chosenAttack);
            e.Animator.SetTrigger(EnemyController.Hash_Attack);
            e.ApplyAttackColor();
            
            // Actualizar daño en el collider del ataque
            e.SetAttackDamage(Mathf.Max(1, e.Stats.Damage));
            e.BeginAttackColliderWindow();
        }
    }

    public void EndAttackPhase(EnemyController e)
    {
        e.Stats.Damage = 10;
        _attackFired = false;
        e.EndAttackColor();
    }

    public bool PrefersBlock(EnemyController e) => Random.value > 0.5f;

    public void OnDefeated(EnemyController e)
    {
        
    }

    void TryPickupWeapon(EnemyController e)
    {
        if (e.Detection.HasMeleeWeaponInDangerArea(out var weapon))
        {
            e.Stats.SetSpecialty(new MeleeWeaponSpecialty(weapon));
        }
    }
}

// ═══════════════════════════════════════════════════════════════════════════
//  ESPECIALIDAD: ARMAS BLANCAS
// ═══════════════════════════════════════════════════════════════════════════
public class MeleeWeaponSpecialty : IEnemySpecialty
{
    EnemyMeleeWeapon _weapon;
    bool        _hasUsedAnAttack;   
    float       _scheduledAttackTime;
    bool        _attackFired;
    int         _pendingAttack;

    public MeleeWeaponSpecialty(EnemyMeleeWeapon weapon) => _weapon = weapon;

    public void OnAssigned(EnemyController e)
    {
        e.Stats.MaxHealth = 150;
        e.Stats.Damage    = 20;
        e.Stats.EquipWeapon(_weapon);
        _hasUsedAnAttack = false;
    }

    public void UpdatePassiveBehavior(EnemyController e)
    {
        e.CombatState.DoOrbit(radius: 3.5f, speedMultiplier: 1.2f);        
    }

    public void BeginAttackPhase(EnemyController e)
    {
        
        int max = _hasUsedAnAttack ? 3 : 2;
        int choice = Random.Range(0, max);
        _scheduledAttackTime = Random.Range(0.2f, 4.5f);

        
        _pendingAttack = choice;
        _attackFired   = false;
    }

    public void TickAttack(EnemyController e, float elapsed)
    {
        if (elapsed >= _scheduledAttackTime && !_attackFired)
        {
            _attackFired     = true;
            _hasUsedAnAttack = true;
            e.Animator.SetInteger(EnemyController.Hash_AttackIndex, _pendingAttack);
            e.Animator.SetTrigger(EnemyController.Hash_Attack);  // Trigger de ataque
            e.ApplyAttackColor();

            // Actualizar daño en colliders (20 para arma blanca)
            e.SetAttackDamage(20);
            e.BeginAttackColliderWindow();

            if (_pendingAttack == 2)  // Lanzar arma
            {
                e.Stats.ThrowWeapon(e.Detection.LastKnownPlayerPosition);
                // Perder arma → cambiar a puño limpio con "Priorizar Armas"
                e.Stats.SetSpecialty(new BareKnuckleSpecialty());
            }
        }
    }

    public void EndAttackPhase(EnemyController e)
    {
        _attackFired = false;
        e.EndAttackColor();
    }

    public bool PrefersBlock(EnemyController e) => true;  // siempre bloquea (reduce daño a 0)

    public void OnDefeated(EnemyController e)
    {
        
    }
}

// ═══════════════════════════════════════════════════════════════════════════
//  ESPECIALIDAD: JUEGO SUCIO
// ═══════════════════════════════════════════════════════════════════════════
public class DirtyPlaySpecialty : IEnemySpecialty
{
    EnemyThrowableObject _heldObject;
    bool            _attackFired;
    float           _scheduledAttackTime;

    public DirtyPlaySpecialty(EnemyThrowableObject obj) => _heldObject = obj;

    public void OnAssigned(EnemyController e)
    {
        e.Stats.MaxHealth = 50;
        e.Stats.Damage    = 25;
        e.Stats.EquipThrowable(_heldObject);
    }

    public void UpdatePassiveBehavior(EnemyController e)
    {
        e.CombatState.DoOrbit(radius: 3.5f, speedMultiplier: 1.2f);
    }

    public void BeginAttackPhase(EnemyController e)
    {
        _scheduledAttackTime = Random.Range(0.2f, 4.5f);
        _attackFired         = false;
    }

    public void TickAttack(EnemyController e, float elapsed)
    {
        if (elapsed >= _scheduledAttackTime && !_attackFired)
        {
            _attackFired = true;
            e.Animator.SetInteger(EnemyController.Hash_AttackIndex, 0);  // Lanzar objeto
            e.Animator.SetTrigger(EnemyController.Hash_Attack);  // Trigger de ataque
            e.ApplyAttackColor();
            e.BeginAttackColliderWindow();
        }
    }

    System.Collections.IEnumerator RetentionCooldown(EnemyController e)
    {
        yield return new WaitForSeconds(2f);
        e.Stats.ReloadThrowable();
    }

    public void EndAttackPhase(EnemyController e)
    {
        _attackFired = false;
        e.EndAttackColor();
    }

    public bool PrefersBlock(EnemyController e) => false; 
    public void OnDefeated(EnemyController e)
    {
        e.Stats.DropThrowable();   
    }
}

    

