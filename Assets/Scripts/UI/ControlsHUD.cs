using UnityEngine;

public class ControlsHUD : MonoBehaviour
{
    [SerializeField] GameObject controlsButton;
    [SerializeField] GameObject closeControlsButton;
    private void Start()
    {
        closeControlsButton.SetActive(false);
        controlsButton.SetActive(true);
    }
    public void OpenControlsHUD()
    {
        controlsButton.SetActive(false);
        closeControlsButton.SetActive(true);
    }
    public void CloseControlsHUD()
    {
        closeControlsButton.SetActive(false);
        controlsButton.SetActive(true);
    }
}
