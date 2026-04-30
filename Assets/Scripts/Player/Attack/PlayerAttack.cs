using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] GameObject hitCollider;
    void Start()
    {
        hitCollider.SetActive(false);
    }
    [SerializeField] float timer;
    bool timerOn;
    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            hitCollider.SetActive(true);
            timerOn = true;
        }
        if (timerOn)
        {
            timer += Time.deltaTime;
        }
        if(timer >= 0.4f)
        {
            hitCollider.SetActive(false);
            timer = 0;
            timerOn = false;
        }
    }
}
