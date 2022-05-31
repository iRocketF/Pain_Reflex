using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableGlass : MonoBehaviour
{
    public float glassHp;

    public AudioSource glassBreak;
    public ParticleSystem glassBreakParticles;

    private GameManager manager;

    private void Start()
    {
        manager = FindObjectOfType<GameManager>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            BreakGlass();
        }
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.layer == 6)
        {
            if (manager.matrixMode)
            {
                BreakGlass();
            }
        }
    }

    public void BreakGlass()
    {
        AudioSource glass = Instantiate(glassBreak);
        glass.PlayOneShot(glass.clip);
        glass.gameObject.AddComponent<CleanUp>().cleanUpTime = 2f;

        ParticleSystem particles = Instantiate(glassBreakParticles, transform.position, transform.rotation);
        //particles.Scale(GetComponent<Transform>().localScale);
        particles.gameObject.AddComponent<CleanUp>().cleanUpTime = 3f;

        Destroy(transform.gameObject);
    }
}
