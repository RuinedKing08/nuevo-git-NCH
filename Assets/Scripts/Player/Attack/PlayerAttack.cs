using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] GameObject hitCollider;
    [SerializeField] GameObject hitColliderHeavy;
    [Header("Input Values")]
    [SerializeField] public float lightAttackfloat;
    [SerializeField] public float heavyAttackfloat;
    float timerLight, timerHeavy;

    void Start()
    {
        hitCollider.SetActive(false);
    }
    
    bool timerOn;
    void Update()
    {
        GetInput();
        LightAttack();
    }
    private void LightAttack()
    {
        if (lightAttackfloat > 0)
        {
            hitCollider.SetActive(true);
            timerOn = true;
        }
        if (timerOn)
        {
            timerLight += Time.deltaTime;
            if (timerLight >= 0.4f)
            {
                hitCollider.SetActive(false);
                timerLight = 0;
                timerOn = false;
            }
        }
    }
    private void HeavyAttack()
    {
        if (heavyAttackfloat > 0)
        {
            hitColliderHeavy.SetActive(true);
            timerOn = true;
        }
        if (timerOn)
        {
            timerHeavy += Time.deltaTime;

            if (timerHeavy >= 0.4f)
            {
                hitColliderHeavy.SetActive(false);
                timerHeavy = 0;
                timerOn = false;
            }
        }
    }
    private void GetInput()
    {
        lightAttackfloat = InputsParent.Instance.LightAttackInput().ReadValue<float>();
        heavyAttackfloat = InputsParent.Instance.HeavyAttackInput().ReadValue<float>();
    }
}
