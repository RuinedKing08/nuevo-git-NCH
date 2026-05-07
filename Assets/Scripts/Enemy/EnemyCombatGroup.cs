using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class EnemyCombatGroup : MonoBehaviour
{
    [SerializeField]List<EnemyController> _members       = new();
    [SerializeField]List<EnemyController> _currentAttackers = new();

    private int _finishedAttackersCount = 0;
    private bool _roundInProgress = false;
    const int MaxAttackers = 3;

    public void AddEnemy(EnemyController e)
    {
        if (!_members.Contains(e)) _members.Add(e);        
    }

    public void RemoveEnemy(EnemyController e)
    {
        _members.Remove(e);
        _currentAttackers.Remove(e);
    }
    public void ReportAttackFinished(EnemyController e)
    {
        if (_currentAttackers.Contains(e))
        {
            _finishedAttackersCount++;
        }
    }

    public void NotifyExchangeReady(EnemyController e)
    {
        if (!_roundInProgress && _currentAttackers.Count == 0)
        {
            StartCoroutine(BeginAttackRound());
        }
    }
    public float GetTargetAngleForMember(EnemyController member)
    {
        int index = _members.IndexOf(member);
        if (index == -1) return 0;
        
        float angleStep = 360f / _members.Count;
        return index * angleStep;
    }

    IEnumerator BeginAttackRound()
    {
        _roundInProgress = true;
        _finishedAttackersCount = 0;
        
       
        bool anyPlayerNearby = _members.Any(m => m.Detection != null && (m.Detection.PlayerInInterestArea || m.Detection.PlayerInDangerArea));
        Debug.Log($"BeginAttackRound: members={_members.Count}, anyPlayerNearby={anyPlayerNearby}");
        if (!anyPlayerNearby || _members.Count == 0)
        {
            _roundInProgress = false;
            yield break;
        }

        
        float maxAttackDistance = 10f; 
        var candidates = _members
            .Where(m => m.StateMachine.CurrentState == m.CombatState &&
                        m.CombatState.IsInIdleSubState() &&
                        (m.Detection.PlayerInInterestArea || m.Detection.PlayerInDangerArea || Vector3.Distance(m.transform.position, m.Detection.LastKnownPlayerPosition) <= maxAttackDistance))
            .ToList();

        
        if (candidates.Count == 0)
        {
            Debug.Log("BeginAttackRound: no candidates found (none in idle or in range). Members:");
            foreach (var m in _members)
            {
                var dist = Vector3.Distance(m.transform.position, m.Detection.LastKnownPlayerPosition);
                Debug.Log($" - {m.name}: state={m.StateMachine.CurrentState?.GetType().Name}, sub={m.CombatState.GetSubStateName()}, distToPlayer={dist}, inInterest={m.Detection.PlayerInInterestArea}, inDanger={m.Detection.PlayerInDangerArea}");
            }
            _roundInProgress = false;
            yield break; 
        }

        
        int attackerCount = Mathf.Min(MaxAttackers, candidates.Count);
        _currentAttackers.Clear();

        for (int i = 0; i < attackerCount; i++)
        {
            int idx = Random.Range(0, candidates.Count);
            _currentAttackers.Add(candidates[idx]);
            candidates.RemoveAt(idx);
        }
        //Debug.Log($"BeginAttackRound: selected attackers: {string.Join(", ", _currentAttackers.Select(a=>a.name))}");

        
        var defenders = _members
            .Where(m => m.StateMachine.CurrentState == m.CombatState && !_currentAttackers.Contains(m))
            .ToList();
        //Debug.Log($"EnemyCombatGroup: assigning { _currentAttackers.Count } attackers and { defenders.Count } defenders");
        foreach (var d in defenders)
        {
            //Debug.Log($"EnemyCombatGroup: AssignDefend -> {d.name}");
            d.CombatState.IdleSubState.AssignDefend();
        }
        foreach (var a in _currentAttackers)
        {
            //Debug.Log($"EnemyCombatGroup: AssignAttack -> {a.name}");
            a.CombatState.IdleSubState.AssignAttack();
        }

        
        int targetFinishes = Mathf.Clamp(attackerCount - 1, 1, 2); 
        if (attackerCount == 3) targetFinishes = 2; 

        float timeout = 5f; 
        float timer = 0f;

        while (_finishedAttackersCount < targetFinishes && timer < timeout)
        {
            timer += Time.deltaTime;
            yield return null; 
        }
        
        foreach (var d in defenders)
        {
            
            if(d != null && d.CombatState != null)
                d.CombatState.DefendSubState.ForceToExchange();
        }
        
        _currentAttackers.Clear();
        _roundInProgress = false;
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



