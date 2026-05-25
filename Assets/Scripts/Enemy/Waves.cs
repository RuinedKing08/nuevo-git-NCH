using UnityEngine;
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
    float timerSpawn;
    float timerWaveWaiting;
    float maxTimer;
    float timeBetweenSpawn;
    Coroutine coroutine;
    bool inWave, spawnEnemy, waveWaiting, firstWaveWaiting, startSpawn;
    int indexEnemies;
    public int indexSpawn;
    void Start()
    {
        spawnPoints = transform.GetComponentsInChildren<SpawnPointsForEnemies>();
        wave = 1;
        wavesInWave = Random.Range(3, 6);
        indexSpawn = Random.Range(0, spawnPoints.Length);
        inWave = false;
        startSpawn = false;
        firstWaveWaiting = true;
        EnemyCombatGroup.Instance.OnChangeCurrentMembers += ChangeInWave;
        Coroutine();
    }

    void FixedUpdate()
    {
        CloseSpawn();
        TimerToSpawn();
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
                    ChangeWave();
                    timerInWave = maxTimer;
                    inWave = true;
                }
                else if(EnemyCombatGroup.Instance.GetCurrentMembers().Count < 5 && waveWaiting && !firstWaveWaiting)
                {
                    wavesWaiting--;
                    ChangeWavesWating();
                    ChoseGruopToSpawn();
                    StarSpawnTrue();
                    ChangeWave();
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
                    ChangeWave();
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
                    ChangeWave();
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
        wavesInWave--;
        if(wavesInWave <= 0)
        {
            wave++;
            wavesInWave = Random.Range(3, 6);
        }
    }
    int group;
    void ChoseGruopToSpawn()
    {        
        if (timerLevel < 15) group = Random.Range(1, 2);
        else if(timerLevel < 30) group = Random.Range(1, 3);
        else if(timerLevel < 60) group = Random.Range(2, 4);
        else if(timerLevel < 90) group = Random.Range(3, 5);
        else group = Random.Range(1, 6);
        switch (group)
        {
            case 1:
                enemiesToSpawn = enemiesGroup1;
                timeBetweenSpawn = 0.9f;
                break;
            case 2:
                enemiesToSpawn = enemiesGroup2;
                timeBetweenSpawn = 0.49f;
                break;
            case 3:
                enemiesToSpawn = enemiesGroup3;
                timeBetweenSpawn = 0.33f;
                break;
            case 4:
                enemiesToSpawn = enemiesGroup4;
                timeBetweenSpawn = 0.24f;
                break;
            case 5:
                enemiesToSpawn = enemiesGroup5;
                timeBetweenSpawn = 0.19f;
                break;
            default:
                enemiesToSpawn = enemiesGroup1;
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
