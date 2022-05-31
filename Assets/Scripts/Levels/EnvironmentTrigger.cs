using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnvironmentTrigger : MonoBehaviour
{
    [SerializeField]
    private GameObject[] objectsToActivate;
    [SerializeField]
    private bool isSingleUse;

    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 6)
        {
            for(int i = 0; i < objectsToActivate.Length; i++)
            {
                objectsToActivate[i].SetActive(true);
            }

            if (isSingleUse)
                Destroy(gameObject);
        }
    }
}
