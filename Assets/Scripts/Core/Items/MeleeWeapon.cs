using UnityEngine;

public class MeleeWeapon : EquipableItemBase
{


    public override bool IsBusy()
    {
        return true;
    }

    public override void ProcessInput(ActionInput input)
    {
        _requestedAttack = input.Attack;
       
        _requestedThrow = input.Throw;
        

        if (_requestedThrow)
        {
            Throw();
        }
    }
}
