using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBreathe : MonoBehaviour
{
    Vector3 standPos;
    Vector3 crouchPos;
    Vector3 currentPos;

    [SerializeField]
    private float amplitude;
    [SerializeField]
    private float period;
    private float standPeriod;
    private float crouchPeriod;
    [SerializeField]
    private float time;

    public float debugFloat;

    private CustomCharacterController movement;
    private PlayerCamera pCam;

    // todo, return camera to original position

    void Start()
    {
        standPeriod = period;
        crouchPeriod = period * 1.5f;

        movement = GetComponentInParent<CustomCharacterController>();
        pCam = GetComponent<PlayerCamera>();

        standPos = transform.localPosition;
        crouchPos = new Vector3(0, 0.375f, 0);
        currentPos = standPos;

    }

    void Update()
    {
        if (!movement.isDead && movement.isWalking && movement.isGrounded)
        {
            if (movement.isCrouching)
                period = crouchPeriod;
            else
                period = standPeriod;

            CameraBop();
        }
    }

    void CameraBop()
    {
        time += Time.deltaTime;
        float theta = time / period;
        float distance = amplitude * Mathf.Sin(theta);
        transform.localPosition = currentPos + Vector3.up * distance;
    }

    public void LerpPosition(float lerpPercent, bool isCrouching)
    {
        if (isCrouching)
            currentPos = Vector3.Lerp(currentPos, crouchPos, lerpPercent);
        else
            currentPos = Vector3.Lerp(currentPos, standPos, lerpPercent);
    }

}
