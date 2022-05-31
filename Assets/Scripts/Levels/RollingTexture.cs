using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollingTexture : MonoBehaviour
{
    public Renderer rend;
    public float rollSpeed;

    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        float offset = Time.time * rollSpeed;
        rend.material.mainTextureOffset = new Vector2(0, offset);
    }
}
