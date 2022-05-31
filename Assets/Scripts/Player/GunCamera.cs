using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunCamera : MonoBehaviour
{
    public Camera mainCamera;
    public Camera gunCamera;

    void Update()
    {
        gunCamera.fieldOfView = mainCamera.fieldOfView;
        gunCamera.transform.position = mainCamera.transform.position;
        gunCamera.transform.rotation = mainCamera.transform.rotation;
    }
}
