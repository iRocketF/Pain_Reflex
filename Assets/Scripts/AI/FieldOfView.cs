using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    public float viewRadius;
    [Range(0, 360)]
    public float viewAngle;

    public LayerMask playerMask;
    public LayerMask obstacleMask;

    void FindVisiblePlayer()
    {
        Collider[] playerInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, playerMask);

    }


}
