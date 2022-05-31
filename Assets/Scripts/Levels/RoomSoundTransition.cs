using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomSoundTransition : MonoBehaviour
{
    GameManager manager;
    public float startLowPass;

    public float insideLowPass;
    public float outsideLowPass;

    private float currentLowPass;
    private float lowPassLerp;

    private bool isInside;

    // Start is called before the first frame update
    void Start()
    {
        manager = FindObjectOfType<GameManager>();
        manager.master.SetFloat("musicLowPass", startLowPass);
        outsideLowPass = startLowPass;
        currentLowPass = startLowPass;
    }

    private void Update()
    {
        LowPassLerp();
    }

    void LowPassLerp()
    {
        if (isInside && lowPassLerp < 1f)
        {
            currentLowPass = Mathf.Lerp(outsideLowPass, insideLowPass, lowPassLerp);
            manager.master.SetFloat("musicLowPass", currentLowPass);
            lowPassLerp += Time.deltaTime;
        }
        else if (!isInside && lowPassLerp > 0f)
        {
            currentLowPass = Mathf.Lerp(outsideLowPass, insideLowPass, lowPassLerp);
            manager.master.SetFloat("musicLowPass", currentLowPass);
            lowPassLerp -= Time.deltaTime;
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.transform.parent != null)
        {
            if (other.gameObject.transform.parent.CompareTag("Player"))
            {
                if (!isInside)
                    isInside = true;
                else if (isInside)
                    isInside = false;
            }
        }
    }
}
