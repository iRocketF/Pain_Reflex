using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScoring : MonoBehaviour
{
    public float killScore;
    private string type;
    private ScoreManager manager;

    void Start()
    {
        manager = FindObjectOfType<GameManager>().GetComponent<ScoreManager>();
        type = gameObject.name.ToString();
    }

    public void AddScore()
    {
        manager.UpdateScore(killScore, type);
    }


}
