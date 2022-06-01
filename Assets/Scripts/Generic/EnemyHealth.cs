using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : HealthBase
{
    public EnemyAI enemy;
    public LayerMask reactMask;

    public override void Start()
    {
        base.Start();

        enemy = GetComponent<EnemyAI>();
    }

    public override void TakeDamage(float damage, float force, Vector3 contactPoint, Transform source, bool headshot)
    {
        base.TakeDamage(damage, force, contactPoint, source, headshot);

        if (!enemy.isDead)
            enemy.voice.PlayHurtVoice();

        if (currentHealth <= 0f && !isInvincible)
        {
            currentHealth = 0f;

            EnemyAI enemy = GetComponent<EnemyAI>();

            if (!enemy.isDead)
            {
                enemy.voice.PlayDeathVoice();
                enemy.Die(force, source, headshot);

                ChooseRandomReactor();
            }
        }
    }

    void ChooseRandomReactor()
    {
        Collider[] nearbyEnemies = Physics.OverlapSphere(transform.position, 5, reactMask, QueryTriggerInteraction.Ignore);

        if(nearbyEnemies.Length > 0f)
        {
            for (int i = 0; i < nearbyEnemies.Length; i++)
            {
                if (nearbyEnemies[i].transform.gameObject.GetComponentInParent<EnemyVoice>() != null 
                    && !nearbyEnemies[i].transform.gameObject.GetComponentInParent<EnemyAI>().isDead)
                {
                    if (nearbyEnemies[i].transform.gameObject.GetComponentInParent<EnemyVoice>().PlayAllyDeathVoice())
                        break;
                }
            }
        }
    }
}
