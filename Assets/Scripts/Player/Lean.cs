using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lean : MonoBehaviour
{
    // leaning related variables
    [Header("Lean variables")]
    public float leanSpeed;
    public float maxAngle;
    private float leanLerp;
    private float currentAngle;
    public Transform pivot;

    // Start is called before the first frame update
    void Start()
    {
        pivot = transform.parent;
    }

    // Update is called once per frame
    void Update()
    {
        PlayerLean();
    }

    void PlayerLean()
    {
        if (Input.GetAxis("LeftLean") > 0f || Input.GetAxis("RightLean") > 0f)
        {

            if (Input.GetAxis("LeftLean") > 0f)
                currentAngle = Mathf.MoveTowardsAngle(currentAngle, maxAngle, leanSpeed * Time.deltaTime);
            else if (Input.GetAxis("RightLean") > 0)
                currentAngle = Mathf.MoveTowardsAngle(currentAngle, -maxAngle, leanSpeed * Time.deltaTime);

            pivot.transform.rotation = Quaternion.Euler(pivot.transform.eulerAngles.x, pivot.transform.eulerAngles.y, currentAngle);
        }
        else if (currentAngle != 0f)
        {
            currentAngle = Mathf.MoveTowardsAngle(currentAngle, 0, leanSpeed * Time.deltaTime);

            pivot.transform.rotation = Quaternion.Euler(0, pivot.transform.eulerAngles.y, currentAngle);
        }
    }
}
