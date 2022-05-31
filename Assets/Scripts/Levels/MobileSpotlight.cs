using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobileSpotlight : MonoBehaviour
{
    public Transform spot1;
    public Transform spot2;
    public Transform mobileSpotlight;

    public int direction = 1;

    private float timer;
    public float lerpTime = 1;

    void Update()
    {
        timer += Time.deltaTime * direction;
        mobileSpotlight.position = Vector3.Lerp(spot1.position, spot2.position, timer / lerpTime);

        if (timer >= lerpTime || timer <= 0)
            direction *= -1;
    }
}
