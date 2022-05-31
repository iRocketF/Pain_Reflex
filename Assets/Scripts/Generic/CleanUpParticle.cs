using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CleanUpParticle : MonoBehaviour
{
    private ParticleSystem particles;

    private float cleanUpTime;
    private float timer;

    // Start is called before the first frame update
    void Start()
    {
        particles = GetComponent<ParticleSystem>();
        cleanUpTime = particles.main.duration * 2;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= cleanUpTime)
        {
            Destroy(gameObject);
        }
    }
}
