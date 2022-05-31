using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoBase : MonoBehaviour
{
    public int ammoInt;
    public float currentMag;
    public float maxMag;
    // public float reserveAmmo;
    // public float maxReserveAmmo;

    public bool isReloading;
    public bool startEmpty;
    public bool isDrop;
    [SerializeField]
    private bool infiniteAmmo;

    private WeaponBase weapon;
    private PlayerHUD hud;
    [HideInInspector]
    public PlayerInventory inventory;

    public void Start()
    {
        inventory = FindObjectOfType<PlayerInventory>();
        hud = inventory.GetComponentInChildren<PlayerHUD>();
        weapon = GetComponent<WeaponBase>();

        if (startEmpty && !isDrop)
            currentMag = 0f;
        else if (!isDrop)
            currentMag = maxMag;

        for (int i = 0; i < inventory.ammoTypes.Count; i++)
        {
            if (weapon.caliber == inventory.ammoTypes[i])
                ammoInt = i;
        }

    }

    public void ReduceAmmo()
    {
        currentMag--;

        if (infiniteAmmo)
            currentMag++;

        hud.UpdateAmmoText();
    }

    // normal shooter mechanics right now, bullet based reloads, not magazines
    // logic: check how much ammo needed for full reload
    // if needed ammo is larger than currently available, match reduced ammo to current reserve ammo
    // reduce ammo from reserve ammo pool
    public void Reload()
    {
        float neededAmmo;

        neededAmmo = maxMag - currentMag;

        if (neededAmmo > inventory.currentAmmo[ammoInt])
        {
            neededAmmo = inventory.currentAmmo[ammoInt];
            inventory.currentAmmo[ammoInt] = inventory.currentAmmo[ammoInt] - neededAmmo;
        }
        else
            inventory.currentAmmo[ammoInt] = inventory.currentAmmo[ammoInt] - neededAmmo;

        currentMag = currentMag + neededAmmo;

        isReloading = false;

        hud.UpdateAmmoText();
    }

    public void ShotgunReload()
    {
        //Debug.Log("Reloading");

        currentMag++;
        inventory.currentAmmo[ammoInt]--;
        weapon.weaponSound.PlayOneShot(weapon.weaponSounds[5]);

        hud.UpdateAmmoText();

        if (currentMag == maxMag)
        {
            //Debug.Log("Finished Reload");
            weapon.animator.SetTrigger("reloadFinish");
            isReloading = false;
        }
        else if (isReloading && inventory.currentAmmo[ammoInt] == 0)
        {
            weapon.animator.SetTrigger("reloadFinish");
            isReloading = false;
        }
        else if (isReloading && inventory.currentAmmo[ammoInt] > 0)
        {
            weapon.animator.SetTrigger("reloadNext");
            weapon.WeaponReload();
        }
    }

    public void InfiniteAmmo()
    {
        if (!infiniteAmmo)
            infiniteAmmo = true;
        else
            infiniteAmmo = false;
    }
}
