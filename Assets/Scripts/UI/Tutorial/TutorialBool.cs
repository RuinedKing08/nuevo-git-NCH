using UnityEngine;

public class TutorialBool : MonoBehaviour
{
    [SerializeField] bool thisIsALevel;
    public static bool tutorial;
    public static bool finishTutorial;
    public static bool changeLevel;
    void Awake()
    {
        if (thisIsALevel)
        {
            tutorial = false;
            finishTutorial = false;
        }
        else
        {
            ChangeUbicationFadeOut.sceneIndex = 2;
            tutorial = true;
            finishTutorial = true;
        }
    }

}
