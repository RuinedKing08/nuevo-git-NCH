using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    #region References
    [SerializeField] private PlayerCamera playerCamera;
    [SerializeField] private PlayerCharacter playerCharacter;
    [SerializeField] private PlayerAnimations playerAnimations;
    [SerializeField] private PlayerActionController playerActionsC;

    #endregion

    #region Runtime
    [SerializeField] private PlayerState _playerState;
    private SystemInputActions actions;
    private SystemInputActions.PlayerActions input;

   


    #endregion

    public void Awake()
    {
        actions = new SystemInputActions();
        input = actions.Player;

    }

    private void OnEnable()
    {
        input.Enable();
    }

    private void OnDisable()
    {
        input.Disable();
    }
    public void Start()
    {
        _playerState.generalInput = new GeneralInput
        {
            movementInputPressed = false,
            movementInputHeld = false,
            movementInputTapped = false,
            ChangueFromToMenu = false,
            movementInputDuration = 0f,
            inputDirection = Vector2.zero,
        };

        playerCamera.Initialize(playerCharacter.CamPos);
        playerCharacter.Initialize();
        playerAnimations.Initialize();
        playerActionsC.Initialize();

    }

    public void Update()
    {
        ProcessInput();
       
        playerAnimations.UpdateProperties(_playerState);
        playerAnimations.UpdateAnimator();
        playerCamera.BeforeCameraUpdate();
    }

    public void FixedUpdate()
    {
        _playerState.characterState = playerCharacter._characterState;
        playerActionsC.Detect(playerCamera._CMcamera.transform);


    }

    public void LateUpdate()
    {
        playerCamera.CameraUpdate();
        playerCamera.UpdatePosition(playerCharacter.CamPos);
    }

    public void ProcessInput()
    {
        var cameraInput = new CameraInput
        {
            Look = input.Look.ReadValue<Vector2>(),
            LockOn = GameSettings.Instance.lockOnSettings == InputSettings.Toggle ?  input.LockOn.WasPressedThisFrame() : input.LockOn.IsPressed(),
        };

        playerCamera.ProcessInput(cameraInput);

        var characterInput = new characterMotorInput
        {
            Move = input.Move.ReadValue<Vector2>(),
            Crouch = input.Crouch.WasPressedThisFrame(),
            Rotation = playerCamera.transform.rotation
        };
        playerCharacter.ProcessInput(characterInput);

        var ActionControllerInput = new ActionInput
        {
            Interact = input.Interact.WasPressedThisFrame(),
            Attack = input.Fire.WasPerformedThisFrame(), // See this after if can be pressed continue
            Fire = input.Fire.IsPressed(),
            Throw = input.Throw.WasPressedThisFrame()
        };
        playerActionsC.ProcessInput(ActionControllerInput);


        if (characterInput.Move.magnitude > 0.1f)
        {
            if (_playerState.generalInput.movementInputDuration == 0)
            {
                _playerState.generalInput.movementInputTapped = true;
            }
            else if (_playerState.generalInput.movementInputDuration > 0 && _playerState.generalInput.movementInputDuration < GameSettings.Instance.buttonHoldTreshhold)

            {
                _playerState.generalInput.movementInputTapped = false;
                _playerState.generalInput.movementInputHeld = false;
                _playerState.generalInput.movementInputPressed = true;

            }
            else
            {
                _playerState.generalInput.movementInputTapped = false;
                _playerState.generalInput.movementInputHeld = true;
                _playerState.generalInput.movementInputPressed = false;
            }
            _playerState.generalInput.movementInputDuration += Time.deltaTime;
            _playerState.generalInput.inputDirection = characterInput.Move;


        }
        else
        {
            _playerState.generalInput.movementInputDuration = 0f;
            _playerState.generalInput.movementInputTapped = false;
            _playerState.generalInput.movementInputHeld = false;
            _playerState.generalInput.movementInputPressed = false;

        }


    }


}
