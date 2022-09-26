using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    private Transform player;

    public bool spawnerActive;
    public bool proximityLimit;
    public bool infiniteSpawns;
    public bool cinematicAI;
    public bool spawnStatic;
    public bool spawnPursuer;

    // then enemy that the spawner creates
    public List<GameObject> enemyPool;
    public GameObject enemyToSpawn;
    
    // how many enemies the spawner can create
    public int spawnAmount;
    public int enemiesSpawned;

    // spawn timings
    public float spawnDelay;
    private float spawnTimer;

    // the range from the player the spawner doesnt spawn
    public float rangeToPlayerLimit;
    private float distanceToPlayer;
    

    // where spawned enemies go
    public Transform rallyPoint;

    private void Start()
    {
        player = FindObjectOfType<CustomCharacterController>().transform;
    }

    void Update()
    {
        if(!proximityLimit)
        {
            if (spawnerActive && enemiesSpawned < spawnAmount)
            {
                spawnTimer += Time.deltaTime;

                if (spawnTimer >= spawnDelay)
                {
                    SpawnEnemy();
                    spawnTimer = 0f;
                }
            }
        }
        else
        {
            if (spawnerActive && enemiesSpawned < spawnAmount)
            {
                spawnTimer += Time.deltaTime;

                if (spawnTimer >= spawnDelay && distanceToPlayer > rangeToPlayerLimit)
                {
                    SpawnEnemy();
                    spawnTimer = 0f;
                }
            }
        }


        distanceToPlayer = Vector3.Distance(transform.position, player.position);
    }

    void SpawnEnemy()
    {
        GameObject newEnemy;
        EnemyAI newEnemyAI;

        // pick enemy from the pool
        if (enemyPool.Count > 0)
        {
            enemyToSpawn = enemyPool[Random.Range(0, enemyPool.Count)];

            newEnemy = Instantiate(enemyToSpawn, transform.position, transform.rotation);
            newEnemyAI = newEnemy.GetComponent<EnemyAI>();
        }
        else
        {
            newEnemy = Instantiate(enemyToSpawn, transform.position, transform.rotation);
            newEnemyAI = newEnemy.GetComponent<EnemyAI>();
        }


        if(rallyPoint != null)
            newEnemyAI.waypoint = rallyPoint;

        // set enemy parameters from spawn
        if (cinematicAI)
            newEnemyAI.destroyAtWalkpoint = true;
        if (spawnStatic)
            newEnemyAI.isStatic = true;
        if (spawnPursuer)
            newEnemyAI.isPursuer = true;
        if (infiniteSpawns)
            spawnAmount++;

        enemiesSpawned++;

        if (enemiesSpawned == spawnAmount)
            spawnerActive = false;

    }

    public void ActivateSpawner()
    {
        spawnerActive = true;
    }
}
