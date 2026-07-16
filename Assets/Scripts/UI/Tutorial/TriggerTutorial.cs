using UnityEngine;

public class TriggerTutorial : MonoBehaviour
{
    [SerializeField] bool tutorial;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EnemyDetect") && tutorial)
        {
            DialoguesTutorial.Instance.ActivePanel();
            Time.timeScale = 0f;
            tutorial = false;
        }
    }
}
