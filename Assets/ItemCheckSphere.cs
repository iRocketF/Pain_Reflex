using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCheckSphere : MonoBehaviour
{
    public float size;

    [SerializeField]
    private WeaponBase currentWeapon;
    private AmmoBase currentAmmo;

    private PlayerInventory inventory;

    void Start()
    {
        transform.localScale = new Vector3(size, size, size);
    }

    public void UpdateWeapon(WeaponBase weapon)
    {
        currentWeapon = weapon;
        currentAmmo = weapon.GetComponent<AmmoBase>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(currentWeapon !=null)
        {
            if (other.CompareTag("Weapon"))
            {
                if (currentAmmo.currentMag <= currentAmmo.maxMag / 2 && other.GetComponent<AmmoBase>().currentMag > 0)
                {
                    other.GetComponent<Outline>().enabled = true;
                    other.GetComponent<Outline>().OutlineMode = Outline.Mode.OutlineAndSilhouette;
                    other.GetComponent<Outline>().enableTimer = false;
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Weapon"))
        {
            other.GetComponent<Outline>().enabled = false;
            other.GetComponent<Outline>().OutlineMode = Outline.Mode.OutlineVisible;
            other.GetComponent<Outline>().enableTimer = true;
        }
    }

}
