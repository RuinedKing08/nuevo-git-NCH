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
    private bool pushBackActivated;

    private void Awake()
    {
        _enemyController = GetComponentInParent<EnemyController>();
        if (_attackCollider == null) _attackCollider = GetComponent<Collider>();
        if (_attackCollider != null) { _attackCollider.isTrigger = true; _attackCollider.enabled = false; }
    }

    public void OnAttackStart()
    {
        _isAttackActive = true;
        _hitTargets.Clear();
        if (attackShadow != null) attackShadow.SetActive(true);
        if (_attackCollider != null) _attackCollider.enabled = true;
    }

    public void OnAttackEnd()
    {
        _isAttackActive = false;
        if (attackShadow != null) attackShadow.SetActive(false);
        if (_attackCollider != null) _attackCollider.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_isAttackActive) return;
        TryApplyHit(other);
    }

    private void TryApplyHit(Collider other)
    {
        if (other == null || _hitTargets.Contains(other)) return;

        var player = other.GetComponentInParent<PlayerHealth>();
        if (player == null) return;

        
        if (PlayerActions.isDodging) return;

        
        if (PlayerActions.blocking)
        {
            Vector3 dirToEnemy = (transform.position - other.transform.position).normalized;
            dirToEnemy.y = 0;
            float dot = Vector3.Dot(other.transform.forward, dirToEnemy);

            if (dot > 0.45f) 
            {
                _hitTargets.Add(other);
                if (!pushBackActivated) StartCoroutine(PushBackEnemy());
                return;
            }
        }
        
        _hitTargets.Add(other);
        player.TakeDamage(_damageAmount);
    }

    IEnumerator PushBackEnemy()
    {
        pushBackActivated = true;
        NavMeshAgent agent = _enemyController.Stats.NavAgent;
        if (agent != null && agent.isOnNavMesh)
        {
            agent.isStopped = true;
            float t = 0.2f;
            Vector3 pushDir = -_enemyController.transform.forward;
            while(t > 0) { 
                _enemyController.transform.position += pushDir * 5f * Time.deltaTime; 
                t -= Time.deltaTime; 
                yield return null; 
            }
            yield return new WaitForSeconds(0.4f);
            if(agent.isOnNavMesh) agent.isStopped = false;
        }
        pushBackActivated = false;
    }

    public void SetDamage(int amt) => _damageAmount = amt;
}
