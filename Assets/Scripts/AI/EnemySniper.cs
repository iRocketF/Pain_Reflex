using UnityEngine;
using UnityEngine.AI;

public class EnemySniper : EnemyAI
{
    private EnemyWeaponBase sniper;
    private LineRenderer sniperLaser;

    public override void Start()
    {
        base.Start();

        sniper = GetComponentInChildren<EnemyWeaponBase>();
        sniperLaser = sniper.GetComponent<LineRenderer>();
       
    }

    public override void Update()
    {
        base.Update();

        LaserLogic();
    }

    void LaserLogic()
    {
        if (!isDead && !hasLoS)
            sniperLaser.enabled = false;
        else if (!isDead && hasLoS)
            sniperLaser.enabled = true;
        else if (isDead)
            sniperLaser.enabled = false;
    }
}
