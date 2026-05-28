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
        return new BareKnuckleSpecialty();
    }
}

 
// ═══════════════════════════════════════════════════════════════════════════
public class BareKnuckleSpecialty : IEnemySpecialty
{
    private int _chosenAttack;
    private bool _attackFired;
    private float _prepDuration = 0.5f;

    public void OnAssigned(EnemyController e) { e.Stats.Damage = 10; }

    public void UpdatePassiveBehavior(EnemyController e) { }

    public void BeginAttackPhase(EnemyController e)
    {
        _chosenAttack = UnityEngine.Random.Range(0, 2); 
        _attackFired = false;        
        _prepDuration = UnityEngine.Random.Range(0.2f, 0.4f); 
    }

    public void TickAttack(EnemyController e, float elapsed)
    {
        if (Mathf.Repeat(elapsed, 0.5f) < Time.deltaTime)
        {
            Debug.Log($"[ESPECIALIDAD] {e.name} esperando... Transcurrido: {elapsed:F1} / Prep: {_prepDuration:F1}. Atacado: {_attackFired}");
        }

        if (elapsed >= _prepDuration && !_attackFired)
        {
            _attackFired = true;        

            Debug.Log($"[TRIGGER] {e.name} MANDANDO TRIGGER 'Attack' al Animator con Index: {_chosenAttack}");    
            
            e.Animator.ResetTrigger(EnemyController.Hash_Attack);
            e.Animator.SetInteger(EnemyController.Hash_AttackIndex, _chosenAttack);
            e.Animator.SetTrigger(EnemyController.Hash_Attack);
            
            e.SetAttackDamage(e.Stats.Damage);
        }
    }

    public void EndAttackPhase(EnemyController e) => _attackFired = false;
    public bool PrefersBlock(EnemyController e) => UnityEngine.Random.value > 0.5f;
    public void OnDefeated(EnemyController e) { }
}

// ═══════════════════════════════════════════════════════════════════════════
//  ESPECIALIDAD: ARMAS BLANCAS
// ═══════════════════════════════════════════════════════════════════════════
public class MeleeWeaponSpecialty : IEnemySpecialty
{
   EnemyMeleeWeapon _weapon;
    public MeleeWeaponSpecialty(EnemyMeleeWeapon weapon) => _weapon = weapon;

    public void OnAssigned(EnemyController e) { e.Stats.EquipWeapon(_weapon); e.Stats.Damage = 20; }
    public void UpdatePassiveBehavior(EnemyController e) { }
    public void BeginAttackPhase(EnemyController e) { }
    public void TickAttack(EnemyController e, float elapsed)
    {
        if (elapsed >= 0.5f)
        {
            e.Animator.SetInteger(EnemyController.Hash_AttackIndex, 1);
            e.Animator.SetTrigger(EnemyController.Hash_Attack);
            e.SetAttackDamage(20);
            e.BeginAttackColliderWindow(0.5f);
        }
    }
    public void EndAttackPhase(EnemyController e) { }
    public bool PrefersBlock(EnemyController e) => true;
    public void OnDefeated(EnemyController e) { }
}

// ═══════════════════════════════════════════════════════════════════════════
//  ESPECIALIDAD: JUEGO SUCIO
/* ═══════════════════════════════════════════════════════════════════════════
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
*/
    

