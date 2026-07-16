using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Cinemachine;
using UnityEngine.Playables;

public class DialoguesTutorial : MonoBehaviour
{
    public TMP_Text dialogueText;
    public TMP_Text nameText;
    [SerializeField] private GameObject panelDialogues;
    [TextArea(4, 6)] public string[] actualLines;
    [TextArea(4, 6)] public string[] nameLines;
    public float textSpeed = 0.02f;

    public int index;

    public bool didDialogueStart;
    bool canChangeLines;
    public static DialoguesTutorial Instance;
    private void Awake()
    {
        Instance = this;
    }
    private void OnEnable()
    {
        didDialogueStart = false;
    }
    private void Start()
    {
        index = 0;
        panelDialogues.SetActive(false);
        ChangeName();
    }
    public void FixedUpdate()
    {
        if (Time.timeScale == 0f || Time.timeScale == 1f)
        {
            if (!didDialogueStart && panelDialogues.activeInHierarchy)
            {
                StartDialogue();
            }
            /*else if (dialogueText.text == actualLines[index])
            {
                NextDialogueLine();
            }*/
            if ((Input.GetMouseButton(0) || Input.GetMouseButton(1)) && canChangeLines)
            {
                NextDialogueLine();
            }
            if (dialogueText.text == actualLines[index])
            {

            }
        }
    }
    public void ActivePanel()
    {
        panelDialogues.SetActive(true);
    }
    public void DeActivePanel()
    {
        panelDialogues.SetActive(false);
    }
    public void StartDialogue()
    {
        didDialogueStart = true;
        if (Time.timeScale == 0f || Time.timeScale == 1f)
        {
            StartCoroutine(WriteLine());
        }
    }
    void ChangeName()
    {
        switch (index)
        {
            case 0:
                nameText.text = nameLines[0];
                break;
            case 1:
                nameText.text = nameLines[1];
                break;
        }        

    }
    public void PauseTimeLine()
    {
        canChangeLines = true;
    }
    public void NextDialogueLine()
    {
        canChangeLines = false;
        index++;
        ChangeName();
        if (index < actualLines.Length)
        {
            StartCoroutine(WriteLine());
        }

        if (index >= actualLines.Length)
        {
            Finish();
        }
    }
    public void Finish()
    {
        index = 0;
        StopAllCoroutines();
        DeActivePanel();
    }

    private IEnumerator WriteLine()
    {
        if (Time.timeScale == 0f || Time.timeScale == 1f)
        {
            dialogueText.text = string.Empty;
            foreach (char letter in actualLines[index].ToCharArray())
            {
                dialogueText.text += letter;
                yield return new WaitForSecondsRealtime(textSpeed);
            }
        }
    }
}
