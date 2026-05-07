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
        var detection = e.Detection;        
        
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

        Debug.Log($"Elegí ataque {_chosenAttack} para {e.name}");

        _prepDuration = _chosenAttack switch
        {
            Atk_Heavy => 1.0f,
            Atk_Quick => 0.5f,
            _ => 0.2f
        };
    }

    public void TickAttack(EnemyController e, float elapsed)
    {
        if (elapsed >= _prepDuration && !_attackFired)
        {
            _attackFired = true;            
            
            if (_chosenAttack == Atk_Heavy) e.Stats.Damage = 20;
            else e.Stats.Damage = 10;

            e.Animator.SetInteger(EnemyController.Hash_AttackIndex, _chosenAttack);
            e.Animator.SetTrigger(EnemyController.Hash_Attack);
            e.ApplyAttackColor();
            Vector3 center = e.transform.position + e.transform.forward * 1.25f + Vector3.up * 0.9f;
            float radius = 1.0f;
            float duration = 0.25f;
            EnemyHitbox.Create(e, center, radius, Mathf.Max(1, e.Stats.Damage), duration);
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

// ═══════════════════════════════════════════════════════════════════════════
//  Helper: Hitbox temporal y visible para depuración / impacto
// ═══════════════════════════════════════════════════════════════════════════
public class EnemyHitbox : MonoBehaviour
{
    EnemyController _owner;
    int _damage;
    float _expiryTime;
    Material _mat;

    public static void Create(EnemyController owner, Vector3 center, float radius, int damage, float duration)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        go.name = owner.name + "_Hitbox";
        go.transform.position = center;
        go.transform.localScale = Vector3.one * radius * 2f;
        var col = go.GetComponent<SphereCollider>();
        col.isTrigger = true;
        go.layer = LayerMask.NameToLayer("Ignore Raycast");
        var hb = go.AddComponent<EnemyHitbox>();
        hb._owner = owner;
        hb._damage = damage;
        hb._expiryTime = Time.time + duration;
        var rend = go.GetComponent<MeshRenderer>();
        if (rend != null)
        {
            hb._mat = new Material(Shader.Find("Standard"));
            hb._mat.color = new Color(1f, 0f, 0f, 0.35f);
            hb._mat.SetFloat("_Mode", 3);
            hb._mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            hb._mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            hb._mat.SetInt("_ZWrite", 0);
            hb._mat.DisableKeyword("_ALPHATEST_ON");
            hb._mat.EnableKeyword("_ALPHABLEND_ON");
            hb._mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            hb._mat.renderQueue = 3000;
            rend.material = hb._mat;
        }
        var rb = go.AddComponent<Rigidbody>();
        rb.isKinematic = true;
    }

    void Update()
    {
        if (Time.time >= _expiryTime)
        {
            if (_mat != null)
                Destroy(_mat);
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.SendMessage("TakeDamage", _damage, SendMessageOptions.DontRequireReceiver);
            other.SendMessage("OnHit", _owner.transform.position, SendMessageOptions.DontRequireReceiver);
            if (_mat != null) Destroy(_mat);
            Destroy(gameObject);
            return;
        }
        var pc = other.GetComponentInParent<PlayerController>();
        if (pc != null)
        {
            other.SendMessage("TakeDamage", _damage, SendMessageOptions.DontRequireReceiver);
            if (_mat != null) Destroy(_mat);
            Destroy(gameObject);
            return;
        }
    }

    void OnDestroy()
    {
        if (_mat != null)
            Destroy(_mat);
    }
}

