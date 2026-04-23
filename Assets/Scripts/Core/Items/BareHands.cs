using System.Collections;
using UnityEngine;

public class BareHands : MeleeWeapon
{
    void Reset()
    {
        // Valores recomendados para puños
        attackRadius = 1.0f;
        attackDelay = 0.2f;
        attackCooldown = 0.5f;
        if (settings == null)
        {
            var defaultSettings = ScriptableObject.CreateInstance<WeaponSettings>();
            defaultSettings.Damage = 8f;
            settings = defaultSettings;
        }
    }

    // Procesar input de forma simple: reutilizamos PerformAttack() heredado.
    public override void ProcessInput(ActionInput input)
    {
        if (input.Attack && !IsBusy())
        {
            controller.SetActionState(ActionState.ATTACK);
            controller.StartCoroutine(PerformAttack()); // llama al PerformAttack protegido de la base
        }
    }


}



