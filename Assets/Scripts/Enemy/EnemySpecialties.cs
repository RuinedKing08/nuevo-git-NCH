using UnityEngine;

// ═══════════════════════════════════════════════════════════════════════════
//  INTERFAZ BASE DE ESPECIALIDAD
// ═══════════════════════════════════════════════════════════════════════════
public interface IEnemySpecialty
{
    void OnAssigned(EnemyController e);

    // Comportamiento pasivo en combate (rodear, mantener distancia, etc.)
    void UpdatePassiveBehavior(EnemyController e);

    // Fases de ataque
    void BeginAttackPhase(EnemyController e);
    void TickAttack(EnemyController e, float elapsed);
    void EndAttackPhase(EnemyController e);

    // Defensa
    bool PrefersBlock(EnemyController e);

    // Al ser derrotado
    void OnDefeated(EnemyController e);
}

// ═══════════════════════════════════════════════════════════════════════════
//  FACTORY  –  elige especialidad según contexto
// ═══════════════════════════════════════════════════════════════════════════
public static class SpecialtyFactory
{
    public static IEnemySpecialty Resolve(EnemyController e)
    {
        var detection = e.Detection;
        
        // Prioridad: Juego Sucio > Arma Blanca > Puño Limpio
        if (detection.HasThrowableObjectInDangerArea(out var throwable) &&
            detection.IsCloserThanPlayer(throwable.transform.position))
            return new DirtyPlaySpecialty(throwable);

        if (detection.HasMeleeWeaponInDangerArea(out var weapon) &&
            detection.IsCloserThanPlayer(weapon.transform.position))
            return new MeleeWeaponSpecialty(weapon);

        return new BareKnuckleSpecialty();
   }
}

 
// ═══════════════════════════════════════════════════════════════════════════
public class BareKnuckleSpecialty : IEnemySpecialty
{
    // Ataques disponibles (índices para el Animator)
    const int Atk_Consecutive = 0;   // Golpes consecutivos (x3)
    const int Atk_Heavy       = 1;   // Golpe fuerte
    const int Atk_Quick       = 2;   // Golpe rápido

    float _scheduledAttackTime;
    int   _chosenAttack;
    bool  _attackFired;

    public void OnAssigned(EnemyController e)
    {
        e.Stats.MaxHealth = 100;
        e.Stats.Damage    = 10;
        // Prioridad 10: atacar a matar (no busca objetos)
    }

    public void UpdatePassiveBehavior(EnemyController e)
    {
        // Rodear al jugador cuando no está atacando
        Vector3 playerPos  = e.Detection.LastKnownPlayerPosition;
        Vector3 orbitPoint = playerPos + Quaternion.Euler(0, Time.time * 60f, 0) * Vector3.forward * 2f;
        if (e.Stats.NavAgent && e.Stats.NavAgent.isOnNavMesh)
            e.Stats.NavAgent.SetDestination(orbitPoint);
    }

    public void BeginAttackPhase(EnemyController e)
    {
        _chosenAttack        = Random.Range(0, 3);
        _scheduledAttackTime = Random.Range(0.2f, 4.5f);  // dentro de la ventana de 5s
        _attackFired         = false;
    }

    public void TickAttack(EnemyController e, float elapsed)
    {
        if (elapsed >= _scheduledAttackTime && !_attackFired)
        {
            _attackFired = true;
            e.Animator.SetInteger(EnemyController.Hash_AttackIndex, _chosenAttack);
            e.Animator.SetTrigger(EnemyController.Hash_Attack);  // Trigger de ataque

            // Intentar equiparse si ve un arma cerca y no está atacando
            TryPickupWeapon(e);
        }
    }

    public void EndAttackPhase(EnemyController e) => _attackFired = false;

    public bool PrefersBlock(EnemyController e) => Random.value > 0.5f;

    public void OnDefeated(EnemyController e)
    {
        
    }

    void TryPickupWeapon(EnemyController e)
    {
        if (e.Detection.HasMeleeWeaponInDangerArea(out var weapon))
            e.Stats.SetSpecialty(new MeleeWeaponSpecialty(weapon));
        else if (e.Detection.HasThrowableObjectInDangerArea(out var throwable))
            e.Stats.SetSpecialty(new DirtyPlaySpecialty(throwable));
    }
}

