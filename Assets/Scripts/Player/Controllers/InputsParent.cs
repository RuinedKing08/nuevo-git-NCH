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
    [SerializeField] private InputActionReference attack;
    [SerializeField] private InputActionReference throwI;
    [SerializeField] private InputActionReference lockOn;
    private InputAction moveXAction, moveZAction, interactionAction, attackAction, throwAction, lockOnAction;
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
        attackAction = attack.ToInputAction();
        throwAction = throwI.ToInputAction();
        lockOnAction = lockOn.ToInputAction();
    }

    void Update()
    {
        
    }
    public InputAction MoveXInput() {return moveXAction; }
    public InputAction MoveZInput() {return moveZAction; }
    public InputAction ThrowInput() {return throwAction; }
    public InputAction InteractionInput() {return interactionAction; }
    public InputAction AttackInput() {return attackAction; }
    public InputAction lockOnInput() {return lockOnAction; }
    private void OnEnable()
    {
        moveXAction = moveX.ToInputAction();
        moveXAction.Enable();

        moveZAction = moveZ.ToInputAction();
        moveZAction.Enable();

        interactionAction = interaction.ToInputAction();
        interactionAction.Enable();

        attackAction = attack.ToInputAction();
        attackAction.Enable();

        throwAction = throwI.ToInputAction();
        throwAction.Enable();

        lockOnAction = lockOn.ToInputAction();
        lockOnAction.Enable();
    }

    private void OnDisable()
    {
        moveXAction.Disable();
        moveZAction.Disable();
        interactionAction.Disable();
        attackAction.Disable();
        throwAction.Disable();
        lockOnAction.Disable();
    }
}
