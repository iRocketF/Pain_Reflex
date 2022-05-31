using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyVoice : MonoBehaviour
{
    [Range(0,1)]
    public float phraseChance;

    public AudioClip[] reloadClip;
    public AudioClip[] hurtClip;
    public AudioClip[] deathClip;
    public AudioClip[] spottedClip;

    private AudioSource voice;

    void Start()
    {
        voice = GetComponent<AudioSource>();
    }

    public void PlayReloadVoice()
    {
        if (Random.value < phraseChance)
        {
            voice.clip = reloadClip[Random.Range(0, reloadClip.Length)];
            voice.Play();
        }

    }

    public void PlayHurtVoice()
    {
        voice.clip = hurtClip[Random.Range(0, hurtClip.Length)];
        voice.Play();
    }

    public void PlayDeathVoice()
    {
        voice.clip = deathClip[Random.Range(0, deathClip.Length)];
        voice.Play();
    }

    public void PlaySpotVoice()
    {
        if (Random.value < phraseChance)
        {
            voice.clip = spottedClip[Random.Range(0, spottedClip.Length)];
            voice.Play();
        }
           
    }
}
