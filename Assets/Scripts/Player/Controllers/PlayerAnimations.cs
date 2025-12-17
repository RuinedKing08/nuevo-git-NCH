using System.Net.NetworkInformation;
using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{
    #region References

    private Animator _animator;
    private PlayerState _tempState;

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
    }
 
}
