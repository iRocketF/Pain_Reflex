using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugMode : MonoBehaviour
{
    public GameObject[] bullets;

    private CleanUpTemp tempCleaner;

    void Start()
    {
        tempCleaner = GetComponent<CleanUpTemp>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F1))
        {
            ToggleBulletDebug();
        }

        if(Input.GetKeyDown(KeyCode.F2))
        {
            EnableInfiniteAmmo();
        }
    }

    void ToggleBulletDebug()
    {
        for(int i = 0; i < bullets.Length; i++)
        {
            if (!bullets[i].GetComponentInChildren<BulletBase>().debug)
            {
                bullets[i].GetComponentInChildren<BulletBase>().debug = true;
                Debug.Log("bullet debug on");
            }
               
            else
            {
                bullets[i].GetComponentInChildren<BulletBase>().debug = false;
                tempCleaner.SeekAndDestroy();
                Debug.Log("bullet debug false");
            }
        }
    }

    void EnableInfiniteAmmo()
    {
        PlayerInventory inventory = FindObjectOfType<PlayerInventory>();

        if (inventory.weaponInventory[0].GetComponent<AmmoBase>() != null)
            inventory.weaponInventory[0].GetComponent<AmmoBase>().InfiniteAmmo();
    }
}