// ═══════════════════════════════════════════════════════════════════════════
//  ESPECIALIDAD: ARMAS BLANCAS
// ═══════════════════════════════════════════════════════════════════════════
public class MeleeWeaponSpecialty : IEnemySpecialty
{
    EnemyMeleeWeapon _weapon;
    bool        _hasUsedAnAttack;   // condición para poder lanzar el arma
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
        // Mantener distancia media (no rodea como puño limpio)
        Vector3 playerPos = e.Detection.LastKnownPlayerPosition;
        Vector3 dir       = (e.transform.position - playerPos).normalized;
        
        if (e.Stats.NavAgent && e.Stats.NavAgent.isOnNavMesh)
            e.Stats.NavAgent.SetDestination(playerPos + dir * 1.5f);

        // Sin arma → buscarla o defender
        if (!e.Stats.HasWeapon)
        {
            if (e.Detection.HasMeleeWeaponInDangerArea(out var newWeapon))
            {
                if (e.Stats.NavAgent && e.Stats.NavAgent.isOnNavMesh)
                    e.Stats.NavAgent.SetDestination(newWeapon.transform.position);
                    
                if (Vector3.Distance(e.transform.position, newWeapon.transform.position) < 0.5f)
                    e.Stats.EquipWeapon(newWeapon);
            }
            // Si no encuentra, simplemente defiende (manejado por el grupo)
        }
    }

    public void BeginAttackPhase(EnemyController e)
    {
        // Elegir ataque: cargado, débil, o lanzar (si ya atacó antes)
        int max = _hasUsedAnAttack ? 3 : 2;
        int choice = Random.Range(0, max);
        _scheduledAttackTime = Random.Range(0.2f, 4.5f);

        // Guardar elección para TickAttack
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

            if (_pendingAttack == 2)  // Lanzar arma
            {
                e.Stats.ThrowWeapon(e.Detection.LastKnownPlayerPosition);
                // Perder arma → cambiar a puño limpio con "Priorizar Armas"
                e.Stats.SetSpecialty(new BareKnuckleSpecialty());
            }
        }
    }

    public void EndAttackPhase(EnemyController e) => _attackFired = false;

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
        // Mantener distancia máxima del área de detección
        Vector3 playerPos   = e.Detection.LastKnownPlayerPosition;
        float   maxDistance = e.Detection.InterestAreaRadius * 0.9f;
        Vector3 idealPos    = playerPos + (e.transform.position - playerPos).normalized * maxDistance;
        
        if (e.Stats.NavAgent && e.Stats.NavAgent.isOnNavMesh)
            e.Stats.NavAgent.SetDestination(idealPos);

        // Sin objeto → buscar uno
        if (!e.Stats.HasThrowable)
        {
            if (e.Detection.HasThrowableObjectInDangerArea(out var newObj))
            {
                if (e.Stats.NavAgent && e.Stats.NavAgent.isOnNavMesh)
                    e.Stats.NavAgent.SetDestination(newObj.transform.position);
                    
                if (Vector3.Distance(e.transform.position, newObj.transform.position) < 0.5f)
                    e.Stats.EquipThrowable(newObj);
            }
        }
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

            // Lanzar → 2 segundos de recuperación (retención de objeto): sacar otro igual
            e.Stats.ThrowObject(e.Detection.LastKnownPlayerPosition);
            e.StartCoroutine(RetentionCooldown(e));
        }
    }

    System.Collections.IEnumerator RetentionCooldown(EnemyController e)
    {
        yield return new WaitForSeconds(2f);
        // Sacar objeto equivalente (game logic del inventario)
        e.Stats.ReloadThrowable();
    }

    public void EndAttackPhase(EnemyController e) => _attackFired = false;

    public bool PrefersBlock(EnemyController e) => false;  // siempre prefiere alejarse

    public void OnDefeated(EnemyController e)
    {
        e.Stats.DropThrowable();   // el jugador puede recogerlo
    }
}

