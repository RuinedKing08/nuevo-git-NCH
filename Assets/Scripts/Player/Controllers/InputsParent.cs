using UnityEngine;
using UnityEngine.InputSystem;
public class InputsParent : MonoBehaviour
{
    [Header("Input Variables")]
    [SerializeField] private InputActionMap playerInputMap;
    [SerializeField] private InputActionAsset playerInputAsset;
    [Header("Input References")]
    [SerializeField] private InputActionReference moveX;
    [SerializeField] private InputActionReference moveZ;
    [SerializeField] private InputActionReference interaction;
    [SerializeField] private InputActionReference lightAttack;
    [SerializeField] private InputActionReference heavyAttack;
    [SerializeField] private InputActionReference throwI;
    [SerializeField] private InputActionReference lockOn;
    [SerializeField] private InputActionReference sideStep;
    private InputAction moveXAction, moveZAction, interactionAction, lightAttackAction, heavyAttackAction, throwAction, lockOnAction, sideStepAction;
    public static InputsParent Instance;
    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        playerInputMap = playerInputAsset.FindActionMap("PlayerAction");
        moveXAction = moveX.ToInputAction();
        moveZAction = moveZ.ToInputAction();
        interactionAction = interaction.ToInputAction();
        lightAttackAction = lightAttack.ToInputAction();
        heavyAttackAction = heavyAttack.ToInputAction();
        throwAction = throwI.ToInputAction();
        lockOnAction = lockOn.ToInputAction();
        sideStepAction = sideStep.ToInputAction();
    }

    public InputAction MoveXInput() {return moveXAction; }
    public InputAction MoveZInput() {return moveZAction; }
    public InputAction ThrowInput() {return throwAction; }
    public InputAction InteractionInput() {return interactionAction; }
    public InputAction LightAttackInput() {return lightAttackAction; }
    public InputAction HeavyAttackInput() {return heavyAttackAction; }
    public InputAction LockOnInput() {return lockOnAction; }
    public InputAction SideStepInput() {return sideStep; }
    private void OnEnable()
    {
        moveXAction = moveX.ToInputAction();
        moveXAction.Enable();

        moveZAction = moveZ.ToInputAction();
        moveZAction.Enable();

        interactionAction = interaction.ToInputAction();
        interactionAction.Enable();

        lightAttackAction = lightAttack.ToInputAction();
        lightAttackAction.Enable();

        heavyAttackAction = heavyAttack.ToInputAction();
        heavyAttackAction.Enable();

        throwAction = throwI.ToInputAction();
        throwAction.Enable();

        lockOnAction = lockOn.ToInputAction();
        lockOnAction.Enable();

        sideStepAction = sideStep.ToInputAction();
        sideStepAction.Enable();
    }

    private void OnDisable()
    {
        moveXAction.Disable();
        moveZAction.Disable();
        interactionAction.Disable();
        lightAttackAction.Disable();
        heavyAttackAction.Disable();
        throwAction.Disable();
        lockOnAction.Disable();
        sideStepAction.Disable();
    }
}
