using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    public Image healthBar;
    public Image armorBar;

    public HealthBase enemyHealth;

    private EnemyAI thisEnemy;
    private Transform player;

    // Start is called before the first frame update
    void Start()
    {
        healthBar.enabled = false;
        if(armorBar != null)
            armorBar.enabled = false;

        thisEnemy = GetComponentInParent<EnemyAI>();
        enemyHealth = GetComponentInParent<HealthBase>();
        player = thisEnemy.player;
    }

    // Update is called once per frame
    void Update()
    {
        if(healthBar != null)
            if (enemyHealth.currentHealth < enemyHealth.maxHealth)
            {
                healthBar.enabled = true;
                healthBar.transform.LookAt(player, Vector3.up);
                UpdateHealthBar();
            }

        if(armorBar != null)
            if (enemyHealth.currentArmor < enemyHealth.maxArmor)
            {
                armorBar.enabled = true;
                armorBar.transform.LookAt(player, Vector3.up);
                UpdateArmorBar();
            }

    }

    void UpdateHealthBar()
    {
        healthBar.fillAmount = enemyHealth.currentHealth / enemyHealth.maxHealth;
        healthBar.color = Color.Lerp(Color.red, Color.green, healthBar.fillAmount);


        if (enemyHealth.currentHealth <= 0f)
            Destroy(healthBar.gameObject);

    }

    void UpdateArmorBar()
    {
        armorBar.fillAmount = enemyHealth.currentArmor / enemyHealth.maxArmor;

        if(enemyHealth.currentArmor <= 0f)
            armorBar.enabled = false;

    }
}
