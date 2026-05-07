using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    InputsParent inputs;
    [SerializeField] GameObject hitCollider;
    [Header("Input Values")]
    [SerializeField] public float attackfloat;

    
    void Start()
    {
        hitCollider.SetActive(false);
        inputs = GameObject.FindWithTag("Player").GetComponent<InputsParent>();
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
        if (attackfloat > 0)
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
        attackfloat = inputs.AttackInput().ReadValue<float>();
    }
}
