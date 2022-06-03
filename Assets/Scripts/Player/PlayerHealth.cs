using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : HealthBase
{
    private PlayerHUD hud;

    public delegate void OnDamageTaken();
    public static OnDamageTaken onDamageTaken;

    public override void Start()
    {
        base.Start();

        hud = GetComponentInChildren<PlayerHUD>();
    }

    public override void Heal(float healAmount)
    {
        base.Heal(healAmount);

        hud.UpdateHealthBar();
        hud.isHealed = true;
    }

    public override void AddArmor(float armorAmount)
    {
        base.AddArmor(armorAmount);

        hud.UpdateArmorBar();
    }

    public override void TakeDamage(float damage, float force, Vector3 contactPoint, Transform source, bool headshot)
    {
        base.TakeDamage(damage, force, contactPoint, source, headshot);

        onDamageTaken();

        hud.UpdateHealthBar();
        hud.UpdateArmorBar();
        hud.isHurt = true;

        if (currentHealth <= 0f && !isInvincible)
        {
            currentHealth = 0f;
            CustomCharacterController player = GetComponent<CustomCharacterController>();

            if (!player.isDead)
            {
                source.GetComponentInParent<EnemyVoice>().PlayPlayerDeathVoice();
                PlayerDeath(player);
            }
        }
        else if (currentHealth <= 0f && isInvincible)
        {
            currentHealth = 1f;
        }
    }
 
    void PlayerDeath(CustomCharacterController player)
    {
        player.isDead = true;

        // disable player movement & collision from the character controller
        GetComponent<CustomCharacterController>().enabled = false;

        // add these components for physics so the player can "ragdoll"
        gameObject.GetComponent<Rigidbody>().freezeRotation = false;
        gameObject.GetComponent<Rigidbody>().mass = 40f;
        gameObject.GetComponent<Rigidbody>().AddForce(Vector3.back * 600f);

        // drop the currently equipped weapon, if weapon is equipped
        if (GetComponent<PlayerInventory>().weaponInventory[0] != null)
        {
            if (GetComponent<PlayerInventory>().weaponInventory[0].GetComponent<WeaponBase>() != null)
                GetComponent<PlayerInventory>().weaponInventory[0].GetComponent<WeaponBase>().Drop();
            else if (GetComponent<PlayerInventory>().weaponInventory[0].GetComponent<MeleeWeapon>() != null)
                GetComponent<PlayerInventory>().weaponInventory[0].GetComponent<MeleeWeapon>().Drop();
        }
    }
}
