using UnityEngine;

public class AnimationEventRelay : MonoBehaviour
{
    private PlayerCombatSystem combatSystem;
    private PlayerActions playerActions;

    void Start()
    {
        
        combatSystem = GetComponentInParent<PlayerCombatSystem>();
        playerActions = GetComponentInParent<PlayerActions>();
    }

    
    public void AE_EnableHitbox() => combatSystem?.AE_EnableHitbox();
    public void AE_DisableHitbox() => combatSystem?.AE_DisableHitbox();
    public void AE_AttackEnd() => combatSystem?.AE_AttackEnd();
    
    public void AE_StartDodge() => playerActions?.AE_StartDodge();
    public void AE_EndDodge() => playerActions?.AE_EndDodge();
}
