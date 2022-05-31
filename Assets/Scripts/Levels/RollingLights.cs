using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollingLights : MonoBehaviour
{
    public Light[] lights;

    public int lightsToTurnOn;
    public float lightDelay;

    private AudioSource sound;

    void Start()
    {
        lights = GetComponentsInChildren<Light>();
        sound = GetComponent<AudioSource>();
    }

    // cascading lights for a cinematic effect
    public void RollLights()
    {
        int lightsOn = 0;

        for(int i = 0; i < lights.Length; i++)
        {
            if (!lights[i].enabled)
            {
                lights[i].enabled = true;
                sound.PlayOneShot(sound.clip);
                lightsOn++;

                if (lightsOn == lightsToTurnOn)
                    break;
            }
        }
    }

    // setup the lights
    public void InvokeLights()
    {
        Invoke("RollLights", lightDelay);

        Invoke("RollLights", lightDelay * 2);

        Invoke("RollLights", lightDelay * 3);
    }
}
