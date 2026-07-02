using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using Unity.Cinemachine;
public class PlayerActions : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private PlayerCombatSystem _combatSystem;
    [SerializeField] private Camera _mainCamera;

    [Header("Configuración de Rotación")]
    [SerializeField] private float rotateSpeed = 10f;
    
    public static bool blocking;
    public static bool isDodging; 

    [SerializeField] float radius;
    void Start()
    {
        _mainCamera = Camera.main;

        UniqueUpgrades.margaritaBuff = true;
        UniqueUpgrades.margaritaAmount = 2;

        //InputsParent.Instance.BlockInput().performed += ctx => StartBlock();
        //InputsParent.Instance.BlockInput().canceled += ctx => EndBlock();
        InputsParent.Instance.SideStepInput().performed += ctx => Dodge();
        InputsParent.Instance.LockOnInput().performed += ctx => UltraPush();
    }

    void Update()
    {
        LookWithCamera();
    }

    void LookWithCamera()
    {
        if (Cursor.lockState == CursorLockMode.None || blocking || _mainCamera == null) return;

        Vector3 camForward = _mainCamera.transform.forward;
        camForward.y = 0;

        if (camForward.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(camForward);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
        }
    }

    void StartBlock()
    {
        blocking = true;
        _combatSystem.SetBlockingState(true);
    }

    void EndBlock()
    {
        blocking = false;
        _combatSystem.SetBlockingState(false);
    }

    void Dodge()
    {
        if (isDodging) return; 
        _combatSystem.PlayDodge();
        AudioManager.Instance.Play(_combatSystem.DodgeSound, transform.position);
    }
    void UltraPush()
    {
        if (!UniqueUpgrades.margaritaBuff) return;
        
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius);
        foreach(Collider hit in hitColliders)
        {
            AttackColliderHandler enemy = hit.GetComponent<AttackColliderHandler>();
            if(enemy != null)
            {
                StartCoroutine(enemy.PushBackEnemy());
                UniqueUpgrades.margaritaAmount--;
                if (UniqueUpgrades.margaritaAmount <= 0)
                {
                    UniqueUpgrades.margaritaAmount = 0;
                    UniqueUpgrades.margaritaBuff = false;
                }
            }
        }
    }
    public void AE_StartDodge() => isDodging = true;
    public void AE_EndDodge() => isDodging = false;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
