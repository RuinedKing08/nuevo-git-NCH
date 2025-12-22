using UnityEngine;

public class FireArmWeapon : EquipableItemBase
{
     // WeaponSettings here
    public override bool IsBusy()
    {
        return false;
    }

    public override void ProcessInput(ActionInput input)
    {
        
    }
}
