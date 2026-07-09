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
    public static bool dodgeInCooldown;
    bool startTimer;
    float timer;
    [SerializeField] float radius;
    void Start()
    {
        _mainCamera = Camera.main;

        UniqueUpgrades.margaritaBuff = true;
        UniqueUpgrades.margaritaAmount += 2;
        dodgeInCooldown = false;
        isDodging = false;

        ultraPushActivated = false;

        //InputsParent.Instance.BlockInput().performed += ctx => StartBlock();
        //InputsParent.Instance.BlockInput().canceled += ctx => EndBlock();
        InputsParent.Instance.SideStepInput().performed += ctx => Dodge();
        InputsParent.Instance.LockOnInput().performed += ctx => UltraPush();
        
    }

    void Update()
    {
        LookWithCamera();
        CooldownDodge();
    }
    void CooldownDodge()
    {
        if (startTimer)
        {
            timer += Time.deltaTime;
            if (!dodgeInCooldown)
            {
                timer = 0;
                startTimer = false;
            }
            if(timer >= 1)
            {
                dodgeInCooldown = false;
                startTimer = false;
                timer = 0;
            }
        }
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
        if (dodgeInCooldown) return;
        _combatSystem.PlayDodge();
        DodgeTest.Instance.Activate();
        AudioManager.Instance.Play(_combatSystem.DodgeSound, transform.position);
        dodgeInCooldown = true;
    }

    
    bool ultraPushActivated;
    void UltraPush()
    {
        if (!UniqueUpgrades.margaritaBuff) return;
          
        
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius);
        foreach(Collider hit in hitColliders)
        {
            AttackColliderHandler enemy = hit.GetComponent<AttackColliderHandler>();
            if(enemy != null)
            {
                ultraPushActivated = true;
                DJVFX.Instance.PlayPush();
                
                StartCoroutine(enemy.PushBackEnemy());
            }
        }
        if(ultraPushActivated) 
        {
            UniqueUpgrades.margaritaAmount--;
            _combatSystem.PlayPush(); 
        }
        if (UniqueUpgrades.margaritaAmount <= 0)
        {
            UniqueUpgrades.margaritaAmount = 0;
            UniqueUpgrades.margaritaBuff = false;
        }
        ultraPushActivated = false; 
    }
    public void AE_StartDodge() => isDodging = true;
    public void AE_EndDodge() { isDodging = false; startTimer = true; }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
