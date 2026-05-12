using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] GameObject hitCollider;
    [SerializeField] GameObject hitColliderHeavy;
    [Header("Input Values")]
    [SerializeField] public float lightAttackfloat;
    [SerializeField] public float heavyAttackfloat;

    
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
            float timer = 0;
            timer += Time.deltaTime;
            if (timer >= 0.4f)
            {
                hitCollider.SetActive(false);
                timer = 0;
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
            float timer = 0;
            timer += Time.deltaTime;

            if (timer >= 0.4f)
            {
                hitColliderHeavy.SetActive(false);
                timer = 0;
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
