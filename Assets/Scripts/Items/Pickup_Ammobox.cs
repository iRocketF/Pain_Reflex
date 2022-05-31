using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup_Ammobox : MonoBehaviour
{
    // player has consistent inventory with ammo
    // add ammunition to playerinventory in PlayerInventory

    [Header("Pickup basic info")]
    public string caliber;
    public float ammoAmount;

    public void Remove()
    {
        Destroy(gameObject);
    }
}
