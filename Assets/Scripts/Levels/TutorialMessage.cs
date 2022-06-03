using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialMessage : MonoBehaviour
{
    public bool isStatic;
    private PlayerCamera pCam;

    void Start()
    {
        pCam = FindObjectOfType<PlayerCamera>();
    }

    void Update()
    {
        if (!isStatic)
            LookAtPlayer();
    }

    // message always looks at player
    void LookAtPlayer()
    {
        Vector3 upAxis = Vector3.up;
        transform.rotation = Quaternion.LookRotation(Vector3.Cross(upAxis, Vector3.Cross(upAxis, pCam.transform.forward)), upAxis);
    }

}
