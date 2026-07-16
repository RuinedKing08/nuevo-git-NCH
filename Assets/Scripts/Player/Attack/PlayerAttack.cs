using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{    
    private PlayerCombatSystem _combatSystem;

    private void Awake()
    {
        _combatSystem = GetComponent<PlayerCombatSystem>();
    }

    void Update()
    {        
        if (InputsParent.Instance.LightAttackInput().triggered)
        {
            if (TutorialBool.tutorial)
            {
                Tutorial();
                return;
            }
            if(Time.timeScale == 1f) _combatSystem.PerformLightAttack();           
            
        }
    }
    void Tutorial()
    {
        if(DialoguesTutorial.Instance.dialogueText.text == DialoguesTutorial.Instance.actualLines[0])
        {
            Debug.Log("Atataaatata");
            _combatSystem.PerformLightAttack();
            DialoguesTutorial.Instance.NextDialogueLine();
            Time.timeScale = 1f;
        }
    }
}
