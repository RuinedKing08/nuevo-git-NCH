using UnityEngine;

public class RagdollTest : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Collider mainCollider;
    [SerializeField] private Rigidbody mainRigidbody;

    private Rigidbody[] ragdollRigidbodies;
    private Collider[] ragdollColliders;

    private void Awake()
    {
        ragdollRigidbodies = GetComponentsInChildren<Rigidbody>();
        ragdollColliders = GetComponentsInChildren<Collider>();

        SetRagdoll(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y))
        {
            FuckingDie();
        }
    }

    public void SetRagdoll(bool active)
    {
        animator.enabled = !active;

        if (mainCollider != null)
            mainCollider.enabled = !active;

        if (mainRigidbody != null)
            mainRigidbody.isKinematic = active;

        foreach (Rigidbody rb in ragdollRigidbodies)
        {
            if (rb == mainRigidbody) continue;
            rb.isKinematic = !active;
        }

        foreach (Collider col in ragdollColliders)
        {
            if (col == mainCollider) continue;
            col.enabled = active;
        }
    }

    public void FuckingDie()
    {
        SetRagdoll(true);
    }


}
