using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SniperScope : MonoBehaviour
{
    private WeaponBase sniper;
    private Camera scopeCam;

    // Start is called before the first frame update
    void Start()
    {
        sniper = GetComponentInParent<WeaponBase>();
        scopeCam = GetComponentInChildren<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (sniper.transform.parent == null)
            scopeCam.enabled = false;
        else
            scopeCam.enabled = true;
    }
}
