using UnityEngine;

public class LeftHandAttack : MonoBehaviour
{
    void Start()
    {
        
    }


    private void OnTriggerEnter(Collider other)
    {
        if(PlayerCombatSystem.Instance._comboIndex == 1) PlayerHitCollider.Instance.TriggerAttack(other);
    }
}
