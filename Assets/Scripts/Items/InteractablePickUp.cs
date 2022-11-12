using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractablePickUp : MonoBehaviour
{
    // changed the code here, test it out
    // eliminating the need for collision for ammo and other pickups
    // useful for objects like ammo and weapons

    // code made uniform and to handle the pick up scripts rather than player hud

    private Outline outline;

    private void Start()
    {
        outline = GetComponent<Outline>();
    }

    public string ReturnInteractText(PlayerInventory inventory, HealthBase health)
    {
        if (inventory.weaponInventory[0] == null && GetComponent<WeaponBase>() != null)
        {
            WeaponBase weapon = GetComponent<WeaponBase>();

            if(weapon.canPickUp)
                outline.enabled = true;

            return "Pick up " + weapon.weaponName;
        }
        else if (inventory.weaponInventory[0] == null && GetComponent<MeleeWeapon>() != null)
        {
            MeleeWeapon weapon = GetComponent<MeleeWeapon>();

            if (weapon.canPickUp)
                outline.enabled = true;

            return "Pick up " + weapon.weaponName;
        }
        else if (inventory.weaponInventory[0] != null && GetComponent<WeaponBase>() != null)
        {
            WeaponBase weapon = GetComponent<WeaponBase>();

            if (weapon.canPickUp)
                outline.enabled = true;

            return "Swap weapon for " + weapon.weaponName;
        }
        else if (inventory.weaponInventory[0] != null && GetComponent<MeleeWeapon>() != null)
        {
            MeleeWeapon weapon = GetComponent<MeleeWeapon>();

            if (weapon.canPickUp)
                outline.enabled = true;

            if (inventory.weaponInventory[0].GetComponent<WeaponBase>() != null && !inventory.hasMaxKnives(weapon))
                return "Pick up extra " + weapon.weaponName;
            if (inventory.weaponInventory[0].GetComponent<MeleeWeapon>() != null && !inventory.hasMaxKnives(weapon))
                return "Pick up extra " + weapon.weaponName;
            else if (inventory.weaponInventory[0].GetComponent<MeleeWeapon>() != null && inventory.hasMaxKnives(weapon))
                return "Carrying max amount of bayonets";
            else if (inventory.weaponInventory[0].GetComponent<WeaponBase>() != null && inventory.hasMaxKnives(weapon))
                return "Carrying max amount of bayonets";

        }
        else if (CompareTag("AmmoPickUp"))
        {
            outline.enabled = true;

            Pickup_Ammobox ammoBox = GetComponent<Pickup_Ammobox>();

            if (!inventory.hasMaxAmmo(ammoBox))
                return "Pick up " + ammoBox.caliber + " ammo";
            else
                return ammoBox.caliber + " ammo full";

        }
        else if (CompareTag("HealthPickUp"))
        {
            outline.enabled = true;

            Pickup_Healthbox healthBox = GetComponent<Pickup_Healthbox>();

            if (health.currentHealth == health.maxHealth)
                return "Health full";
            else
                return "Pick up health";
        }
        else if (CompareTag("ArmorPickUp"))
        {
            outline.enabled = true;

            PickUp_Armor armorBox = GetComponent<PickUp_Armor>();

            if (health.currentArmor == health.maxArmor)
                return "Armor full";
            else
                return "Pick up armor";
        }
        else if (CompareTag("Keycard"))
        {
            outline.enabled = true;

            return "Pick up keycard";
        }
        return null;
    }

    public void AttemptPickUp(PlayerInventory inventory, HealthBase health, PlayerHUD hud)
    {
        if (GetComponent<WeaponBase>() != null || GetComponent<MeleeWeapon>() != null)
        {
            if (GetComponent<WeaponBase>() != null)
                PickUpWeapon(inventory, hud);
            else if (GetComponent<MeleeWeapon>() != null)
                PickUpMelee(inventory, hud);
        }
        else if (CompareTag("AmmoPickUp"))
            PickUpAmmo(inventory, hud);
        else if (CompareTag("HealthPickUp"))
            PickUpHealth(health, hud);
        else if (CompareTag("ArmorPickUp"))
            PickUpArmor(health, hud);
        else if (CompareTag("Keycard"))
            PickUpKeycard(inventory, hud);
    }

    public void AttemptGroundPickUp(PlayerInventory inventory, HealthBase health, PlayerHUD hud)
    {
        bool hasAmmo = false;
        // only pick up weapons if they have ammo in them, otherwise dont autopickup them
        if (GetComponent<AmmoBase>() != null)
        {
            if (GetComponent<AmmoBase>().currentMag > 0)
                hasAmmo = true;
            else
                hasAmmo = false;
        }

        // pick up first weapon the player makes contact with if currently not holding a weapon
        if (inventory.weaponInventory[0] == null)
        {
            if (GetComponent<WeaponBase>() != null)
            {
                if (hasAmmo)
                    PickUpWeapon(inventory, hud);
            }
            else if (GetComponent<MeleeWeapon>() != null)
                PickUpMelee(inventory, hud);
        }
        else if (inventory.weaponInventory[0] != null)
        {
            // pick up additional bayonets to the player inventory in case the player is holding a firearm
            if (inventory.weaponInventory[0].GetComponent<WeaponBase>() && GetComponent<MeleeWeapon>() != null)
                PickUpMelee(inventory, hud);
            // pick up additional bayonets when holding a knife
            else if (inventory.weaponInventory[0].GetComponent<MeleeWeapon>() && GetComponent<MeleeWeapon>() != null)
                PickUpMelee(inventory, hud);
            // pick up a weapon if holding a bayonet
            else if (inventory.weaponInventory[0].GetComponent<MeleeWeapon>() && GetComponent<WeaponBase>() != null)
            {
                if (hasAmmo)
                    PickUpWeapon(inventory, hud);
            }

        }

        if (CompareTag("AmmoPickUp"))
            PickUpAmmo(inventory, hud);
        else if (CompareTag("HealthPickUp"))
            PickUpHealth(health, hud);
        else if (CompareTag("ArmorPickUp"))
            PickUpArmor(health, hud);
        else if (CompareTag("Keycard"))
            PickUpKeycard(inventory, hud);
    }

    public void PickUpWeapon(PlayerInventory inventory, PlayerHUD hud)
    {
        WeaponBase weapon = gameObject.GetComponent<WeaponBase>();

        if (weapon.canPickUp && inventory.weaponInventory[0] == null)
        {
            inventory.SetWeapon(weapon.gameObject);
            hud.uiSound.PlayOneShot(hud.sounds[3]);

            hud.UpdatePickUpText(weapon.weaponName);
        }
        else if (weapon.canPickUp && inventory.weaponInventory[0] != null)
        {
            if (inventory.weaponInventory[0].GetComponent<WeaponBase>() != null)
            {
                WeaponBase currentWeapon = inventory.weaponInventory[0].GetComponent<WeaponBase>();
                currentWeapon.Swap();
                inventory.SetWeapon(weapon.gameObject);

                hud.uiSound.PlayOneShot(hud.sounds[3]);

                hud.UpdatePickUpText(weapon.weaponName);
            }
            else if (inventory.weaponInventory[0].GetComponent<MeleeWeapon>() != null)
            {
                MeleeWeapon currentWeapon = inventory.weaponInventory[0].GetComponent<MeleeWeapon>();
                currentWeapon.Swap();
                inventory.SetWeapon(weapon.gameObject);

                hud.uiSound.PlayOneShot(hud.sounds[3]);

                hud.UpdatePickUpText(weapon.weaponName);
            }
        }
    }

    public void PickUpMelee(PlayerInventory inventory, PlayerHUD hud)
    {
        MeleeWeapon weapon = gameObject.GetComponent<MeleeWeapon>();

        if (weapon.canPickUp && inventory.weaponInventory[0] == null)
        {
            inventory.SetWeapon(weapon.gameObject);
            hud.uiSound.PlayOneShot(hud.sounds[3]);

            hud.UpdatePickUpText(weapon.weaponName);
        }
        else if(weapon.canPickUp && inventory.weaponInventory != null)
        {
            if(inventory.weaponInventory[0].GetComponent<WeaponBase>() != null)
            {
                if (!inventory.hasMaxKnives(weapon))
                {
                    MeleeWeapon currentWeapon = inventory.weaponInventory[0].GetComponent<MeleeWeapon>();
                    //currentWeapon.Drop();
                    //inventory.SetWeapon(weapon.gameObject);
                    inventory.AddExtraKnife(weapon);

                    hud.uiSound.PlayOneShot(hud.sounds[3]);

                    hud.UpdateAmmoText();
                    hud.UpdatePickUpText(weapon.weaponName);
                }
            }
            else if (inventory.weaponInventory[0].GetComponent<MeleeWeapon>() != null)
            {
                if(!inventory.hasMaxKnives(weapon))
                {
                    MeleeWeapon currentWeapon = inventory.weaponInventory[0].GetComponent<MeleeWeapon>();
                    //currentWeapon.Drop();
                    //inventory.SetWeapon(weapon.gameObject);
                    inventory.AddExtraKnife(weapon);

                    hud.uiSound.PlayOneShot(hud.sounds[3]);

                    hud.UpdateAmmoText();
                    hud.UpdatePickUpText(weapon.weaponName);
                }
            }
        }
    }

    public void PickUpAmmo(PlayerInventory inventory, PlayerHUD hud)
    {
        Pickup_Ammobox pickUp = GetComponent<Pickup_Ammobox>();

        if(!inventory.hasMaxAmmo(pickUp))
        {
            inventory.AddAmmo(pickUp);
            hud.uiSound.PlayOneShot(hud.sounds[0]);
            hud.UpdateAmmoText();
            hud.UpdatePickUpText(pickUp.caliber + " AMMO");
        }
    }

    public void PickUpHealth(HealthBase pHealth, PlayerHUD hud)
    {
        Pickup_Healthbox pickUp = GetComponent<Pickup_Healthbox>();

        if (pHealth.currentHealth < pHealth.maxHealth)
        {
            pHealth.Heal(pickUp.healAmount);
            pickUp.Remove();
            hud.uiSound.PlayOneShot(hud.sounds[2]);
            hud.UpdatePickUpText("HEALTH");

        }
    }

    public void PickUpArmor(HealthBase pHealth, PlayerHUD hud)
    {
        PickUp_Armor pickUp = GetComponent<PickUp_Armor>();

        if (pHealth.currentArmor < pHealth.maxArmor)
        {
            pHealth.AddArmor(pickUp.armorAmount);
            pickUp.Remove();
            hud.uiSound.PlayOneShot(hud.sounds[1]);
            hud.UpdatePickUpText("ARMOR");

        }
    }

    public void PickUpKeycard(PlayerInventory inventory, PlayerHUD hud)
    {
        inventory.AddKeyCard(gameObject);
        gameObject.SetActive(false);

        hud.uiSound.PlayOneShot(hud.sounds[4]);
        hud.UpdatePickUpText("KEYCARD");
    }

    // auto pick up on trigger
    private void OnTriggerEnter(Collider other)
    {       
        if(other.gameObject.layer == 6)
        {
            // Debug.Log("picked up " + gameObject);

            if(!other.GetComponentInParent<CustomCharacterController>().isDead)
            {
                PlayerInventory inventory = other.transform.parent.GetComponent<PlayerInventory>();
                HealthBase health = other.transform.parent.GetComponent<HealthBase>();
                PlayerHUD hud = other.transform.parent.GetChild(1).GetComponentInChildren<PlayerHUD>();

                AttemptGroundPickUp(inventory, health, hud);
            }

        }
    }
}

