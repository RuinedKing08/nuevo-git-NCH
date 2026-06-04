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
    int indexEnemies;
    public int indexSpawn;
    int group, enemiesLeftCount;
    int index1, index2, index3, index4, index5, indexTotal;
    void Start()
    {
        spawnPoints = transform.GetComponentsInChildren<SpawnPointsForEnemies>();
        waveTMP = GameObject.Find("Canvas").transform.Find("WaveTMP").GetComponent<TMP_Text>();
        enemiesLeftTMP = GameObject.Find("Canvas").transform.Find("EnemiesLeftTMP").GetComponent<TMP_Text>();
        waveAnim = GameObject.Find("Canvas").GetComponent<Animator>();
        wave = 0;
        ChangeWave();
        indexSpawn = Random.Range(0, spawnPoints.Length);
        inWave = false;
        startSpawn = false;
        firstWaveWaiting = true;
        EnemyCombatGroup.Instance.OnChangeCurrentMembers += ChangeInWave;
        EnemyCombatGroup.Instance.OnDecreaseCurrentMembers += ChangeEnemiesLeftTMP;
        EnemyCombatGroup.Instance.OnDecreaseCurrentMembers += ChangeWave;
        Coroutine();
    }

    void FixedUpdate()
    {
        CloseSpawn();
        TimerToSpawn();
    }
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
    void TimerToSpawn()
    {
        timerLevel += Time.fixedDeltaTime;
        if (inWave)
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
            
        }
    }
    void ChangeWavesWating()
    {
        if (wavesWaiting <= 0)
        {
            waveWaiting = false;
            wavesWaiting = 0;
        }
        else waveWaiting = true;
    }
    void ChangeWave()
    {
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
        if (wavesInWave <= 0)
        {
            wave++;
            waveTMP.text = ($"Oleada {wave}");
            waveAnim.Play("WaveTitle");
            wavesInWave = Random.Range(3, 6);
            AmountOfGroupsToSpawn(wavesInWave);
            enemiesLeftTMP.text = ($"Enemigos Restantes: {indexTotal}");
        }
    }
    void AmountOfGroupsToSpawn(int wavesInWave)
    {
        switch (wavesInWave)
        {
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
                index2 = AmountToSpawnInGroup();
                index3 = AmountToSpawnInGroup();
                index4 = 0;
                index5 = 0;
                break;
        }
        indexTotal = index1 + index2 + index3 + index4 + index5;
    }
    int AmountToSpawnInGroup()
    {
        int amount;
        if (timerLevel < 15) amount = Random.Range(1, 2);
        else if (timerLevel < 30) amount = Random.Range(1, 3);
        else if (timerLevel < 60) amount = Random.Range(2, 4);
        else if (timerLevel < 90) amount = Random.Range(3, 5);
        else amount = Random.Range(1, 6);
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
            default:
                enemiesToSpawn = new List<GameObject>(enemiesGroup1);
                timeBetweenSpawn = 0.9f;
                break;
        }

        amountToSpawn = enemiesToSpawn.Count;
        indexSpawn = Random.Range(0, spawnPoints.Length);
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
        indexEnemies = indexSpawn;// Random.Range(0, enemiesToSpawn.Count);
        indexEnemies = Mathf.Clamp(indexEnemies, 0, enemiesToSpawn.Count - 1);
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
}
