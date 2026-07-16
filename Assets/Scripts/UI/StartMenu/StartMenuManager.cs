using UnityEngine;
using UnityEngine.SceneManagement;
public class StartMenuManager : MonoBehaviour
{
    [SerializeField] GameObject startMenuPanel;
    [SerializeField] GameObject optionsPanel;
    [SerializeField] GameObject creditsPanel;
    [SerializeField] string sceneToLoad;
    [SerializeField] AudioData MenuMusic;
     private void Awake()
    {
        AudioManager.Instance.PlayLooped(MenuMusic);
    }
    private void Start()
    {

        startMenuPanel = transform.Find("StartMenuPanel").Find("MenuPanel").gameObject;
        optionsPanel = transform.Find("OptionsPanel").gameObject;
        creditsPanel = transform.Find("CreditsPanel").gameObject;
        startMenuPanel.SetActive(true);
        optionsPanel.SetActive(false);
        sceneToLoad = SceneToLoad.sceneSaved;
    }
    public void PlayButton()
    {
        UniqueUpgrades.Instance.DisabelAll();
        SceneManager.LoadScene(sceneToLoad);
        AudioManager.Instance.StopLoopedSound();
    }

    
    public void OptionsButton()
    {
        startMenuPanel.SetActive(false);
        optionsPanel.SetActive(true);
    }
   
    public void ReturnOptionsButton()
    {
        startMenuPanel.SetActive(true);
        optionsPanel.SetActive(false);
    }
    public void CreditsButton()
    {
        startMenuPanel.SetActive(false);
        creditsPanel.SetActive(true);
    }
    public void ReturnOptionsCreditsButton()
    {
        startMenuPanel.SetActive(true);
        creditsPanel.SetActive(false);
    }

    public void ExitButton()
    {
        Application.Quit();
    }
}
