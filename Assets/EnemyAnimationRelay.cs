using UnityEngine;

public class EnemyAnimationRelay : MonoBehaviour
{
    private EnemyController _controller;
    [SerializeField] ParticleSystem leftHandAnticipacion;

    void Start()
    {
        
        _controller = GetComponentInParent<EnemyController>();
        
        if (_controller == null)
        {
            Debug.LogError($"No se encontró EnemyController en los padres de {gameObject.name}");
        }
    }

    // --- EVENTOS PARA LOS ATAQUES DEL ENEMIGO ---

    public void AE_EnemyAttackStart()
    {
        if (_controller != null) _controller.EnableAttackColliders();
    }

    public void AE_EnemyAttackEnd()
    {
        if (_controller != null) _controller.DisableAttackColliders();
    }

    
    public void AE_EnemyActionFinished()
    {
        if (_controller != null && _controller.CombatGroup != null)
        {
            _controller.CombatGroup.ReportAttackFinished(_controller);
        }
    }

    public void StartAnticipationParticle()
    {
        leftHandAnticipacion.Play();
    }

    public void AE_EnemyHitSound()
    {
        if (_controller != null)
        {
            AudioManager.Instance.Play(_controller.Stats.HitSound);
        }
    }
}
