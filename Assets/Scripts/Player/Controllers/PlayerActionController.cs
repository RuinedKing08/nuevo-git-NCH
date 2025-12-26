using UnityEngine;

public class PlayerActionController : MonoBehaviour
{
    #region References

    public Transform weaponBone;
    private PlayerState _tempState;
    #endregion
    IEquipable equippedItem;

    private PlayerActionState actionStateInternal;
    public PlayerActionState actionState => actionStateInternal;

    public InteractSettings _InteractSettings;


    public IInteractable currentInteractable;
    public void Detect(Transform camPos)
    {


        Ray ray = new Ray(camPos.position, camPos.forward);

        equippedItem?.receiveRay(ray);

        RaycastHit[] hits = Physics.SphereCastAll(
            ray,
            _InteractSettings.sphereRadius,
            _InteractSettings.maxDistance,
            _InteractSettings.interactableMask);

        currentInteractable = ChooseBest(hits, camPos);
    }

    public IInteractable ChooseBest(RaycastHit[] hits, Transform camPos)
    {
        float bestScore = 0f;
        IInteractable best = null;

        foreach (var hit in hits)
        {
            var interactable = hit.collider.GetComponent<IInteractable>();
            if (interactable == null)
            {
                Debug.Log("InsInteractable");
                continue;
            }

            Vector3 dir = (hit.point - camPos.position).normalized;
            float angle = Vector3.Dot(camPos.transform.forward, dir); // -1 a 1
            float distanceScore = 1f - (hit.distance / _InteractSettings.maxDistance);

            float score = angle * 0.7f + distanceScore * 0.3f;

            if (score > bestScore)
            {
                bestScore = score;
                best = interactable;
            }
        }


        return best;
    }
    public void Initialize()
    {


       
    }

    public void OnEquipItem(ItemBase item)
    {
        UnEquip();

        var equipable = item as IEquipable;
        if (equipable == null) return;

        Debug.Log("Equipped Item");

        item.transform.SetParent(weaponBone);
        item.transform.localPosition = Vector3.zero;
        item.transform.localRotation = weaponBone.rotation;

        equippedItem = equipable;
        equippedItem.OnEquip(this);
        

        actionStateInternal.HoldingState = item is FireArmWeapon ? Holding.FireArm :
                                           item is MeleeWeapon ? Holding.MeleeItem :
                                           Holding.None;

    }
    public void UnEquip()
    {
        actionStateInternal.HoldingState = Holding.None;
        if (equippedItem != null)
        {
            equippedItem.OnUnequip();

        }
        equippedItem = null;
    }
    public void ThrowWeapon()
    {
        UnEquip();
    }
    public void ProcessInput(ActionInput input)
    {

        equippedItem?.ProcessInput(input);

        if (input.Interact)
        {
            TryInteract();
        }
    }

    public bool CanInteract()
    {
        if (equippedItem == null) return true;
        else return !equippedItem.IsBusy();
    }

    public void TryInteract()
    {
        if (!CanInteract()) return;

        if (currentInteractable == null) return;



        if (currentInteractable.CanInteract(this))
        {
            currentInteractable.Interact(this);
        }
    }

    public void SetActionState(ActionState newState)
    {
        actionStateInternal.actionState = newState;
    }


}
