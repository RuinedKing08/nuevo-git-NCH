using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    private PlayerCombatSystem _combatSystem;

    private void Awake()
    {
        _combatSystem = GetComponent<PlayerCombatSystem>();
    }

    void Update()
    {        
        if (InputsParent.Instance.LightAttackInput().triggered)
        {
            _combatSystem.PerformLightAttack();
        }
    }
}
