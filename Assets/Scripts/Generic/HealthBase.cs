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


    // Start is called before the first frame update
    public virtual void Start()
    {
        currentHealth = maxHealth;
        reductionPercentage = armorDamageReductionPercentage / 100f;

        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).name == "Head")
                critBox = transform.GetChild(i).GetComponent<Collider>();
        }
    }

    public virtual void Heal(float healAmount)
    {
        currentHealth = currentHealth + healAmount;

        if (currentHealth > maxHealth)
            currentHealth = maxHealth;
    }

    public virtual void AddArmor(float armorAmount)
    {
        currentArmor = currentArmor + armorAmount;

        if (currentArmor > maxArmor)
            currentArmor = maxArmor;
    }

    public virtual void TakeDamage(float damage, float force, Vector3 contactPoint, Transform source, bool headshot)
    {
        damage = ArmorCalculation(damage);

        GameObject bloodSpray = Instantiate(blood,contactPoint,Quaternion.identity);
        bloodSpray.transform.LookAt(source);

        currentHealth = currentHealth - damage;
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

}
