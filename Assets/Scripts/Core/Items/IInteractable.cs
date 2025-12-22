using UnityEngine;

public interface IInteractable 
{
    bool CanInteract(PlayerActionController controller);

    void Interact(PlayerActionController controller);
}
