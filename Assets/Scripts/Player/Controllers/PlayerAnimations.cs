using System.Net.NetworkInformation;
using UnityEditor.Purchasing;
using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{
    #region References

    private Animator _animator;
    private PlayerState _tempState;

    [SerializeField]private PlayerIkHeadController _HeadController;

    #endregion
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Initialize()
    {
        _animator = GetComponent<Animator>();
    }

    public void UpdateProperties(PlayerState state)
    {
        _tempState = state;

    }
    public void UpdateAnimator()
    {
        _animator.SetBool("IsGrounded",_tempState.characterState.isGrounded);
        _animator.SetBool("IsStopped", (_tempState.characterState.MoveSpeed >= 0.4f && (_tempState.characterState.MoveSpeed <= -0.4f)));
        _animator.SetFloat("MoveSpeed", _tempState.characterState.MoveSpeed);
        _animator.SetBool("MovementInputTapped", _tempState.generalInput.movementInputTapped);
        _animator.SetBool("MovementInputPressed", _tempState.generalInput.movementInputPressed);
        _animator.SetBool("MovementInputHeld", _tempState.generalInput.movementInputHeld);
        _animator.SetFloat("StrafeDirectionX", _tempState.characterState.inputDirection.x); 
        _animator.SetFloat("StrafeDirectionZ", _tempState.characterState.inputDirection.y);
    }

    public void UpdateIks()
    {
        _HeadController.UpdateHead();
    }

    
    


}
