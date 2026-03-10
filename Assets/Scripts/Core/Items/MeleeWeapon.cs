using UnityEngine;

public class MeleeWeapon : EquipableItemBase
{

    protected bool busy;

    public override void ProcessInput(ActionInput input)
    {
        _requestedAttack = input.Attack;
       
        _requestedThrow = input.Throw;
        

        if (_requestedThrow && !IsBusy())
        {
            Throw();
        }
        if(_requestedAttack && !IsBusy())
        {
            Attack();  
        }


    }

    public virtual void Attack()
    {

    }

    public override bool IsBusy()
    {
        return busy;
    }
}
