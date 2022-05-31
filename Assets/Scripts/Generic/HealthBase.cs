using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBase : MonoBehaviour
{
    public bool isInvincible;

    public float currentHealth;
    public float maxHealth;

    public float currentArmor;
    public float maxArmor;

    [Range(0,100)]
    public float armorDamageReductionPercentage;
    private float reductionPercentage;

    public GameObject blood;
    public Collider critBox;
    private PlayerHUD hud;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        reductionPercentage = armorDamageReductionPercentage / 100f;

        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).name == "Head")
                critBox = transform.GetChild(i).GetComponent<Collider>();
        }

        if (transform.CompareTag("Player"))
            hud = GetComponentInChildren<PlayerHUD>();
    }

    public void Heal(float healAmount)
    {
        currentHealth = currentHealth + healAmount;

        if (currentHealth > maxHealth)
            currentHealth = maxHealth;

        if (transform.CompareTag("Player"))
        {
            hud.UpdateHealthBar();
            hud.isHealed = true;
        }

    }

    public void AddArmor(float armorAmount)
    {
        currentArmor = currentArmor + armorAmount;

        if (currentArmor > maxArmor)
            currentArmor = maxArmor;

        hud.UpdateArmorBar();
    }

    public void TakeDamage(float damage, float force, Vector3 contactPoint, Transform source, bool headshot)
    {
        damage = ArmorCalculation(damage);

        GameObject bloodSpray = Instantiate(blood,contactPoint,Quaternion.identity);
        bloodSpray.transform.LookAt(source);

        currentHealth = currentHealth - damage;

        if(transform.CompareTag("Player"))
        {
            hud.UpdateHealthBar();
            hud.UpdateArmorBar();
            hud.isHurt = true;
        }

        // Turn an enemy AI aggressive if it takes dmg + play hurt sound
        if(transform.CompareTag("Enemy"))
        {
            EnemyAI enemy = GetComponent<EnemyAI>();

            //enemy.isAggressive = true;

            if(!enemy.isDead)
            {
                enemy.voice.PlayHurtVoice();
            }
        }

        if (currentHealth <= 0f && !isInvincible)
        {
            currentHealth = 0f;

            if(transform.CompareTag("Enemy"))
            {
                EnemyAI enemy = GetComponent<EnemyAI>();

                if(!enemy.isDead)
                {
                    // add deathsounds here
                    enemy.voice.PlayDeathVoice();

                    enemy.Die(force, source, headshot);
                }

            }

            if(transform.CompareTag("Player"))
            {
                CustomCharacterController player = GetComponent<CustomCharacterController>();

                if(!player.isDead)
                {
                    PlayerDeath(player);
                }

            }
        }

        else if (currentHealth <= 0f && isInvincible)
        {
            currentHealth = 1f;
        }
    }

    float ArmorCalculation(float damage)
    {
        if(currentArmor > 0f)
        {
            float armorDamage;
            float healthDamage;

            healthDamage = damage - damage * reductionPercentage;
            armorDamage = damage - healthDamage;

            currentArmor = currentArmor - armorDamage;

            damage = healthDamage;

            return damage;
        }

        return damage;
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
        if(GetComponent<PlayerInventory>().weaponInventory[0] != null)
        {
            if (GetComponent<PlayerInventory>().weaponInventory[0].GetComponent<WeaponBase>() != null)
                GetComponent<PlayerInventory>().weaponInventory[0].GetComponent<WeaponBase>().Drop();
            else if (GetComponent<PlayerInventory>().weaponInventory[0].GetComponent<MeleeWeapon>() != null)
                GetComponent<PlayerInventory>().weaponInventory[0].GetComponent<MeleeWeapon>().Drop();
        }
            



    }
}
