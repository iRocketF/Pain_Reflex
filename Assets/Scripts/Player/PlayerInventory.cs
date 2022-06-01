using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInventory : MonoBehaviour
{
    public int maxWeapons;
    public GameObject[] weaponInventory;

    // create an array/list for different ammotypes and their max carry capacity
    // the inventory should also store the different amount of spare ammo for every ammo type
    // assign them "pistol", "shotgun", "sniper" etc.

    // ammo inventory PROTOTYPE
    public List<string> ammoTypes = new List<string>();
    public List<float> maxAmmoCapacity = new List<float>();
    public List<float> currentAmmo = new List<float>();

    // keycard inventory
    // KEYCARDS required for different colored buttons/card readers
    // KEYCARDS will unlock new areas, weapons, ammo etc.
    // LOGIC
    /*
     * button code
     * 
     * string requiredColor = "predefined"
     * 
     * GET Keycards from list, check if it has required color
    */
    public List<GameObject> keycards = new List<GameObject>();

    // gameobjects to save into manager so the player can receive them after death
    private List<GameObject> itemsToSave = new List<GameObject>();
    private List<float> ammoToSave = new List<float>();

    [SerializeField]
    private PlayerHUD hud;

    public Transform weaponPosition;
    public Transform keycardHolder;
    public RectTransform keycardUiHolder;

    public void Start()
    {
        if (weaponPosition.childCount > 0)
            SetWeapon(weaponPosition.GetChild(0).gameObject);

        hud = GetComponentInChildren<PlayerHUD>();
    }

    public void SetWeapon(GameObject newWeapon)
    {
        weaponInventory[0] = newWeapon.gameObject;

        if(newWeapon.GetComponent<WeaponBase>() != null)
        {
            WeaponBase weapon = newWeapon.GetComponent<WeaponBase>();

            weapon.transform.localScale = weapon.povScale;

            // set weapon and attachment layers to equippedweapon
            newWeapon.layer = 12;
            if (newWeapon.transform.childCount != 0)
            {
                for (int i = 0; i < newWeapon.transform.childCount; i++)
                    newWeapon.transform.GetChild(i).gameObject.layer = 12;
            }

            newWeapon.GetComponent<Rigidbody>().isKinematic = true;
            newWeapon.GetComponent<Collider>().enabled = false;

            newWeapon.transform.SetParent(weaponPosition, false);
            newWeapon.transform.SetPositionAndRotation(weaponPosition.position, Quaternion.identity);

            weapon.SetAimSpot();

            if (weapon.arms != null)
                weapon.arms.enabled = true;

            weapon.animator.enabled = true;
            weapon.animator.SetTrigger("pickUp");

            weapon.isPickedUp = true;
            weapon.SetMesh();

            hud.UpdateAmmoText();
        }
        else if (newWeapon.GetComponent<MeleeWeapon>() != null)
        {
            MeleeWeapon weapon = newWeapon.GetComponent<MeleeWeapon>();

            //weapon.transform.localScale = weapon.povScale;

            newWeapon.layer = 12;
            if (newWeapon.transform.childCount != 0)
            {
                for (int i = 0; i < newWeapon.transform.childCount; i++)
                    newWeapon.transform.GetChild(i).gameObject.layer = 12;
            }

            newWeapon.GetComponent<Rigidbody>().isKinematic = true;
            newWeapon.GetComponent<Collider>().enabled = false;

            newWeapon.transform.SetParent(weaponPosition, false);
            newWeapon.transform.SetPositionAndRotation(weaponPosition.position, Quaternion.identity);

            weapon.animator.enabled = true;
            weapon.animator.SetTrigger("pickUp");

            hud.UpdateAmmoText();
        }
    }

    public void AddAmmo(Pickup_Ammobox pickUp)
    {
        for (int i = 0; i < ammoTypes.Count; i++)
        {
            if (pickUp.caliber == ammoTypes[i])
            {
                if(currentAmmo[i] < maxAmmoCapacity[i])
                {
                    currentAmmo[i] = currentAmmo[i] + pickUp.ammoAmount;

                    if (currentAmmo[i] > maxAmmoCapacity[i])
                        currentAmmo[i] = maxAmmoCapacity[i];

                    pickUp.Remove();
                    break;
                }
            }
        }
    }

    public bool hasMaxAmmo(Pickup_Ammobox pickUp)
    {
        for (int i = 0; i < ammoTypes.Count; i++)
        {
            if (pickUp.caliber == ammoTypes[i])
            {
                if (currentAmmo[i] < maxAmmoCapacity[i])
                {
                    return false;
                }
                else
                    return true;
            }
        }
        return default;
    }

    public List<GameObject> SaveItems()
    {
        // get the player weapon and add it to the list
        itemsToSave.Add(weaponInventory[0]);

        // add collected keycards to the list
        // functionality DISABLED for now, player will have to find keycards again in new level
        // this instead removes keycards from player inventory now

        for (int i = 0; i < keycards.Count; i++)
            keycards.Remove(keycards[i]);

        for (int i = 0; i < keycardUiHolder.childCount; i++)
            Destroy(keycardUiHolder.GetChild(i).gameObject);

        return itemsToSave;
    }

    public List<float> SaveAmmo()
    {
        for (int i = 0; i < currentAmmo.Count; i++)
            ammoToSave.Add(currentAmmo[i]);

        return ammoToSave;
    }

    public void AddKeyCard(GameObject newKeycard)
    {
        newKeycard.transform.SetParent(keycardHolder, false);
        newKeycard.transform.SetPositionAndRotation(weaponPosition.position, Quaternion.identity);

        keycards.Add(newKeycard);

        PlayerHUD pHud = FindObjectOfType<PlayerHUD>();

        // create a new image for the picked up keycard on the hud
        Image newKeycardHud = Instantiate(pHud.keycardIcon, 
            pHud.keycardIcon.transform.position, 
            pHud.keycardIcon.transform.rotation,
            pHud.transform);

        newKeycardHud.gameObject.SetActive(true);

        // the keycard gets an offset to not place it on top of an existing keycard icon on the HUD
        if (keycards.Count >= 1f)
            newKeycardHud.transform.localPosition = new Vector3(pHud.keycardIcon.transform.localPosition.x + pHud.keycardXOffset * (keycards.Count - 1f), 
                pHud.keycardIcon.transform.localPosition.y, 
                pHud.keycardIcon.transform.localPosition.z);

        // color the new keycard image with the color of the picked up keycard
        newKeycardHud.sprite = pHud.icons[newKeycard.GetComponent<Keycard>().colorInt];

        // set new parent for keycard UI
        newKeycardHud.rectTransform.SetParent(keycardUiHolder, true);
        
    }

    public Color CheckKeyCards(Color requiredColor)
    {
        for(int i = 0; i < keycards.Count; i++)
        {
            if (keycards[i].GetComponent<Keycard>().GetKeycardColor() == requiredColor)
            {
                return keycards[i].GetComponent<Keycard>().GetKeycardColor();
            }
        }

        return Color.clear;
    }

}
