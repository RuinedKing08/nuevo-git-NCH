using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("Configuración")]
    public GameObject prefabASpawner;
    public float interval = 11f;

    void Start()
    {
        InvokeRepeating(nameof(Spawn), 0f, interval);
    }

    void Spawn()
    {
        if (prefabASpawner == null)
        {
            Debug.LogWarning("No prefab available");
            return;
        }

        Instantiate(prefabASpawner, transform.position, transform.rotation);
    }
}
