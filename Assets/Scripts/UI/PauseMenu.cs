using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject controlsPanel;
    [SerializeField] GameObject optionsPanel;
    [SerializeField] TMP_Text optionsTMP;
    bool options, pause;

    private void Awake()
    {
        Time.timeScale = 0;
    }
    void Start()
    {
        Time.timeScale = 0; 
        controlsPanel.SetActive(true);
        optionsPanel.SetActive(false);
        optionsTMP.text = "Opciones";
        options = false;
        pause = true;
        InputsParent.Instance.PauseInput().performed += Pause;
    }

    void Pause(InputAction.CallbackContext context)
    {
        if (pause) return;

        Time.timeScale = 0;
        pauseMenu.SetActive(true);
        pause = true;
    }
    public void Play()
    {
        pauseMenu.SetActive(false);
        pause = false;
        if (ChangeCards.cardsOpened) return;
        Time.timeScale = 1;
    }

    public void PanelManager()
    {
        if (!options)
        {
            controlsPanel.SetActive(false);
            optionsPanel.SetActive(true);
            optionsTMP.text = "Controles";
            options = true;
        }
        else
        {
            controlsPanel.SetActive(true);
            optionsPanel.SetActive(false);
            optionsTMP.text = "Opciones";
            options = false;
        }
    }

    public void MenuPrincipal()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("StartMenu");
    }

}
