using UnityEngine;

public class ExplotionDuration : MonoBehaviour
{
    [SerializeField] private bool timeOfExplosion;
    [SerializeField] private float timer, maxTimer, timerExplotion;
    [SerializeField] private GameObject colli;
    [SerializeField] private GameObject blood;
    [SerializeField] private SphereCollider colliderExplotion;

    void Start()
    {
        
        timeOfExplosion = true;
        timer = 0;
        colli.SetActive(false);
        blood.SetActive(false);
    }

    void Update()
    {
        if (timeOfExplosion)
        {
            TimeExplosion();
        }

    }
    private void FixedUpdate()
    {
        
    }

    private void TimeExplosion()
    {
        timer += Time.deltaTime;
        if(timer >= timerExplotion)
        {
            colli.SetActive(true);
            blood.SetActive(true);
        }
        if(timer >= timerExplotion + 0.5f && timer < timerExplotion + 1.3f)
        {
            colliderExplotion.radius = 4;
        }
        if(timer >= timerExplotion + 1.3f)
        {
            colliderExplotion.radius = 3;
        }
        if (timer >= maxTimer)
        {
            Destroy(gameObject);
            timeOfExplosion = false;
        }
    }
}
