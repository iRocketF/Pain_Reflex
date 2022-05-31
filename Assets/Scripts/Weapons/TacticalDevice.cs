using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TacticalDevice : MonoBehaviour
{
    public Light spotLight;
    public bool deviceActive;

    //private WeaponBase weapon;

    // Start is called before the first frame update
    void Start()
    {
        spotLight = GetComponentInChildren<Light>();
        spotLight.enabled = false;

        //weapon = GetComponentInParent<WeaponBase>();
    }

    // Update is called once per frame
    void Update()
    {
        //if(weapon.isPickedUp)

        if (Input.GetButtonDown("Tactical"))
                SwitchTactical();
    }

    void SwitchTactical()
    {
        if (!deviceActive)
        {
            spotLight.enabled = true;
            deviceActive = true;
        }
        else
        {
            spotLight.enabled = false;
            deviceActive = false;
        }
    }
}
