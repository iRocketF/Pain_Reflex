using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{

    public Transform nextWaypoint;
    //public Transform previousWaypoint;
    private MeshRenderer mesh;

    public bool isFinalWaypoint;

    void Start()
    {
        mesh = GetComponent<MeshRenderer>();
        mesh.enabled = false;

        if (nextWaypoint == null)
            isFinalWaypoint = true;
    }

    void Update()
    {
        
    }
}
