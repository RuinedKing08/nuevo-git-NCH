using UnityEngine;
using System.Collections.Generic;

public class AttackColliderHandler : MonoBehaviour
{
    [SerializeField] private Collider _attackCollider;
    [SerializeField] private int _damageAmount = 10;

    [Header("Solape al activar")]
    [Tooltip("Si el hitbox ya solapa al jugador al encenderse, OnTriggerEnter puede no dispararse.")]
    [SerializeField] private bool _probeOverlapsOnAttackStart = true;
    [SerializeField] private int _overlapProbeMax = 16;
    [Tooltip("Infla la caja de consulta para compensar desfases sin geometrías solapadas.")]
    [SerializeField] private float _probeBoundsPadding = 0.2f;
    [Tooltip("Si la caja no encuentra nada, prueba una esfera hacia el frente del hit (solo capa Player).")]
    [SerializeField] private bool _fallbackSphereTowardForward = true;
    [SerializeField] private float _fallbackSphereRadius = 1.25f;
    [SerializeField] private float _fallbackSphereForwardOffset = 0.35f;

    [Tooltip("Si la sonda solo en capa Player no encuentra nada, repite incluyendo Default (útil si el jugador sigue en capa Default).")]
    [SerializeField] private bool _probeFallbackIncludeDefaultLayer = true;

    [Header("Debug daño → jugador")]
    [SerializeField] private bool _debugDamage = true;
    [Tooltip("Si está off, no se loguean contactos con suelo/cubos sin PlayerHealth (menos spam).")]
    [SerializeField] private bool _logNonPlayerContacts = false;

    private EnemyController _enemyController;
    private HashSet<Collider> _hitTargets = new HashSet<Collider>();
    private bool _isAttackActive = false;
    private Collider[] _overlapBuffer;
    [SerializeField] GameObject attackShadow;
    private void Awake()
    {
        _enemyController = GetComponentInParent<EnemyController>();
        if (_enemyController != null)
            gameObject.layer = _enemyController.gameObject.layer;

        if (_attackCollider == null)
            _attackCollider = GetComponent<Collider>();

        if (_attackCollider != null)
        {
            _attackCollider.isTrigger = true;
            _attackCollider.enabled = false;
        }

        _overlapBuffer = new Collider[_overlapProbeMax];

       
    }

    void LogDbg(string msg)
    {
       // if (_debugDamage) Debug.Log($"[EnemyHitDbg] {msg}");
    }

    public void OnAttackStart()
    {
        _isAttackActive = true;
        _hitTargets.Clear();
        attackShadow.SetActive(true);
        if (_attackCollider != null)
        {
            _attackCollider.enabled = true;
            Physics.SyncTransforms();
            if (_probeOverlapsOnAttackStart)
                ProbeOverlappingPlayer();
        }

        if (_debugDamage)
        {
            if (_attackCollider != null)
            {
                Bounds wb = _attackCollider.bounds;
               // Debug.Log($"[EnemyHitDbg] OnAttackStart '{name}' damage={_damageAmount} colliderEnabled={_attackCollider.enabled} boundsCenter={wb.center} extents={wb.extents}");
            }
            //else Debug.LogWarning($"[EnemyHitDbg] OnAttackStart '{name}' sin _attackCollider asignado; no habrá hit.");
        }
    }

