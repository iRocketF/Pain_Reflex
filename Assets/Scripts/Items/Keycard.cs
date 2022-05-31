using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Keycard : MonoBehaviour
{
    public MeshRenderer rend;

    public List<Color> keycardColor;
    public List<Material> keycardMaterial;

    [Header("0 = Green, 1 = Blue, 2 = Yellow, 3 = Red, 4 = Black (unused)")]
    public int colorInt;


    void Start()
    {
        rend = GetComponent<MeshRenderer>();
        rend.material = keycardMaterial[colorInt];
    }

    public Color GetKeycardColor()
    {
        return keycardColor[colorInt];
    }

    public void Remove()
    {
        Destroy(gameObject);
    }


}
