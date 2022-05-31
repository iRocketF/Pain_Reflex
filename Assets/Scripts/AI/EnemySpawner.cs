using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public bool spawnerActive;
    public bool infiniteSpawns;

    // then enemy that the spawner creates
    public GameObject enemyToSpawn;
    
    // how many enemies the spawner can create
    public int spawnAmount;
    private int enemiesSpawned;

    public float spawnDelay;
    private float spawnTimer;

    public Transform rallyPoint;

    void Update()
    {
        if (spawnerActive && enemiesSpawned < spawnAmount)
        {
            spawnTimer += Time.deltaTime;

            if(spawnTimer >= spawnDelay)
            {
                SpawnEnemy();
                spawnTimer = 0f;
            }

            if (enemiesSpawned == spawnAmount)
                enabled = false;
        }
    }

    void SpawnEnemy()
    {
        GameObject newEnemy = Instantiate(enemyToSpawn, transform.position, transform.rotation);

        newEnemy.GetComponent<EnemyAI>().waypoint = rallyPoint;

        if (infiniteSpawns)
            spawnAmount++;

        enemiesSpawned++;

        if (enemiesSpawned == spawnAmount)
            Destroy(gameObject);

    }

    public void ActivateSpawner()
    {
        spawnerActive = true;
    }
}
