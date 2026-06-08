using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class EnemyCombatGroup : MonoBehaviour
{
    public static EnemyCombatGroup Instance { get; private set; }
    [SerializeField] private List<EnemyController> _members = new();
    [SerializeField] private List<EnemyController> _currentAttackers = new();
    private bool _roundInProgress = false;
    private int _finishedCount = 0;
    public event ChangeCurrentMembers OnChangeCurrentMembers;
    public delegate void ChangeCurrentMembers();
    public event DecreaseCurrentMembers OnDecreaseCurrentMembers;
    public delegate void DecreaseCurrentMembers();

    private void Awake() { Instance = this; }

    public void AddEnemy(EnemyController e)
    {
        if (!_members.Contains(e))
        {
            _members.Add(e);
            InvokeChangeCurrentMembers();
        }
    }
    public void RemoveEnemy(EnemyController e) { _members.Remove(e); _currentAttackers.Remove(e); InvokeChangeCurrentMembers(); InvokeDecreaseCurrentMembers(); }

    public Vector3 GetOrbitPosition(EnemyController member, Vector3 playerPos)
    {
        int index = _members.IndexOf(member);
        if (index == -1) return playerPos;

        
        float baseAngle = index * (360f / Mathf.Max(1, _members.Count));
        float rotationDir = (member.IndividualSeed > 500) ? 1 : -1;
        float individualRotation = Time.time * (15f + (member.IndividualSeed % 10f)) * rotationDir;
        
        float finalAngle = baseAngle + individualRotation;

        
        float radiusNoise = Mathf.PerlinNoise(Time.time * 0.2f, member.IndividualSeed + 100f);
        float dynamicRadius = Mathf.Lerp(4.5f, 7.5f, radiusNoise);

        Vector3 offset = new Vector3(Mathf.Cos(finalAngle * Mathf.Deg2Rad), 0, Mathf.Sin(finalAngle * Mathf.Deg2Rad)) * dynamicRadius;
        return playerPos + offset;
    }

    public void NotifyExchangeReady(EnemyController e)
    {
        if (!_roundInProgress) StartCoroutine(BeginAttackRound());
    }

    public void ReportAttackFinished(EnemyController e) => _finishedCount++;

    private IEnumerator BeginAttackRound()
    {
        _roundInProgress = true;
        _finishedCount = 0;

        var candidates = _members.Where(m => m.StateMachine.CurrentState == m.CombatState).ToList();
        if (candidates.Count == 0) { _roundInProgress = false; yield break; }

        _currentAttackers = candidates.Take(3).ToList(); 

        foreach (var m in _members)
        {
            if (m.StateMachine.CurrentState != m.CombatState) continue;
            if (_currentAttackers.Contains(m)) m.CombatState.IdleSubState.AssignAttack();
            else /*m.CombatState.IdleSubState.AssignDefend()*/;
        }

        yield return new WaitForSeconds(4f); 

        foreach (var m in _members) 
            if(m.StateMachine.CurrentState == m.CombatState) m.CombatState.DefendSubState.ForceToExchange();

        _currentAttackers.Clear();
        _roundInProgress = false;
    }
    public List<EnemyController> GetCurrentMembers() { return _members; }
    public List<EnemyController> GetCurrentAttackers() { return _currentAttackers; }
    public void InvokeChangeCurrentMembers() { OnChangeCurrentMembers?.Invoke(); }
    public void InvokeDecreaseCurrentMembers() { OnDecreaseCurrentMembers?.Invoke(); }
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



