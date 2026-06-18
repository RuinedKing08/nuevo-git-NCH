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
    [SerializeField] private InputActionReference throwI;
    [SerializeField] private InputActionReference lockOn;
    [SerializeField] private InputActionReference sideStep;
    [SerializeField] private InputActionReference block;
    [SerializeField] private InputActionReference pause;
    private InputAction moveXAction, moveZAction, interactionAction, lightAttackAction, heavyAttackAction, throwAction, lockOnAction, sideStepAction, blockAction, pauseAction;
    public static InputsParent Instance;
    private void Awake()
    {
        Instance = this;
        playerInputMap = playerInputAsset.FindActionMap("PlayerAction");
        OnEnable();
    }
    void Start()
    {
        playerInputMap = playerInputAsset.FindActionMap("PlayerAction");
        moveXAction = moveX.ToInputAction();
        moveZAction = moveZ.ToInputAction();
        interactionAction = interaction.ToInputAction();
        lightAttackAction = lightAttack.ToInputAction();        
        throwAction = throwI.ToInputAction();
        lockOnAction = lockOn.ToInputAction();
        sideStepAction = sideStep.ToInputAction();
        blockAction = block.ToInputAction();
        pauseAction = pause.ToInputAction();
    }

    public InputAction MoveXInput() {return moveXAction; }
    public InputAction MoveZInput() {return moveZAction; }
    public InputAction ThrowInput() {return throwAction; }
    public InputAction InteractionInput() {return interactionAction; }
    public InputAction LightAttackInput() {return lightAttackAction; }    
    public InputAction LockOnInput() {return lockOnAction; }
    public InputAction SideStepInput() {return sideStepAction; }
    public InputAction BlockInput() {return blockAction; }
    public InputAction PauseInput() {return pauseAction; }
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
        

        throwAction = throwI.ToInputAction();
        throwAction.Enable();

        lockOnAction = lockOn.ToInputAction();
        lockOnAction.Enable();

        sideStepAction = sideStep.ToInputAction();
        sideStepAction.Enable();

        blockAction = block.ToInputAction();
        blockAction.Enable();

        pauseAction = pause.ToInputAction();
        pauseAction.Enable();
    }

    private void OnDisable()
    {
        moveXAction.Disable();
        moveZAction.Disable();
        interactionAction.Disable();
        lightAttackAction.Disable();
        throwAction.Disable();
        lockOnAction.Disable();
        sideStepAction.Disable();
        blockAction.Disable();
        pauseAction.Disable();
    }
    private void OnDestroy()
    {
        OnDisable();
    }
}
