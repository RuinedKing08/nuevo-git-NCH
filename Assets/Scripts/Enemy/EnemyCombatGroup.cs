using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class EnemyCombatGroup : MonoBehaviour
{
    [SerializeField]List<EnemyController> _members       = new();
    [SerializeField]List<EnemyController> _currentAttackers = new();

    const int MaxAttackers = 3;

    public void AddEnemy(EnemyController e)
    {
        _members.Add(e);
        e.CombatGroup = this;
    }

    public void RemoveEnemy(EnemyController e)
    {
        _members.Remove(e);
        _currentAttackers.Remove(e);
    }

    public void NotifyExchangeReady(EnemyController e)
    {
        if (_currentAttackers.Count == 0)
            StartCoroutine(BeginAttackRound()); 
    }

    IEnumerator BeginAttackRound()
    {
        bool anyPlayerNearby = _members.Any(m => m.Detection != null && (m.Detection.PlayerInInterestArea || m.Detection.PlayerInDangerArea));
        if (!anyPlayerNearby)
            yield break;
        var idleMembers = _members
            .Where(m => m.CombatState != null &&
                        m.StateMachine.CurrentState == m.CombatState)
            .ToList();

        _currentAttackers.Clear();

        int attackerCount = Mathf.Min(MaxAttackers, idleMembers.Count);
        for (int i = 0; i < attackerCount; i++)
        {
            int idx = Random.Range(0, idleMembers.Count);
            _currentAttackers.Add(idleMembers[idx]);
            idleMembers.RemoveAt(idx);
        }

        var defenders = _members.Except(_currentAttackers).ToList();
        foreach (var d in defenders)
            d.CombatState.IdleSubState.AssignDefend();

        foreach (var a in _currentAttackers)
            a.CombatState.IdleSubState.AssignAttack();

        yield return new WaitForSeconds(2.5f);

        foreach (var d in defenders)
            d.CombatState.DefendSubState.ForceToExchange();

        _currentAttackers.Clear();
    }
}
//═════════════════════════════════════════════════════════════════════════

public class EnemyMeleeWeapon : MonoBehaviour
{
    [SerializeField] private float throwForce = 20f;
    [SerializeField] private Rigidbody _rb;
    
    private bool _isEquipped;
    
    void OnEnable()
    {
        if (_rb == null) _rb = GetComponent<Rigidbody>();
    }

    public void Pickup()
    {
        _isEquipped = true;
        if (_rb) _rb.isKinematic = true;
        gameObject.SetActive(true);
    }

    public void Drop()
    {
        _isEquipped = false;
        if (_rb)
        {
            _rb.isKinematic = false;
            _rb.linearVelocity = Vector3.zero;
        }
    }

    public void Throw(Vector3 targetPos)
    {
        _isEquipped = false;
        if (_rb)
        {
            _rb.isKinematic = false;
            Vector3 direction = (targetPos - transform.position).normalized;
            _rb.linearVelocity = direction * throwForce;
        }
    }
}

public class EnemyThrowableObject : MonoBehaviour
{
    [SerializeField] private float throwForce = 15f;
    [SerializeField] private Rigidbody _rb;
    
    private bool _isEquipped;
    
    void OnEnable()
    {
        if (_rb == null) _rb = GetComponent<Rigidbody>();
    }

    public void Pickup()
    {
        _isEquipped = true;
        if (_rb) _rb.isKinematic = true;
        gameObject.SetActive(true);
    }

    public void Drop()
    {
        _isEquipped = false;
        if (_rb)
        {
            _rb.isKinematic = false;
            _rb.linearVelocity = Vector3.zero;
        }
    }

    public void Throw(Vector3 targetPos)
    {
        _isEquipped = false;
        if (_rb)
        {
            _rb.isKinematic = false;
            Vector3 direction = (targetPos - transform.position).normalized;
            _rb.linearVelocity = direction * throwForce;
        }
    }
}



