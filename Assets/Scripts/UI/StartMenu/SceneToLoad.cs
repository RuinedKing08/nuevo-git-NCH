using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneToLoad : MonoBehaviour
{
    public static string sceneSaved = "BlockoutNivelBar";

    private void Awake()
    {
        sceneSaved = SceneManager.GetActiveScene().name;
    }
}
