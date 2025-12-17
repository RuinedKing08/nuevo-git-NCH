using UnityEngine;

public class PlayerActionController : MonoBehaviour
{
    #region References

    Transform weaponBone;

    #endregion

    public void Initialize(Transform weaponBone)
    {
        this.weaponBone = weaponBone;
    }



}
