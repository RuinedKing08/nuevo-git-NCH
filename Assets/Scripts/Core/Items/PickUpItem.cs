using UnityEngine;

public class PickUpItem : MonoBehaviour , IInteractable
{
    [SerializeField] ItemBase itemPrefab;

    public bool CanInteract(PlayerActionController controller)
    {
        return controller.CanInteract();
    }

    public void Interact(PlayerActionController controller)
    {
        controller.OnEquipItem(Instantiate(itemPrefab));
        Destroy(gameObject);
    }
}
