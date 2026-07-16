using UnityEngine;

public class TutorialBool : MonoBehaviour
{
    [SerializeField] bool thisIsALevel;
    public static bool tutorial;
    public static bool finishTutorial;
    public static bool changeLevel;
    void Start()
    {
        if (thisIsALevel)
        {
            tutorial = false;
            finishTutorial = false;
        }
        else
        {
            tutorial = true;
            finishTutorial = true;
        }
    }

}
