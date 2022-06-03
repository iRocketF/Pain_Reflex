using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Muzzleflash : MonoBehaviour
{
    Animator animator;

    [SerializeField] Transform smoke;
    [SerializeField] Transform flame;

    float randomZ;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetTrigger("Play");

        randomZ = Random.Range(0, 360);

        smoke.Rotate(0, 0, randomZ);
        flame.Rotate(0, 0, randomZ);
    }
}
