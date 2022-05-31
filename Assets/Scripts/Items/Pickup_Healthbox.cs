using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup_Healthbox : MonoBehaviour
{
    [Header("Pickup basic info")]
    public float healAmount;

    public void Remove()
    {
        Destroy(gameObject);
    }
}
