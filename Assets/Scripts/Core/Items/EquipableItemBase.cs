using UnityEngine;

public abstract class EquipableItemBase : ItemBase, IEquipable
{
    [SerializeField] protected GameObject ThrowWeapon;
    public WeaponSettings weaponSettings;
    protected PlayerActionController controller;

    protected bool _requestedAttack;

    protected bool _requestedThrow;



    Ray ray;
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

    public virtual void Throw()
    {
        GameObject obj = Instantiate(ThrowWeapon, controller.weaponBone.transform.position, Quaternion.identity);
        obj.GetComponent<Rigidbody>().AddForce(CalculateAimDirection(controller.weaponBone.position) * weaponSettings.ThrowForce, ForceMode.Impulse);


        controller.ThrowWeapon();
        Destroy(gameObject);


    }

    public virtual Vector3 CalculateAimDirection(Vector3 initPos)
    {
        Vector3 targetpoint;
        if (Physics.Raycast(ray, out RaycastHit hit, weaponSettings.maxDistance, weaponSettings.aimMask))
        {
            targetpoint = hit.point;
        }
        else
        {
            targetpoint = ray.origin + ray.direction * weaponSettings.maxDistance;
        }
        Vector3 Direc = (targetpoint - initPos).normalized;

        return Direc;
    }


    public void receiveRay(Ray ray)
    {
        this.ray = ray;

    }
}
