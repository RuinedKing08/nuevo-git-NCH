using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private PlayerCombatSystem _combatSystem;
    
    [Header("Input Values")]
    [SerializeField] public float lightAttackfloat;
    [SerializeField] public float heavyAttackfloat;
    [SerializeField] GameObject attackShadow;

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
            attackShadow.SetActive(true);
        }
        else
        {
            attackShadow.SetActive(false);
        }

        if (heavyAttackfloat > 0)
        {
            _combatSystem.PerformHeavyAttack();
            attackShadow.SetActive(true);

        }
    }

    
    public PlayerCombatSystem GetCombatSystem() => _combatSystem;
}
