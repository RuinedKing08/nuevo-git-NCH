using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private PlayerCombatSystem _combatSystem;
    
    [Header("Input Values")]
    [SerializeField] public float lightAttackfloat;
    [SerializeField] public float heavyAttackfloat;

    private void Awake()
    {
        if (_combatSystem == null)
            _combatSystem = GetComponent<PlayerCombatSystem>();
    }

    void Start()
    {
        
    }
    
    void Update()
    {
        GetInput();
        HandleAttackInput();
    }

    
    private void GetInput()
    {
        lightAttackfloat = InputsParent.Instance.LightAttackInput().ReadValue<float>();
        heavyAttackfloat = InputsParent.Instance.HeavyAttackInput().ReadValue<float>();
    }

   
    private void HandleAttackInput()
    {
        if (_combatSystem == null) return;

        if (lightAttackfloat > 0)
        {
            _combatSystem.PerformLightAttack();
        }
        
        if (heavyAttackfloat > 0)
        {
            _combatSystem.PerformHeavyAttack();
        }
    }

    
    public PlayerCombatSystem GetCombatSystem() => _combatSystem;
}
