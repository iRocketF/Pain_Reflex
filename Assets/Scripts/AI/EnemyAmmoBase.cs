using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAmmoBase : MonoBehaviour
{
    public float currentMag;
    public float maxMag;
    // public float reserveAmmo;
    // public float maxReserveAmmo;

    private EnemyWeaponBase weapon;

    // Start is called before the first frame update
    void Start()
    {
        weapon = GetComponent<EnemyWeaponBase>();

        currentMag = maxMag;
    }

    public void ReduceAmmo()
    {
        currentMag--;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Reload()
    {
        currentMag = maxMag;
    }
}
