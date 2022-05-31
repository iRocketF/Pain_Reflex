using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleDecalPool : MonoBehaviour
{
    // this particle decal system was created using this tutorial:
    // https://www.youtube.com/watch?v=JZ6KDDA6I4Q
    // https://learn.unity.com/tutorial/recorded-video-session-controlling-particles-via-script
    // remember to check it out in case of problems

    public int maxDecals = 100;
    public float decalSizeMin;
    public float decalSizeMax;

    [HideInInspector]
    public ParticleSystem decalParticleSystem;
    public int particleDecalDataIndex;
    private ParticleDecalData[] particleData;
    private ParticleSystem.Particle[] particles;

    void Awake()
    {
        decalParticleSystem = GetComponent<ParticleSystem>();

        particles = new ParticleSystem.Particle[maxDecals];
        particleData = new ParticleDecalData[maxDecals];

        for(int i = 0; i < maxDecals; i++)
        {
            particleData[i] = new ParticleDecalData();
        }
    }

    public void ParticleHit(ParticleCollisionEvent particleCollisionEvent)
    {
        SetParticleData(particleCollisionEvent);
        DisplayParticles();
    }

    void SetParticleData(ParticleCollisionEvent particleCollisionEvent)
    {
        if(particleDecalDataIndex >= maxDecals)
        {
            particleDecalDataIndex = 0;
        }

        // record collision position, rotation, size and color
        particleData[particleDecalDataIndex].position = particleCollisionEvent.intersection;
        Vector3 particleRotationEuler = Quaternion.LookRotation(particleCollisionEvent.normal).eulerAngles;
        particleRotationEuler.z = Random.Range(0, 360);
        particleData[particleDecalDataIndex].rotation = particleRotationEuler;
        particleData[particleDecalDataIndex].size = Random.Range(decalSizeMin, decalSizeMax);
        // particleData[particleDecalDataIndex].parent = particleCollisionEvent.colliderComponent.transform;

        particleDecalDataIndex++;
    }

    void DisplayParticles()
    {
        for(int i = 0; i < particleData.Length; i++)
        {
            particles[i].position = particleData[i].position;
            particles[i].rotation3D = particleData[i].rotation;
            particles[i].startSize = particleData[i].size;
            particles[i].startColor = particleData[i].color;
        }

        decalParticleSystem.SetParticles(particles, particles.Length);
    }

    public void ClearParticles()
    {
        if(particleData != null)
            for (int i = 0; i < particleData.Length; i++)
                particleData[i] = null;

    }

}
