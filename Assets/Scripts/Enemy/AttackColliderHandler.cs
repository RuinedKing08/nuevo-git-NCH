using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
public class AttackColliderHandler : MonoBehaviour
{
    [SerializeField] private Collider _attackCollider;
    [SerializeField] private int _damageAmount = 10;
    [SerializeField] GameObject attackShadow;

    private EnemyController _enemyController;
    private HashSet<Collider> _hitTargets = new HashSet<Collider>();
    private bool _isAttackActive = false;
    private NavMeshAgent agent;
    private bool pushBackActivated;

    private void Awake()
    {
        _enemyController = GetComponentInParent<EnemyController>();
        if (_attackCollider == null) _attackCollider = GetComponent<Collider>();
        if (_attackCollider != null)
        {
            _attackCollider.isTrigger = true;
            _attackCollider.enabled = false;
        }
        agent = GetComponentInParent<NavMeshAgent>();
    }

    public void OnAttackStart()
    {
        _isAttackActive = true;
        _hitTargets.Clear();
        attackShadow.SetActive(true);
        if (_attackCollider != null) _attackCollider.enabled = true;
    }

    public void OnAttackEnd()
    {
        _isAttackActive = false;
        attackShadow.SetActive(false);
        if (_attackCollider != null) _attackCollider.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_isAttackActive) return;
        TryApplyHit(other);
    }

    private void TryApplyHit(Collider other)
    {
        if (!_isAttackActive || other == null || _hitTargets.Contains(other)) return;

        var playerHealth = other.GetComponentInParent<PlayerHealth>();
        if (playerHealth == null) return;
        
        if (PlayerActions.isDodging)
        {
            Debug.Log("Jugador esquivó con I-Frames");
            return; 
        }

        if (PlayerActions.blocking)
        {
            Blocking(other.transform, playerHealth);
        }
        else
        {
            _hitTargets.Add(other);
            playerHealth.TakeDamage(_damageAmount);
        }
    }

    void Blocking(Transform playerTransform, PlayerHealth playerHealth)
    {
        Vector3 dirToEnemy = (transform.position - playerTransform.position).normalized;
        dirToEnemy.y = 0;
        float dot = Vector3.Dot(playerTransform.forward, dirToEnemy);

        if (dot > 0.5f) 
        {
            Debug.Log("Ataque Bloqueado");
            if (!pushBackActivated) StartCoroutine(PushBack());
            return;
        }
        
        _hitTargets.Add(playerTransform.GetComponent<Collider>());
        playerHealth.TakeDamage(_damageAmount);
    }

    IEnumerator PushBack()
    {
        pushBackActivated = true;
        if (agent != null) agent.isStopped = true;
        
        float pushForce = 15f;
        float timer = 0.2f;
        Vector3 pushDir = -transform.forward;

        while (timer > 0)
        {
            transform.root.position += pushDir * pushForce * Time.deltaTime;
            timer -= Time.deltaTime;
            yield return null;
        }

        if (agent != null) agent.isStopped = false;
        pushBackActivated = false;
    }

    public void SetDamage(int amount) => _damageAmount = amount;
}