    private void ProbeOverlappingPlayer()
    {
        if (_attackCollider == null || !_isAttackActive) return;

        Bounds b = _attackCollider.bounds;
        b.Expand(_probeBoundsPadding);

        int playerOnlyMask = LayerMask.GetMask("Player");
        if (playerOnlyMask == 0 && _debugDamage)
            Debug.LogWarning("[EnemyHitDbg] No hay capa llamada 'Player' en Tag Manager. Añade la capa 'Player' o asigna colliders del jugador a ella.");

        RunOverlapBoxProbe(b, playerOnlyMask != 0 ? playerOnlyMask : ~0, "Player-only", onlyCollidersWithPlayerHealth: false);

        if (_probeFallbackIncludeDefaultLayer && playerOnlyMask != 0)
        {
            int wideMask = LayerMask.GetMask("Player", "Default");
            if (wideMask != playerOnlyMask)
                RunOverlapBoxProbe(b, wideMask, "Player+Default", onlyCollidersWithPlayerHealth: true);
        }

        if (!_fallbackSphereTowardForward)
            return;

        Vector3 origin = _attackCollider.bounds.center + transform.forward * _fallbackSphereForwardOffset;
        RunOverlapSphereProbe(origin, playerOnlyMask != 0 ? playerOnlyMask : ~0, "Player-only", onlyCollidersWithPlayerHealth: false);

        if (_probeFallbackIncludeDefaultLayer && playerOnlyMask != 0)
        {
            int wideMask = LayerMask.GetMask("Player", "Default");
            if (wideMask != playerOnlyMask)
                RunOverlapSphereProbe(origin, wideMask, "Player+Default", onlyCollidersWithPlayerHealth: true);
        }
    }

    private void RunOverlapBoxProbe(Bounds b, int layerMask, string label, bool onlyCollidersWithPlayerHealth)
    {
        int count = Physics.OverlapBoxNonAlloc(
            b.center,
            b.extents,
            _overlapBuffer,
            Quaternion.identity,
            layerMask,
            QueryTriggerInteraction.Collide);

        if (_debugDamage)
        {
            LogDbg($"Probe box [{label}] mask={layerMask} overlaps={count}");
            if (count == 0)
                LogDistanceToPlayerIfPossible(b.center);
        }

        for (int i = 0; i < count; i++)
        {
            Collider c = _overlapBuffer[i];
            if (c == null || c == _attackCollider) continue;
            if (onlyCollidersWithPlayerHealth && c.GetComponentInParent<PlayerHealth>() == null)
                continue;
            if (_debugDamage && c.GetComponentInParent<PlayerHealth>() != null)
                LogDbg($"  box hit (jugador) '{c.name}' layer={c.gameObject.layer} path={GetTransformPath(c.transform)}");
            TryApplyHit(c);
        }
    }

    private void RunOverlapSphereProbe(Vector3 origin, int layerMask, string label, bool onlyCollidersWithPlayerHealth)
    {
        int count = Physics.OverlapSphereNonAlloc(origin, _fallbackSphereRadius, _overlapBuffer, layerMask, QueryTriggerInteraction.Collide);
        if (_debugDamage)
        {
            LogDbg($"Probe sphere [{label}] center={origin} r={_fallbackSphereRadius} overlaps={count}");
            if (count == 0)
                LogDistanceToPlayerIfPossible(origin);
        }

        for (int i = 0; i < count; i++)
        {
            Collider c = _overlapBuffer[i];
            if (c == null || c == _attackCollider) continue;
            if (onlyCollidersWithPlayerHealth && c.GetComponentInParent<PlayerHealth>() == null)
                continue;
            if (_debugDamage && c.GetComponentInParent<PlayerHealth>() != null)
                LogDbg($"  sphere hit (jugador) '{c.name}' path={GetTransformPath(c.transform)}");
            TryApplyHit(c);
        }
    }

    private void LogDistanceToPlayerIfPossible(Vector3 from)
    {
        if (!_debugDamage) return;
        if (PlayerHealth.Instance == null)
        {
            LogDbg("  (sin PlayerHealth.Instance: no se puede medir distancia al jugador)");
            return;
        }

        Vector3 p = PlayerHealth.Instance.transform.position;
        float d = Vector3.Distance(from, p);
        LogDbg($"  distancia aprox. hit→jugador={d:F2}m (si es grande, el hitbox no alcanza; si es pequeña y overlaps=0, revisa capas del jugador / triggers)");
    }

