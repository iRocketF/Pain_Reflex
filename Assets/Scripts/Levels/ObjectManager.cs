using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    // public GameObject[] objectsToMove;

    public Transform start;
    public Transform end;

    private GameObject player;

    void Start()
    {
        player = FindObjectOfType<CustomCharacterController>().gameObject;
    }

    void Update()
    {
        transform.position = new Vector3(0, 0, player.transform.position.z);
    }
}
