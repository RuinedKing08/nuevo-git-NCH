using UnityEngine;

public class ArrowController : MonoBehaviour
{
     public static bool isCursorUnlocked;

    void Start()
    {
        LockCursor();
    }

    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            UnlockCursor();
        }

        if (Input.GetKeyUp(KeyCode.LeftAlt))
        {
            LockCursor();
        }

        
        if (Time.timeScale == 0 && !isCursorUnlocked)
        {
            UnlockCursor();
        }
    }

    public void UnlockCursor()
    {
        isCursorUnlocked = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void LockCursor()
    {
        isCursorUnlocked = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
