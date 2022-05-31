using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAIMelee : EnemyAI
{
    public float meleeDmg;
    public float meleeRange;
    public float attackCooldown;
    private float attackTimer;

    public override void Start()
    {
        base.Start();
    }

    public override void Update()
    {
        base.Update();

        if (sawPlayer && agent.velocity == Vector3.zero)
            TurnTowardsPlayer();
    }

    public override void Attack()
    {
        attackTimer += Time.deltaTime;

        //isPassive = false;
        //isAggressive = true;

        agent.SetDestination(transform.position);

        if (attackTimer >= attackCooldown)
        {
            attackTimer = 0f;

            RaycastHit hit;

            if (Physics.Raycast(transform.position,
                transform.TransformDirection(Vector3.forward),
                out hit, meleeRange, playerMask))
            {
                HealthBase hitPlayer = hit.transform.gameObject.GetComponent<HealthBase>();
                hitPlayer.TakeDamage(meleeDmg, 0, hit.point, transform, false);
            }
        }
        
    }

    public override void AttackMove()
    {
        agent.speed = 5f;

        walkPointSet = false;

        //isPassive = false;
        //isAggressive = false;
        //isAlert = true;

        agent.SetDestination(player.position);
    }

    void TurnTowardsPlayer()
    {
        Quaternion ogRotation = transform.rotation;
        transform.LookAt(player);
        Quaternion newRotation = transform.rotation;
        transform.rotation = ogRotation;
        transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, aimSpeed * Time.deltaTime);
    }
}
