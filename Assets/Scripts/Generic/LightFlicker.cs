using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFlicker : MonoBehaviour
{
    public Light spotLight;
    public float minIntensity = 0f;
    public float maxIntensity = 1f;

    [Range(1, 50)]
    public int smoothing = 5;

    Queue<float> smoothQueue;
    float lastSum = 0;

    public void Reset()
    {
        smoothQueue.Clear();
        lastSum = 0;
    }

    void Start()
    {
        smoothQueue = new Queue<float>(smoothing);
        spotLight = GetComponent<Light>();
    }

    void Update()
    {
        if (spotLight == null)
            return;

        while (smoothQueue.Count >= smoothing)
        {
            lastSum -= smoothQueue.Dequeue();
        }

        float newVal = Random.Range(minIntensity, maxIntensity);
        smoothQueue.Enqueue(newVal);
        lastSum += newVal;

        spotLight.intensity = lastSum / (float)smoothQueue.Count;
    }
}
