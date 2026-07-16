using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeUbicationFadeOut : MonoBehaviour
{
    int sceneIndex = 2;
    public void FadeIn()
    {
        if (TutorialBool.finishTutorial)
        {
            SceneManager.LoadScene("BlockoutNivelBar");
            TutorialBool.finishTutorial = false;
            return;
        }
        if (TutorialBool.changeLevel)
        {
            sceneIndex++;
            SceneManager.LoadScene(sceneIndex);
            return;
        }
        Waves.Instance.player.transform.position = Waves.Instance.startPointLevel2.transform.position;
        Waves.Instance.player.transform.rotation = Waves.Instance.startPointLevel2.transform.rotation;
        Waves.Instance.changeUbicationAnim.SetTrigger("UbicationChanged");
        Time.timeScale = 1;
    }
    public void FadeOut()
    {
        Time.timeScale = 1;
        Waves.Instance.WaitingForChangeUbication(false);
    }
}
