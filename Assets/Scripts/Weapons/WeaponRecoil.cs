using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponRecoil : MonoBehaviour
{
    public float recoilKickTime; // time in seconds for the weapon to kick
    public float recoilRecoveryTime; // time in seconds to recover fully
    private float currentRecoilTime; // how much time has elapsed
    private float lerp; // percentage of time passed from recoilRecoveryTime

   
    //public float recoilSmooth;
    [SerializeField]
    public float recoilSmoothModifier;

    private PlayerCamera pCam;
    private WeaponBase weapon;
    private float recoil;
    private bool isKicking;
    private bool isRecovering;

    public void Start()
    {
        weapon = GetComponent<WeaponBase>();
        
        pCam = FindObjectOfType<PlayerCamera>();

        currentRecoilTime = recoilRecoveryTime;
    }

    void Update()
    {
        if(weapon.isPickedUp)
        {
            if(isKicking)
                RecoilKickback();

            if (currentRecoilTime < recoilRecoveryTime && isRecovering)
                RecoilRecovery();
        }     
    }

    public void AddRecoil()
    {
        // reset the recoil lerp to 0 to start the "animation" from beginning
        // add recoil to camera with a lerp to smooth it

        currentRecoilTime = 0f;
        //recoilSmooth = 0f;
        //isRecovering = false;
        recoil = weapon.recoil;

        isKicking = true;
        isRecovering = false;

        //pCam.xRotation += recoil;
    }

    void RecoilKickback()
    {
        currentRecoilTime += Time.deltaTime;

        lerp = currentRecoilTime / recoilKickTime;

        pCam.xRotation = pCam.xRotation + Mathf.Lerp(recoil, 0, lerp) * Time.deltaTime * recoilSmoothModifier;

        if(lerp > 1f)
        {
            currentRecoilTime = 0f;
            isKicking = false;
            isRecovering = true;
        }

    }

    void RecoilRecovery()
    {
        currentRecoilTime += Time.deltaTime;

        if (currentRecoilTime > recoilRecoveryTime)
        {
            currentRecoilTime = recoilRecoveryTime;
            lerp = 1f;
            isRecovering = false;
        }

        lerp = currentRecoilTime / recoilRecoveryTime;
        lerp = Mathf.Sin(lerp * Mathf.PI * 0.5f);

        pCam.xRotation = pCam.xRotation - Mathf.Lerp(recoil, 0, lerp) * Time.deltaTime; // recovery



    }
}
