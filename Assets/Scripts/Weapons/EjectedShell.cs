using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EjectedShell : MonoBehaviour
{
    private AudioSource audioSource;
    public AudioClip[] shellSounds;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

        audioSource.clip = shellSounds[Mathf.RoundToInt(Random.Range(0, shellSounds.Length))];
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!audioSource.isPlaying)
            audioSource.Play();
    }
}
