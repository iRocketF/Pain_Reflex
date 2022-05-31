using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp_Armor : MonoBehaviour
{
    [Header("Pickup basic info")]
    public float armorAmount;

    public void Remove()
    {
        Destroy(gameObject);
    }
}
