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
    public AudioClip[] playerDeathClip;
    public AudioClip[] allyDeathClip;

    private AudioSource voice;

    void Start()
    {
        voice = GetComponent<AudioSource>();
    }

    public void PlayReloadVoice()
    {
        voice.clip = reloadClip[Random.Range(0, reloadClip.Length)];
        voice.Play();
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
        voice.clip = spottedClip[Random.Range(0, spottedClip.Length)];
        voice.Play();
    }

    public bool PlayAllyDeathVoice()
    {
        if (Random.value < phraseChance)
        {
            voice.clip = allyDeathClip[Random.Range(0, allyDeathClip.Length)];
            voice.Play();

            return true;
        }

        return false;
    }

    public void PlayPlayerDeathVoice()
    {
        voice.clip = playerDeathClip[Random.Range(0, playerDeathClip.Length)];
        voice.Play();
    }
}
