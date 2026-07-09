using UnityEngine;

public class EnemyAnimationRelay : MonoBehaviour
{
    private EnemyController _enemyController;
    private BossController _bossController;
    [SerializeField] ParticleSystem leftHandAnticipacion;

    void Start()
    {
        
        _enemyController = GetComponentInParent<EnemyController>();
        _bossController = GetComponentInParent<BossController>();
        
        if (_enemyController == null && _bossController == null)
        {
            Debug.LogError($"No se encontró EnemyController ni BossController en los padres de {gameObject.name}");
        }
    }

    // --- EVENTOS PARA LOS ATAQUES DEL ENEMIGO/BOSS ---

    public void AE_EnemyAttackStart()
    {
        if (_enemyController != null) 
            _enemyController.EnableAttackColliders();
        else if (_bossController != null)
            _bossController.EnableAttackColliders();
    }

    public void AE_EnemyAttackEnd()
    {
        if (_enemyController != null) 
            _enemyController.DisableAttackColliders();
        else if (_bossController != null)
            _bossController.DisableAttackColliders();
    }

    
    public void AE_EnemyActionFinished()
    {
        if (_enemyController != null && _enemyController.CombatGroup != null)
        {
            _enemyController.CombatGroup.ReportAttackFinished(_enemyController);
        }
        
    }

    public void StartAnticipationParticle()
    {
        if (leftHandAnticipacion != null)
            leftHandAnticipacion.Play();
    }

    public void AE_EnemyHitSound()
    {
        if (_enemyController != null && _enemyController.Stats != null)
        {
            AudioManager.Instance.Play(_enemyController.Stats.HitSound);
        }
        else if (_bossController != null && _bossController.Stats != null)
        {
            AudioManager.Instance.Play(_bossController.Stats.HitSound);
        }
    }
}
