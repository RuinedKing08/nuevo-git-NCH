using UnityEngine;

public class ChangeUbicationFadeOut : MonoBehaviour
{
    public void FadeIn()
    {
        Waves.Instance.player.transform.position = Waves.Instance.startPointLevel2.transform.position;
        Waves.Instance.player.transform.rotation = Waves.Instance.startPointLevel2.transform.rotation;
        Waves.Instance.changeUbicationAnim.SetTrigger("UbicationChanged");
        Time.timeScale = 1;
    }
    public void FadeOut()
    {
        Waves.Instance.WaitingForChangeUbication(false);
    }
}