    static string GetTransformPath(Transform t)
    {
        if (t == null) return "";
        string path = t.name;
        while (t.parent != null)
        {
            t = t.parent;
            path = t.name + "/" + path;
        }
        return path;
    }

    public void OnAttackEnd()
    {
        _isAttackActive = false;
        attackShadow.SetActive(false);
        if (_attackCollider != null)
            _attackCollider.enabled = false;

        if (_debugDamage)
            LogDbg($"OnAttackEnd '{name}'");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_isAttackActive || _attackCollider == null)
        {
            if (_debugDamage && other != null)
                LogDbg($"OnTriggerEnter ignorado (attack inactive o sin collider) other='{other.name}'");
            return;
        }

        if (_debugDamage && (other.GetComponentInParent<PlayerHealth>() != null || _logNonPlayerContacts))
            LogDbg($"OnTriggerEnter other='{other.name}' layer={other.gameObject.layer}");
        TryApplyHit(other);
    }

    private void TryApplyHit(Collider other)
    {
        if (!_isAttackActive || _attackCollider == null || other == null)
        {
            if (_debugDamage)
                LogDbg($"TryApplyHit abort: active={_isAttackActive} attackCol={(_attackCollider != null)} otherNull={other == null}");
            return;
        }

        if (_enemyController != null)
        {
            var otherEnemy = other.GetComponentInParent<EnemyController>();
            if (otherEnemy != null && otherEnemy == _enemyController)
            {
                if (_debugDamage)
                    LogDbg($"TryApplyHit skip mismo enemigo collider='{other.name}'");
                return;
            }
        }

        if (_hitTargets.Contains(other))
        {
            if (_debugDamage)
                LogDbg($"TryApplyHit skip duplicado '{other.name}'");
            return;
        }

        var playerHealth = other.GetComponentInParent<PlayerHealth>();
        if (playerHealth == null)
        {
            if (_debugDamage && _logNonPlayerContacts)
                LogDbg($"TryApplyHit sin PlayerHealth en padres de '{other.name}' layer={other.gameObject.layer} path={GetTransformPath(other.transform)}");
            return;
        }

        _hitTargets.Add(other);

        if (_damageAmount <= 0)
        {
            Debug.LogWarning($"[EnemyHitDbg] Daño es {_damageAmount} — no se aplicará vida. Revisa SetAttackDamage / inspector.");
            return;
        }

        if (PlayerActions.blocking)
        {
            if (_debugDamage)
                LogDbg($"Jugador en bloqueo → evaluar ángulo…");
            Blocking(other.transform, playerHealth);
        }
        else
        {
            if (_debugDamage)
                LogDbg($"TakeDamage al jugador amount={_damageAmount} (sin bloqueo)");
            playerHealth.TakeDamage(_damageAmount);
        }
    }

    void Blocking(Transform other, PlayerHealth playerHealth)
    {
        Vector3 directionFromOtherToMe = (transform.position - other.transform.position).normalized;
        directionFromOtherToMe.y = 0;
        Vector3 otherForward = other.transform.forward;
        otherForward.y = 0;
        float dot = Vector3.Dot(otherForward, directionFromOtherToMe.normalized);
        if (_debugDamage)
            LogDbg($"Blocking dot={dot:F3} (>.5 = bloqueado, sin daño)");
        if (dot > 0.5f)
        {
            Debug.Log("AtaqueBloqueado");
            if (_debugDamage)
                LogDbg("Ataque bloqueado (dot > 0.5)");
            return;
        }

        if (_debugDamage)
            LogDbg($"TakeDamage al jugador (bloqueo fallido) amount={_damageAmount}");
        playerHealth.TakeDamage(_damageAmount);
    }

    public void SetDamage(int amount)
    {
        _damageAmount = amount;
        if (_debugDamage)
            LogDbg($"SetDamage → {_damageAmount}");
    }

    public void ResetHitTargets()
    {
        _hitTargets.Clear();
    }
}
