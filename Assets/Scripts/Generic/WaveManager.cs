using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WaveManager : MonoBehaviour
{
    // FULLY COMMENT THIS AT SOME POINT

    // bools
    public bool hasStarted;
    public bool waveOngoing;

    // wave timers
    public float prepTime;
    public float timeBetweenWaves;
    [SerializeField] private float timer;

    // wave related variables
    [SerializeField] private int currentWave;
    [SerializeField] private int enemiesToSpawn;
    [SerializeField] private int enemiesSpawned;
    [SerializeField] private int enemiesKilled;

    // spawners and variables
    public List<EnemySpawner> spawners;
    private int spawnsToAllocate;
    [SerializeField] private int waveTier;
    [SerializeField] private int wavesPerTier;

    // enemy tier spawns & lists
    public List<GameObject> enemies_tier1;
    public List<GameObject> enemies_tier2;
    public List<GameObject> enemies_tier3;

    // hud elements
    public TextMeshProUGUI waveText;
    public TextMeshProUGUI killCountText;
    public TextMeshProUGUI enemiesLeftText;

    private Transform player;

    void Start()
    {
        timer = prepTime;

        spawners.AddRange(FindObjectsOfType<EnemySpawner>());
        player = FindObjectOfType<CustomCharacterController>().transform;

        enemiesToSpawn = spawners.Count;

        killCountText.enabled = false;
        enemiesLeftText.enabled = false;
    }

    void Update()
    {
        if (!hasStarted)
        {
            timer = timer - Time.deltaTime;
            waveText.text = Mathf.RoundToInt(timer).ToString();

            if (timer <= 0f)
            {
                waveTier++;

                hasStarted = true;
                waveOngoing = true;

                timer = timeBetweenWaves;

                StartWave();
            }
        }
        else if (!waveOngoing && hasStarted)
        {
            timer = timer - Time.deltaTime;
            waveText.text = Mathf.RoundToInt(timer).ToString();

            if (timer <= 0f)
            {
                waveOngoing = true;

                timer = timeBetweenWaves;

                StartWave();
            }
        }

    }

    void StartWave()
    {
        currentWave++;

        if (currentWave > waveTier * wavesPerTier)
            UpgradeTier();

        enemiesToSpawn = enemiesToSpawn + spawners.Count;

        waveText.text = "WAVE: " + currentWave.ToString();

        ToggleText();

        enemiesSpawned = 0;
        enemiesKilled = 0;

        killCountText.text = "KILLS: 0";
        enemiesLeftText.text = "LEFT: " + enemiesToSpawn.ToString();

        for (int i = 0; i < spawners.Count; i++)
        {
            spawners[i].spawnerActive = true;
            spawners[i].enemiesSpawned = 0;
            spawners[i].spawnAmount = enemiesToSpawn / spawners.Count;
            //spawners[i].rallyPoint = player;

            switch(waveTier)
            {
                case 1:
                    spawners[i].enemyPool = enemies_tier1;
                    break;
                case 2:
                    spawners[i].enemyPool = enemies_tier2;
                    break;
                case 3:
                    spawners[i].enemyPool = enemies_tier3;
                    break;
            }
        }
    }

    void FinishWave()
    {
        waveOngoing = false;

        ToggleText();
    }

    public void AddToKills()
    {
        enemiesKilled++;

        killCountText.text = "KILLS: " + enemiesKilled.ToString();
        enemiesLeftText.text = "LEFT: " + (enemiesToSpawn - enemiesKilled).ToString();

        if (enemiesKilled == enemiesToSpawn)
            FinishWave();
    }

    void ToggleText()
    {
        if(killCountText.enabled && enemiesLeftText.enabled)
        {
            killCountText.enabled = false;
            enemiesLeftText.enabled = false;
        }
        else
        {
            killCountText.enabled = true;
            enemiesLeftText.enabled = true;
        }
    }

    void UpgradeTier()
    {
        enemiesToSpawn = spawners.Count;

        waveTier++;
    }
}
