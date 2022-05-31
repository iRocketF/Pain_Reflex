using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public int kills;

    public float score;
    public float scoreMultiplier;

    public EnemyAI[] enemies;

    void Start()
    {
        enemies = FindObjectsOfType<EnemyAI>();
    }

    public void UpdateScore(float killScore, string enemyType)
    {
        score = score + killScore * scoreMultiplier;
        // Debug.Log("killed " + enemyType + "!");
    }


}
