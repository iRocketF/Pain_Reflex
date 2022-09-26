using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private PlayerCamera pCam;
    private CustomCharacterController controller;

    private void Start()
    {
        pCam = GetComponent<PlayerCamera>();
        controller = GetComponentInParent<CustomCharacterController>();
    }

    public IEnumerator Shake(float duration, float magnitude)
    {
        Vector3 originalPos = transform.localPosition;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            if(controller.isCrouching)
            {
                float x = Random.Range(-0.2f, 0.2f) * magnitude;
                float y = Random.Range((pCam.crouchHeight - 0.05f), (pCam.crouchHeight + 0.05f));

                transform.localPosition = new Vector3(x, y, originalPos.z);
            }
            else
            {
                float x = Random.Range(-0.3f, 0.3f) * magnitude;
                float y = Random.Range((pCam.startHeight - 0.05f), (pCam.startHeight + 0.05f));

                transform.localPosition = new Vector3(x, y, originalPos.z);
            }



            elapsed += Time.deltaTime;

            yield return null;
        }

        transform.localPosition = originalPos;
    }
}
