using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CleanUpGib : MonoBehaviour
{
    public float cleanUpTime = 10f;
    private float timer;

    void Start()
    {

    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= cleanUpTime)
        {
            Destroy(gameObject);
        }
    }
}
