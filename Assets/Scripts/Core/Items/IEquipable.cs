using UnityEngine;

public interface IEquipable 
{
    public void OnEquip(PlayerActionController controller);
    public void OnUnequip();
    void ProcessInput(ActionInput input);
    bool IsBusy();
}
