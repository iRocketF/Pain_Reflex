using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollingObject : MonoBehaviour
{
    public ObjectManager manager;

    public float speed;
    public float lerp;

    void Start()
    {
        manager = GetComponentInParent<ObjectManager>();
    }

    void Update()
    {
        if(transform.position == manager.end.position)
        {
            transform.position = manager.start.position;
            lerp = 0f;
        }

        lerp += Time.deltaTime * speed;
        transform.position = Vector3.Lerp(manager.start.position, manager.end.position, lerp);
    }
}
