using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
public class Waves : MonoBehaviour
{
    [SerializeField] private SpawnPointsForEnemies[] spawnPoints;
    [SerializeField] private List<GameObject> enemiesToSpawn;
    [SerializeField] private List<GameObject> enemiesGroup1;
    [SerializeField] private List<GameObject> enemiesGroup2;
    [SerializeField] private List<GameObject> enemiesGroup3;
    [SerializeField] private List<GameObject> enemiesGroup4;
    [SerializeField] private List<GameObject> enemiesGroup5;
    [SerializeField] private List<GameObject> prefabsBoss;
    [SerializeField] private int wave;
    [SerializeField] private int wavesInWave;
    [SerializeField] private int wavesWaiting;
    [SerializeField] private int amountToSpawn;
    [SerializeField] float timerInWave;
    [SerializeField] float maxTimerInWave;
    [SerializeField] float timerOutWave;
    [SerializeField] float maxTimerOutWave;
    [SerializeField] float timerLevel;
    TMP_Text waveTMP, enemiesLeftTMP;
    Animator waveAnim;
    float timerSpawn;
    float timerWaveWaiting;
    float maxTimer;
    float timeBetweenSpawn;
    Coroutine coroutine;
    bool inWave, spawnEnemy, waveWaiting, firstWaveWaiting, startSpawn;
    bool newWave;
    int indexEnemies;
    public int indexSpawn;
    int group, enemiesLeftCount;
    int index1, index2, index3, index4, index5, indexTotal;
    public static Waves Instance;
    public event ChangeWaves OnChangeWave;
    public delegate void ChangeWaves();
    public static bool bossSpawned;
    bool spawnBoss;
    bool extraAmountEnemies;
    int indexBosses;
    [SerializeField] GameObject bossAlert;
    [SerializeField] float timeToSpawnBoss;
    [HideInInspector] public GameObject player;
    [HideInInspector] public GameObject startPointLevel2;
    GameObject changeUbicationPanel;
    [HideInInspector] public Animator changeUbicationAnim;

