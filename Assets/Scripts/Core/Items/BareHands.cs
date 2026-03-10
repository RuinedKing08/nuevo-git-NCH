using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class BareHands : MeleeWeapon
{
    
    
    public override void Throw(){ }

    public override void Attack()
    {
        if (IsBusy()) return;
        
        busy = true;

        controller.SetActionState(ActionState.ATTACK);

        controller.StartCoroutine(AttackCorrutine());
    }

    IEnumerator AttackCorrutine()
    {
        yield return new WaitForSeconds(0.4f);

        busy = false;

        controller.SetActionState(ActionState.NONE);
    }


}
