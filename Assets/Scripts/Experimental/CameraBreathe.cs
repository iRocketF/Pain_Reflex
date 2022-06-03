using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBreathe : MonoBehaviour
{
    [SerializeField] Vector3 standPos;
    [SerializeField] Vector3 crouchPos;
    [SerializeField] Vector3 currentPos;

    [SerializeField]
    private float amplitude;
    [SerializeField]
    private float period;
    [SerializeField]
    private float time;

    public float debugFloat;

    private CustomCharacterController movement;
    private PlayerCamera pCam;

    // todo, return camera to original position

    void Start()
    {

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
