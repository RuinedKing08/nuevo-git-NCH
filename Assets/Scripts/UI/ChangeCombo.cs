using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Playables;
public class ChangeCombo : MonoBehaviour
{
    public static ChangeCombo Instance;
    [SerializeField] int combo;
    PlayableDirector playable;
    bool resetCombo, resetIcon;
    GameObject comboGameObject;
    Image playerIcon;
    TMP_Text comboTMP;
    bool firstStart;
    float timerIcon;
    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        playable = GetComponent<PlayableDirector>();
        comboGameObject = transform.Find("ComboTMP").gameObject;
        playerIcon = transform.Find("PlayerFace").GetComponent<Image>();
        comboTMP = comboGameObject.GetComponent<TMP_Text>();
        resetCombo = true;
        resetIcon = false;
        TimerText();
        Combo(resetCombo);
        comboTMP.text = "x" + combo;
    }

    void Update()
    {
        TimerText();
    }

    public void Combo(bool resetCombo)
    {
        this.resetCombo = resetCombo;
        if (this.resetCombo)
        {
            combo = 0;
            if (!firstStart) resetIcon = false;
            else resetIcon = this.resetCombo;
            ChangeComboTMP();
        }
        else
        {
            combo++;
            if(combo >= 2) ChangeComboTMP();
        }
    }
    void ChangeComboTMP()
    {
        comboGameObject.SetActive(true);
        comboTMP.text = "x" + combo;
        playable.Play();
    }
    void TimerText()
    {
        if (!resetCombo)
        {
            playerIcon.color = Color.white;
        }
        else
        {
            comboGameObject.SetActive(false);
            if (resetIcon)
            {
                timerIcon += Time.deltaTime;
                playerIcon.color = Color.red;
                if (timerIcon >= 0.6f)
                {
                    playerIcon.color = Color.white;
                    resetIcon = false;
                    timerIcon = 0;
                }
            }
            else
            {
                playerIcon.color = Color.white;
                resetIcon = true;
                resetCombo = false;
            }
           
        }
        
    }
}
