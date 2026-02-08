using UnityEngine;

  
[CreateAssetMenu(fileName = "GameSettings", menuName = "Scriptable Objects/GameSettings")]
public class GameSettings : ScriptableObject
{
    static GameSettings instance;

    public static GameSettings Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Resources.Load<GameSettings>("GameSettings");

                if (instance == null)
                {
                    Debug.LogError(
                        "GameSettings not found! " +
                        "Create one and place it in a Resources folder."
                    );
                }
            }

            return instance;
        }
    }
    [Header("Input")]
    public InputSettings lockOnSettings;

    public float buttonHoldTreshhold;



}
