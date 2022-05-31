using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingElevator : MonoBehaviour
{
    public Transform elevator;
    public Transform firstFloor;
    public Transform secondFloor;

    //public DoubleDoor lowerDoors;
    //public DoubleDoor upperDoors;

    public float lerpTime;
    public float elevatorLerp;

    public bool isMoving;
    private bool atFirstFloor;
    private bool atSecondFloor;

    void Start()
    {
        atFirstFloor = true;
    }

    void Update()
    {
        if (isMoving)
            MoveElevator();
    }

    void MoveElevator()
    {
        // had to remind myself how lerps work again...
        if(atFirstFloor)
        {
            elevatorLerp += Time.deltaTime;
            if (elevatorLerp > lerpTime)
            {
                elevatorLerp = lerpTime;
                isMoving = false;
                atFirstFloor = false;
                atSecondFloor = true;
                //upperDoors.Operate();
            }
        } else if (atSecondFloor)
        {
            elevatorLerp -= Time.deltaTime;
            if(elevatorLerp < 0f)
            {
                elevatorLerp = 0f;
                isMoving = false;
                atFirstFloor = true;
                atSecondFloor = false;
                //lowerDoors.Operate();
            }
        }

        // smoothstep lerp!
        // more cool math curves here https://chicounity3d.wordpress.com/2014/05/23/how-to-lerp-like-a-pro/
        float t = elevatorLerp / lerpTime;
        t = t * t * (3f - 2f * t);

        elevator.position = Vector3.Lerp(firstFloor.position, secondFloor.position, t);
    }

    public void Operate()
    {
        isMoving = true;

        //if (atFirstFloor)
            //lowerDoors.Operate();
        //else if (atSecondFloor)
            //upperDoors.Operate();
    }
}
