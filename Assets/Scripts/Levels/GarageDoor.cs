using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GarageDoor : MonoBehaviour
{
    public Transform door;
    public AudioSource doorSound;
    public AudioClip[] doorSounds;

    private Vector3 closedPosition;
    private Vector3 openPosition;

    [Range(0, 1)]
    public float doorSpeed;
    private float doorLerp;

    public float invokeDelay;

    [Header("Does this door start open?")]
    public bool startOpen;
    [Header("Does this door lock after closing")]
    public bool lockAfterClosing;
    [Header("Door states")]
    public bool isOpening;
    private bool isOpen;
    public bool isClosing;
    private bool isClosed;
    public bool isLocked;

    void Start()
    {
        doorSound = GetComponent<AudioSource>();

        closedPosition = door.localPosition;

        openPosition = new Vector3(door.localPosition.x, door.localPosition.y + 2.5f, door.localPosition.z);

        if (startOpen)
        {
            door.localPosition = openPosition;
            isOpen = true;
        }
        else
            isClosed = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (isOpening && !isOpen && !isLocked)
            Open();
        if (isClosing && !isClosed && !isLocked)
            Close();
    }

    public void Operate()
    {
        if (!isOpening && isClosed && !isLocked)
        {
            doorSound.PlayOneShot(doorSounds[0]);
            isOpening = true;
        }
        if (!isClosing && isOpen)
        {
            doorSound.PlayOneShot(doorSounds[0]);
            isClosing = true;
        }
    }

    void Open()
    {
        if (isClosed)
            isClosed = false;

        doorLerp += doorSpeed * Time.deltaTime;

        // basic lerp stuff for door opening and closing

        door.localPosition = Vector3.Lerp(closedPosition, openPosition, doorLerp);

        if (door.localPosition == openPosition)
        {
            doorSound.PlayOneShot(doorSounds[1]);

            isOpening = false;
            isOpen = true;
            doorLerp = 0f;
        }

    }

    void Close()
    {
        if (isOpen)
            isOpen = false;

        doorLerp += doorSpeed * Time.deltaTime;

        // basic lerp stuff for door opening and closing

        door.localPosition = Vector3.Lerp(openPosition, closedPosition, doorLerp);

        if (door.localPosition == closedPosition)
        {
            doorSound.PlayOneShot(doorSounds[1]);

            isClosing = false;
            isClosed = true;
            doorLerp = 0f;

            if (lockAfterClosing)
                isLocked = true;
        }
    }

    public void InvokeDoor()
    {
        Invoke("Operate", invokeDelay);
    }

}
