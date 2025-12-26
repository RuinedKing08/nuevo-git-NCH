using UnityEngine;

public class FireArmWeapon : EquipableItemBase
{
    // WeaponSettings here

   
    private bool _requestedAim;
    public override bool IsBusy()
    {
        return false;
    }

    public override void ProcessInput(ActionInput input)
    {
        _requestedAttack = (weaponSettings as FireWeaponSettings)._fireMode == FireMode.Semi ? input.Attack : input.Fire;
      
        _requestedThrow = input.Throw;
        _requestedAim = input.Aim;

        if (_requestedThrow)
        {
            Throw();
        }
    }


}
