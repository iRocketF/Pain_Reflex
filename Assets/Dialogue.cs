using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dialogue : MonoBehaviour
{
    private AudioSource source;

    public AudioClip[] lines;

    public bool PlayOnAwake;
    public float delayBetweenLines;

    void Start()
    {
        source = GetComponent<AudioSource>();

        if (PlayOnAwake)
            source.PlayOneShot(lines[0]);
    }

    void Update()
    {
        
    }
}
