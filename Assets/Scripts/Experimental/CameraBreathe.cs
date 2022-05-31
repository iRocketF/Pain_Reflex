using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBreathe : MonoBehaviour
{
    Vector3 startPos;

    [SerializeField]
    private float amplitude;
    [SerializeField]
    private float period;
    [SerializeField]
    private float time;



    public float debugFloat;

    private CustomCharacterController movement;

    // todo, return camera to original position

    void Start()
    {
        startPos = transform.localPosition;

        movement = GetComponentInParent<CustomCharacterController>();
    }

    void Update()
    {
        if (!movement.isDead)
        {        
            CameraBop();
        }

        void CameraBop()
        {
            if(movement.isWalking && !movement.isCrouching)
            {
                time += Time.deltaTime;
                float theta = time / period;
                float distance = amplitude * Mathf.Sin(theta);
                transform.localPosition = startPos + Vector3.up * distance;
            }
        }
    }
}
