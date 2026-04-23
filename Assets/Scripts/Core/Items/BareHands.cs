using System.Collections;
using UnityEngine;

public class BareHands : MeleeWeapon
{
    // BareHands no puede lanzar armas, así que no sobrescribimos Throw()
    // Heredamos el comportamiento de EquipableItemBase

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



