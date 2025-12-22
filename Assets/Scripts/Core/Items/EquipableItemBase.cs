using UnityEngine;

public abstract class EquipableItemBase : ItemBase, IEquipable
{
    protected PlayerActionController controller;
    public virtual void OnEquip(PlayerActionController controller)
    {
        this.controller = controller;
    }

    public void OnUnequip()
    {
        controller = null;
    }

    public abstract void ProcessInput(ActionInput input);

    public abstract bool IsBusy();

}
