using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
public class AttackColliderHandler : MonoBehaviour
{
     [SerializeField] private Collider _attackCollider;
    [SerializeField] private int _damageAmount = 10;
    //[SerializeField] GameObject attackShadow;

    [SerializeField]private EnemyController _enemyController;
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
        //if (attackShadow != null) attackShadow.SetActive(true);
        if (_attackCollider != null) _attackCollider.enabled = true;
    }

    public void OnAttackEnd()
    {
        _isAttackActive = false;
        //if (attackShadow != null) attackShadow.SetActive(false);
        if (_attackCollider != null) _attackCollider.enabled = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!_isAttackActive) return;
        TryApplyHit(other);        
    }
    float a, b, c;
    private void TryApplyHit(Collider other)
    {
        if (other == null || _hitTargets.Contains(other)) return;

        var player = other.GetComponentInParent<PlayerHealth>();
        if (player == null) return;
        a++;
        b++;
        c++;
        Debug.Log(a);
        if(a >= 2)
        {
            a = 0;
            return;
        }
        if(b >= 3)
        {
            a = 0;
            b = 0;
            return;
        }
        if (PlayerActions.isDodging) { float score = 50; Currency.Instance.ChangeMoney(0.10f, score); Score(score); return;}

        
        if (PlayerActions.blocking)
        {
            Vector3 dirToEnemy = (transform.position - other.transform.position).normalized;
            dirToEnemy.y = 0;
            Vector3 otherForward = other.transform.forward;
            otherForward.y = 0;
            float dot = Vector3.Dot(otherForward, dirToEnemy.normalized);
            
            if (dot > 0.5f) 
            {
                //_hitTargets.Add(other);
                float score = 25;
                Currency.Instance.ChangeMoney(0.05f, score);
                Score(score);
                if (!pushBackActivated) StartCoroutine(PushBackEnemy());
                return;
            }
            else
            {
                _hitTargets.Add(other);
                player.TakeDamage(_damageAmount);
                return;
            }            
        }
        
        _hitTargets.Add(other);
        player.TakeDamage(_damageAmount);
    }
    void Score(float score)
    {
        ScorePopUp.Instance.CreateScorePopUp(score);
    }
    IEnumerator PushBackEnemy()
    {
        pushBackActivated = true;
        NavMeshAgent agent = _enemyController.Stats.NavAgent;
        agent.isStopped = true;
        agent.ResetPath();
        agent.updatePosition = false;
        agent.updateRotation = false;
        float pushForce = 15f;
        float pushDuration = 0.25f;
        Vector3 pushDirection = -transform.forward;
        pushDirection.y = 0;
        pushDirection.Normalize();
        float timer = 0;
        while (timer < pushDuration)
        {
            transform.position += pushDirection * pushForce * Time.deltaTime;
            agent.nextPosition = transform.position;
            timer += Time.deltaTime;
            yield return null;
        }
        NavMeshHit hit;
        Vector3 finalPosition = transform.position;
        if (NavMesh.SamplePosition(transform.position, out hit, 2, NavMesh.AllAreas))
        {
            finalPosition = hit.position;

        }
        transform.position = finalPosition;
        agent.Warp(finalPosition);
        agent.updatePosition = true;
        agent.updateRotation = true;
        agent.isStopped = false;
        pushBackActivated = false;
    }

    public void SetDamage(int amt) => _damageAmount = amt;

   
    public int GetDamage() => _damageAmount;

    
    public void ModifyDamage(int amount)
    {
        _damageAmount += amount;
        _damageAmount = Mathf.Max(0, _damageAmount);
    }

    
    public void SetDamageMultiplier(float multiplier)
    {
        _damageAmount = Mathf.Max(1, Mathf.RoundToInt(_damageAmount * multiplier));
    }
}