    [SerializeField] bool tutorial;
    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        spawnPoints = transform.Find("SpawnPoints (1)").GetComponentsInChildren<SpawnPointsForEnemies>();
        waveTMP = GameObject.Find("Canvas").transform.Find("WaveTMP").GetComponent<TMP_Text>();
        enemiesLeftTMP = GameObject.Find("Canvas").transform.Find("EnemiesLeftTMP").GetComponent<TMP_Text>();
        waveAnim = GameObject.Find("Canvas").GetComponent<Animator>();
        player = GameObject.Find("PlayerGo");
        startPointLevel2 = GameObject.Find("StartPointLevel2");
        changeUbicationPanel = GameObject.Find("ChangeUbicationPanel");
        changeUbicationAnim = changeUbicationPanel.GetComponent<Animator>();
        changeUbicationPanel.SetActive(false);
        wave = 0;
        startSpawn = false;
        extraAmountEnemies = false;
        Coroutine();
        ChangeWave();
        NewWave();
        indexSpawn = Random.Range(0, spawnPoints.Length);
        inWave = false;
        firstWaveWaiting = true;
        EnemyCombatGroup.Instance.OnChangeCurrentMembers += ChangeInWave;
        //EnemyController.OnDead += ChangeWave;
        ChangeCards.Instance.OnChangeWave += NewWave;
    }

    void FixedUpdate()
    {
        CloseSpawn();
        TimerToSpawn();
    }
    public void InvokeChangeWave() { OnChangeWave?.Invoke(); }
    public int GetWave() { return wave; }
    void ChangeEnemiesLeftTMP()
    {
        indexTotal--;
        if (indexTotal <= 0) indexTotal = 0;
        enemiesLeftTMP.text = ($"Enemigos Restantes: {indexTotal}");
    }
    void StarSpawnTrue()
    {
        startSpawn = true;
    }
    bool cM = false;
    void TimerToSpawn()
    {
        timerLevel += Time.fixedDeltaTime;
        float tL = Mathf.Round(timerLevel);
        tL = Mathf.Clamp(tL, 1, timerLevel + 1);
        
        if (tL % 30 == 0)
        {
            cM = true;
        }

        if (cM)
        {
            Currency.Instance.ChangeMoney(2f / 50, 600 / 50);
            cM = false;
        }
        //InWave();
    }
    void InWave()
    {
        /*if (inWave)
        {
            maxTimer = maxTimerInWave;
            timerInWave -= Time.fixedDeltaTime;
            if (timerInWave <= 0)
            {
                if(EnemyCombatGroup.Instance.GetCurrentMembers().Count < 5 && !waveWaiting)
                {
                    ChoseGruopToSpawn();
                    StarSpawnTrue();
                    timerInWave = maxTimer;
                    inWave = true;
                }
                else if(EnemyCombatGroup.Instance.GetCurrentMembers().Count < 5 && waveWaiting && !firstWaveWaiting)
                {
                    wavesWaiting--;
                    ChangeWavesWating();
                    ChoseGruopToSpawn();
                    StarSpawnTrue();
                    timerInWave = maxTimer;
                    inWave = true;
                }
                else if(EnemyCombatGroup.Instance.GetCurrentMembers().Count < 5 && waveWaiting && firstWaveWaiting)
                {
                    wavesWaiting++;
                    ChangeWavesWating();
                    timerInWave = maxTimer;
                }
                else if(EnemyCombatGroup.Instance.GetCurrentMembers().Count >= 5)
                {
                    wavesWaiting++;
                    ChangeWavesWating();
                    firstWaveWaiting = true;
                    timerInWave = maxTimer;
                }
            }
        }
        else
        {
            if (!waveWaiting)
            {
                maxTimer = maxTimerOutWave;
                timerOutWave -= Time.fixedDeltaTime;
                if (timerOutWave <= 0)
                {
                    ChoseGruopToSpawn();
                    StarSpawnTrue();
                    timerOutWave = maxTimer;
                    inWave = true;
                }
            }
            else
            {
                timerWaveWaiting += Time.fixedDeltaTime;
                if(timerWaveWaiting >= 0.5f)
                {
                    wavesWaiting--;
                    ChangeWavesWating();
                    ChoseGruopToSpawn();
                    StarSpawnTrue();
                    firstWaveWaiting = false;
                    timerWaveWaiting = 0;
                    inWave = true;
                }
            }
            
        }*/
    }
    void ChangeWavesWating()
    {
        if (wavesWaiting <= 0)
        {
            waveWaiting = false;
            wavesWaiting = 0;
        }
        //else waveWaiting = true;
    }
    public void ChangeWave()
    {
        ChangeEnemiesLeftTMP();
        enemiesLeftCount++;
        switch (wavesInWave)
        {
            case 1:
                if (enemiesLeftCount >= index1) enemiesLeftCount = 0;
                else return;
                break;
            case 2:
                if (enemiesLeftCount >= index2) enemiesLeftCount = 0;
                else return;
                break;
            case 3:
                if (enemiesLeftCount >= index3) enemiesLeftCount = 0;
                else return;
                break;
            case 4:
                if (enemiesLeftCount >= index4) enemiesLeftCount = 0;
                else return;
                break;
            case 5:
                if (enemiesLeftCount >= index5) enemiesLeftCount = 0;
                else return;
                break;
            default:
                if (enemiesLeftCount >= index1) enemiesLeftCount = 0;
                else return;
                break;
        }
        wavesInWave--;
        if (indexTotal > 0 && wavesInWave > 0)
        {
            ChoseGruopToSpawn();
        }
        if (wavesInWave <= 0)
        {
            ChangeBoolNewWave(true);
            InvokeChangeWave();
        }
    }
    bool waitingForChangeUbication, waveTitleAnimated;
    public void WaitingForChangeUbication(bool active)
    {
        waitingForChangeUbication = active;
        if (!active)
        { 
            Time.timeScale = 1;
            ContinueNewWave();
        }
        changeUbicationPanel.SetActive(active);
    }
    void NewWave()
    {
        //Time.timeScale = 1;
        wave++;
        if (wave == 11)
        {
            WaitingForChangeUbication(true);
            Time.timeScale = 0;
            waveTMP.text = ($"Oleada {wave}");
            waveAnim.Play("WaveTitle");
            waveTitleAnimated = true;
            spawnPoints = transform.Find("SpawnPoints (2)").GetComponentsInChildren<SpawnPointsForEnemies>();
            changeUbicationAnim.SetTrigger("StartChange");
        }
        //while(waitingForChangeUbication){ }
        if (!waitingForChangeUbication) { waveTitleAnimated = false; ContinueNewWave(); }
    }
    [SerializeField] int waveInWaveEditableMin, waveInWaveEditableMax;
    void ContinueNewWave()
    {
        if (!waveTitleAnimated)
        {
            waveTMP.text = ($"Oleada {wave}");
            waveAnim.Play("WaveTitle");
        }
        if (tutorial)
        {
            wavesInWave = Random.Range(waveInWaveEditableMin, waveInWaveEditableMax);
        }
        else wavesInWave = Random.Range(1, 4);

        AmountOfGroupsToSpawn(wavesInWave);
        enemiesLeftTMP.text = ($"Enemigos Restantes: {indexTotal}");
        ChangeBoolNewWave(false);
        ChoseGruopToSpawn();
        if (wave % 5 == 0 && wave != 0)
        {
            extraAmountEnemies = true;
            ActivateBoss();
        }
    }
    void ActivateBoss()
    {
        indexTotal += 1;
        enemiesLeftCount--;
        bossSpawned = true;
        spawnBoss = true;
        StartCoroutine(SpawnBoss());
    }
    IEnumerator SpawnBoss()
    {
        bossAlert.SetActive(true);
        yield return new WaitForSeconds(timeToSpawnBoss);
        ChooseEnemiesToSpawn(prefabsBoss);
        SpawnB(prefabsBoss);
        bossAlert.SetActive(false);
        yield return null;
    }
    void ChangeBoolNewWave(bool newWave)
    {
        this.newWave = newWave;
    }
    public bool GetNewWaveBool()
    {
        return newWave;
    }
    void AmountOfGroupsToSpawn(int wavesInWave)
    {
        switch (wavesInWave)
        {
            case 2:
                index1 = AmountToSpawnInGroup();
                index2 = AmountToSpawnInGroup();
                index3 = 0;
                index4 = 0;
                index5 = 0;
                break;
            case 3:
                index1 = AmountToSpawnInGroup();
                index2 = AmountToSpawnInGroup();
                index3 = AmountToSpawnInGroup();
                index4 = 0;
                index5 = 0;
                break;
            case 4:
                index1 = AmountToSpawnInGroup();
                index2 = AmountToSpawnInGroup();
                index3 = AmountToSpawnInGroup();
                index4 = AmountToSpawnInGroup();
                index5 = 0;
                break;
            case 5:
                index1 = AmountToSpawnInGroup();
                index2 = AmountToSpawnInGroup();
                index3 = AmountToSpawnInGroup();
                index4 = AmountToSpawnInGroup();
                index5 = AmountToSpawnInGroup();
                break;
            default:
                index1 = AmountToSpawnInGroup();
                index2 = 0;
                index3 = 0;
                index4 = 0;
                index5 = 0;
                break;
        }
        indexTotal = index1 + index2 + index3 + index4 + index5;
    }
    int extraAmount;
    int editableEnemiesMin, editableEnemiesMax;
    int AmountToSpawnInGroup()
    {
        int amount;
        if (tutorial)
        {
            amount = Random.Range(editableEnemiesMin, editableEnemiesMax);
            return amount;
        }
        if (timerLevel < 15) amount = Random.Range(1, 2);
        else if (timerLevel < 30) amount = Random.Range(1, 3);
        else if (timerLevel < 60) amount = Random.Range(2, 4);
        else if (timerLevel < 90) amount = Random.Range(3, 5);
        else
        {
            if(wave % 5 == 0 && wave != 0 && extraAmount < 3 && extraAmountEnemies)
            {
                extraAmount++;
                extraAmountEnemies = false;
            }
            amount = Random.Range(4, 6 + extraAmount);
        }
        return amount;
    }
    
    void ChoseGruopToSpawn()
    {
        switch (wavesInWave)
        {
            case 1:
                group = index1;
                break;
            case 2:
                group = index2;
                break;
            case 3:
                group = index3;
                break;
            case 4:
                group = index4;
                break;
            case 5:
                group = index5;
                break;
            default:
                group = index1;
                break;
        }
        switch (group)
        {
            case 1:
                enemiesToSpawn = new List<GameObject>(enemiesGroup1);
                timeBetweenSpawn = 0.9f;
                break;
            case 2:
                enemiesToSpawn = new List<GameObject>(enemiesGroup2);
                timeBetweenSpawn = 0.49f;
                break;
            case 3:
                enemiesToSpawn = new List<GameObject>(enemiesGroup3);
                timeBetweenSpawn = 0.33f;
                break;
            case 4:
                enemiesToSpawn = new List<GameObject>(enemiesGroup4);
                timeBetweenSpawn = 0.24f;
                break;
            case 5:
                enemiesToSpawn = new List<GameObject>(enemiesGroup5);
                timeBetweenSpawn = 0.19f;
                break;
            case 6:
                enemiesToSpawn = new List<GameObject>(enemiesGroup5);
                timeBetweenSpawn = 0.16f;
                break;
            case 7:
                enemiesToSpawn = new List<GameObject>(enemiesGroup5);
                timeBetweenSpawn = 0.14f;
                break;
            case 8:
                enemiesToSpawn = new List<GameObject>(enemiesGroup5);
                timeBetweenSpawn = 0.12f;
                break;
            default:
                enemiesToSpawn = new List<GameObject>(enemiesGroup1);
                timeBetweenSpawn = 0.9f;
                break;
        }

        amountToSpawn = enemiesToSpawn.Count;
        indexSpawn = Random.Range(0, spawnPoints.Length);

        StarSpawnTrue();
    }
    void CloseSpawn()
    {
        if (startSpawn)
        {
            timerSpawn += Time.fixedDeltaTime;
            if (timerSpawn >= 1)
            {
                startSpawn = false;
                timerSpawn = 0;
            }
        }        
    }
    void ChangeInWave()
    {
        if (EnemyCombatGroup.Instance.GetCurrentMembers().Count > 0) inWave = true;
        else
        {
            inWave = false;
        }
    }
    void Coroutine()
    {
        coroutine = StartCoroutine(StartSpawn());
    }
    public IEnumerator StartSpawn()
    {
        while (true)
        {
            if (startSpawn)
            {
                for (int i = 0; i < amountToSpawn; i++)
                {
                    //Debug.Log(i);
                    ChooseEnemiesToSpawn(enemiesToSpawn);
                    spawnEnemy = true;
                    Spawn(enemiesToSpawn);

                    yield return new WaitForSeconds(timeBetweenSpawn);
                }
            }
            yield return new WaitForSeconds(timeBetweenSpawn);
        }        
    }
    void ChooseEnemiesToSpawn(List<GameObject> enemiesToSpawn)
    {
        indexEnemies = Random.Range(0, enemiesToSpawn.Count);
        indexEnemies = Mathf.Clamp(indexEnemies, 0, enemiesToSpawn.Count - 1);
        if (spawnBoss)
        {
            indexBosses = Random.Range(0, enemiesToSpawn.Count);
            indexBosses = Mathf.Clamp(indexEnemies, 0, enemiesToSpawn.Count - 1);
        }
    }
    void Spawn(List<GameObject> enemiesToSpawn)
    {
        if (spawnEnemy)
        {
            spawnEnemy = false;
           /* Debug.Log(enemiesToSpawn);
            Debug.Log(indexSpawn);
            Debug.Log(spawnPoints[indexSpawn].transform);
            Debug.Log(indexEnemies + "IndexEnemies");*/
            GameObject obj = Instantiate(enemiesToSpawn[indexEnemies], spawnPoints[indexSpawn].transform);
            indexSpawn = Random.Range(0, spawnPoints.Length);
        }
    }
    void SpawnB(List<GameObject> enemiesToSpawn)
    {
        if (spawnBoss)
        {
            spawnBoss = false;
            indexSpawn = Random.Range(0, spawnPoints.Length);
            GameObject obj = Instantiate(enemiesToSpawn[indexBosses], spawnPoints[indexSpawn].transform);
            StartCoroutine(MonitorBossDeath(obj));
        }
    }
    private IEnumerator MonitorBossDeath(GameObject enemy)
    {
        while (enemy != null)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Enemy was destroyed
        //indexTotal -= 1;
        bossSpawned = false;
    }
}
