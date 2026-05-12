using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] GameObject hitCollider;
    [Header("Input Values")]
    [SerializeField] public float lightAttackfloat;

    
    void Start()
    {
        hitCollider.SetActive(false);
    }
    [SerializeField] float timer;
    bool timerOn;
    void Update()
    {
        GetInput();
        Attack();
    }
    private void Attack()
    {
        if (lightAttackfloat > 0)
        {
            hitCollider.SetActive(true);
            timerOn = true;
        }
        if (timerOn)
        {
            timer += Time.deltaTime;
        }
        if (timer >= 0.4f)
        {
            hitCollider.SetActive(false);
            timer = 0;
            timerOn = false;
        }
    }
    private void GetInput()
    {
        lightAttackfloat = InputsParent.Instance.LightAttackInput().ReadValue<float>();
    }
}
