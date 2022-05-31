using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodSplatter : MonoBehaviour
{
    public ParticleSystem bloodParticles;
    public ParticleDecalPool splatDecalPool;

    private GameObject manager;

    List<ParticleCollisionEvent> collisionEvents;

    void Start()
    {
        manager = FindObjectOfType<GameManager>().gameObject;

        bloodParticles = GetComponent<ParticleSystem>();
        splatDecalPool = manager.GetComponentInChildren<ParticleDecalPool>();

        collisionEvents = new List<ParticleCollisionEvent>();
    }

    private void OnParticleCollision(GameObject other)
    {
        ParticlePhysicsExtensions.GetCollisionEvents(bloodParticles, other, collisionEvents);

        for (int i = 0; i < collisionEvents.Count; i++)
        {
            splatDecalPool.ParticleHit(collisionEvents[i]);
        }
    }

    void Update()
    {
        ParticleSystem.MainModule psMain = bloodParticles.main;
    }


}
