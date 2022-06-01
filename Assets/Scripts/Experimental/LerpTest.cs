using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LerpTest : MonoBehaviour
{
    public Vector3 point1;
    public Vector3 point2;

    public float lerpTime; // time in seconds to complete the lerp
    public float currentLerpTime; // how much time has elapsed
    public float perc; // percentage of time passed from lerpTime
    public float t;
    public float alpha;

    public float moveDistance;

    void Start()
    {
        point1 = transform.position;
        point2 = transform.position + transform.forward * moveDistance;
    }

    void Update()
    {
        //reset when we press spacebar
        if (Input.GetKeyDown(KeyCode.Space))
        {
            currentLerpTime = 0f;
        }

        //increment timer once per frame
        currentLerpTime += Time.deltaTime;

        /* if(currentLerpTime < lerpTime)
        {
            var tempColor = renderer.material.color;
            tempColor.a -= Time.deltaTime / lerpTime;
            renderer.material.color = tempColor;
        }*/

        if (currentLerpTime > lerpTime)
        {
            currentLerpTime = lerpTime;
        }

        //lerp!
        perc = currentLerpTime / lerpTime;
        transform.position = Vector3.Lerp(point1, point2, perc);


    }
}
