using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_VIP : EnemyAI
{
    private GameManager manager; 

    public override void Start()
    {
        base.Start();

        manager = FindObjectOfType<GameManager>();
    }

    public override void Update()
    {
        base.Update();
    }

    public override void Die(float deathForce, Transform source, bool headshot)
    {
        // Debug.Log("VIP KILLED");
        manager.vipKilled = true;
        manager.VictorySlowMo();

        base.Die(deathForce, source, headshot);
    }

}
