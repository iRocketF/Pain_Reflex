using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathTrigger : MonoBehaviour
{
    public float damage;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
            other.GetComponentInParent<HealthBase>().TakeDamage(damage, 0, other.ClosestPoint(transform.position), transform, false);
        }
    }

}
