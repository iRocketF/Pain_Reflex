using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CleanUpTemp : MonoBehaviour
{

    public void SeekAndDestroy()
    {
        GameObject[] temps = GameObject.FindGameObjectsWithTag("Temp");

        for (int i = 0; i < temps.Length; i++)
        {
            Destroy(temps[i]);     
        }
    }

}
