using UnityEngine;

public class ExplotionCollider : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EnemyDetect"))
        {
            other.GetComponent<EnemyController>().InvokeOnDead();
            other.GetComponent<EnemyController>().SetRagdoll(true);
            other.GetComponent<EnemyController>().PushRagdoll(new Vector3(0,0,0), 0);
        }
    }
}
