using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMiniBoss : EnemyAI
{
    public Rigidbody itemDrop;

    public override void Start()
    {
        base.Start(); 
    }

    public override void Update()
    {
        base.Update();
    }

    public override void Die(float deathForce, Transform source, bool headshot)
    {
        if(itemDrop != null)
        {
            itemDrop.transform.SetParent(null);
            itemDrop.isKinematic = false;
        }


        base.Die(deathForce, source, headshot);
    }
}
